using UnityEngine;

namespace Farm.Inventory
{
    public class ItemDropBounce : MonoBehaviour
    {
        private Transform _spriteTransform;
        private Collider2D _collider;

        public float gravity = -3.5f;
        private bool _isGround;
        private float _distance;
        private Vector2 _direction;
        private Vector3 _targetPosition;

        private void Awake()
        {
            _spriteTransform = transform.GetChild(0);
            _collider = GetComponent<Collider2D>();

            _collider.enabled = false;
        }

        private void Update()
        {
            Bounce();
        }

        public void InitBounceItem(Vector3 target, Vector2 dir)
        {
            _collider.enabled = false;
            _direction = dir;
            _targetPosition = target;

            _distance = Vector3.Distance(target, transform.position);
            _spriteTransform.position += Vector3.up * 1.5f;
        }

        private void Bounce()
        {
            _isGround = _spriteTransform.position.y <= transform.position.y;

            if (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
            {
                transform.position += _distance * (-gravity) * Time.deltaTime * (Vector3)_direction;
            }

            if (!_isGround)
            {
                _spriteTransform.position += gravity * Time.deltaTime * Vector3.up;
            }
            else
            {
                _spriteTransform.position = transform.position;
                _collider.enabled = true;
            }
        }
    }
}

