using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

namespace Farm.Dialog
{
    [System.Serializable]
    public class DialogPiece
    {
        [Header("dialog")]
        public Sprite faceImage;
        public bool isOnLeft;
        public string name;

        [TextArea]
        public string dialogText;
        public bool hasToPause;
        public bool isDone;

        // public UnityEvent afterDialogEvent;
    }
}

