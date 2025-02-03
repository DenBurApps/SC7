using System;
using UnityEngine;

namespace GameScreen.GameLogic
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private float _startXPosition = 0f;
        [SerializeField] private float _startYPosition = 10f;
        [SerializeField] private float _maxXBounds = 5f;
        [SerializeField] private float _bounceForce = 5f;
        [SerializeField] private float _maximumVelocity = 10f;
        [SerializeField] private float _minimumVelocity = 1f;
        [SerializeField] private float _downwardForce = 1f;

        private Rigidbody2D _rb;
        private bool _isDropped = false;
        private Vector3 _initialPosition;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            _rb.gravityScale = 0.8f;
            _rb.mass = 1f;
            _rb.drag = 0.1f;
            _rb.angularDrag = 0.05f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.sharedMaterial = CreateBouncyMaterial();

            _initialPosition = new Vector3(_startXPosition, _startYPosition, 0f);
            ResetBall();
        }

        private void Start()
        {
            DropBall();
        }

        private PhysicsMaterial2D CreateBouncyMaterial()
        {
            PhysicsMaterial2D material = new PhysicsMaterial2D("PlinkoMaterial");
            material.friction = 0.4f;
            material.bounciness = 0.5f;
            return material;
        }

        private void FixedUpdate()
        {
            if (!_isDropped) return;

            if (_rb.velocity.magnitude < _minimumVelocity)
            {
                Vector2 currentVelocity = _rb.velocity.normalized * _minimumVelocity;
                _rb.velocity = currentVelocity;
            }

            _rb.AddForce(Vector2.down * _downwardForce);

            if (_rb.velocity.magnitude > _maximumVelocity)
            {
                _rb.velocity = _rb.velocity.normalized * _maximumVelocity;
            }

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -_maxXBounds, _maxXBounds);
            transform.position = pos;
        }


        public void DropBall()
        {
            if (!_isDropped)
            {
                _isDropped = true;
                _rb.bodyType = RigidbodyType2D.Dynamic;
                float randomX = UnityEngine.Random.Range(-0.5f, 0.5f);
                _rb.AddForce(new Vector2(randomX, 0), ForceMode2D.Impulse);
            }
        }

        public void ResetBall()
        {
            transform.position = _initialPosition;
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _isDropped = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.TryGetComponent(out PlinkoCollider plinkoCollider))
            {
                Vector2 bounceDirection = (transform.position - collision.transform.position).normalized;
                bounceDirection += new Vector2(UnityEngine.Random.Range(-0.1f, 0.1f), 0);
                bounceDirection.Normalize();
                
                _rb.velocity = Vector2.zero;
                _rb.AddForce(bounceDirection * _bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}