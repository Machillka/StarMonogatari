using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Farm.Dialog
{
    [RequireComponent(typeof(NPCMovementController))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogController : MonoBehaviour
    {
        private NPCMovementController _npc => GetComponent<NPCMovementController>();
        private GameObject _UISigh;
        public UnityEvent onFinishEvent;

        public List<DialogPiece> dialogList;
        // private Stack<DialogPiece> _dialogStack; //TODO[x] 所以为什么不用队列来写
        private Queue<DialogPiece> _dialogQueue;

        private bool _canTalk;
        private bool _isTalking;

        private void Awake()
        {
            FillDialogStack();
            _UISigh = transform.GetChild(1).gameObject;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _canTalk = !_npc.isMoving && _npc.isInteractable;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _canTalk = false;
        }

        private void Update()
        {
            _UISigh.SetActive(_canTalk);

            if (_canTalk && !_isTalking && InputManager.Instance.IsSpaceButtonPressed)
            {
                StartCoroutine(DialogRoutine());
            }
        }

        private void FillDialogStack()
        {
            _dialogQueue = new Queue<DialogPiece>();

            for (int i = 0; i < dialogList.Count; i++)
            {
                dialogList[i].isDone = false;
                _dialogQueue.Enqueue(dialogList[i]);
            }
        }

        private IEnumerator DialogRoutine()
        {
            _isTalking = true;
            if (_dialogQueue.TryDequeue(out DialogPiece piece))
            {
                EventHandler.CallShowDialogEvent(piece);
                yield return new WaitUntil(() => piece.isDone);
            }
            else
            {
                EventHandler.CallShowDialogEvent(null);
                FillDialogStack();
                onFinishEvent?.Invoke();
            }
            _isTalking = false;
        }
    }
}
