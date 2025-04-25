using UnityEngine;

namespace Farm.Inventory
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemDropShadow : MonoBehaviour
    {
        public SpriteRenderer itemSprite;
        private SpriteRenderer shadowSprite;

        private void Awake()
        {
            shadowSprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            shadowSprite.sprite = itemSprite.sprite;
            shadowSprite.color = new Color(0, 0, 0, 0.3f);
        }
    }
}

