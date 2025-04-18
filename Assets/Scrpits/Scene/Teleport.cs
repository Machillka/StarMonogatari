using UnityEngine;

namespace Farm.SceneTransition
{
    public class Teleport : MonoBehaviour
    {
        public string SceneToLoadName;
        public Vector3 TargetPosition;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(SceneToLoadName, TargetPosition);
            }
        }
    }
}

