using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Farm.Dialog
{
    public class DialogUIController : MonoBehaviour
    {
        public GameObject dialogueBox;
        public TextMeshProUGUI dialogText;
        public Image faceLeft, faceRight;
        public TextMeshProUGUI nameLeft, nameRight;
        public GameObject tipsBox;

        private void Awake()
        {
            tipsBox.SetActive(false);
        }

        private void OnEnable()
        {
            EventHandler.ShowDialogEvent += OnShowDialogEvent;
        }
        private void OnDisable()
        {
            EventHandler.ShowDialogEvent -= OnShowDialogEvent;
        }

        private void OnShowDialogEvent(DialogPiece piece)
        {
            StartCoroutine(ShowDialog(piece));
        }

        private IEnumerator ShowDialog(DialogPiece piece)
        {
            if (piece != null)
            {
                piece.isDone = false;

                dialogueBox.SetActive(true);
                tipsBox.SetActive(false);
                dialogText.text = string.Empty;

                if (piece.name != string.Empty)
                {
                    if (piece.isOnLeft)
                    {
                        faceRight.gameObject.SetActive(false);
                        faceLeft.gameObject.SetActive(true);
                        faceLeft.sprite = piece.faceImage;
                        nameLeft.text = piece.name;
                    }
                    else
                    {
                        faceLeft.gameObject.SetActive(false);
                        faceRight.gameObject.SetActive(true);
                        faceRight.sprite = piece.faceImage;
                        nameRight.text = piece.name;
                    }
                }
                else
                {
                    faceLeft.gameObject.SetActive(false);
                    faceRight.gameObject.SetActive(false);
                    nameLeft.gameObject.SetActive(false);
                    nameRight.gameObject.SetActive(false);
                }
                yield return StartCoroutine(TypeText(piece.dialogText, 1f));

                piece.isDone = true;

                if (piece.hasToPause && piece.isDone)
                {
                    tipsBox.SetActive(true);
                }
                else
                {
                    tipsBox.SetActive(false);
                }
            }
            else
            {
                dialogueBox.SetActive(false);
                yield break;
            }

        }

        private IEnumerator TypeText(string content, float totalTime)
        {
            dialogText.text = string.Empty;
            float breakTime = totalTime / content.Length;                   // 平均一个字的播放时间 //TODO: 可以增添一点随机性

            foreach (var letter in content)
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(breakTime);
            }
        }
    }

}
