using UnityEngine;

namespace GameScreen.GameLogic
{
    public class PlinkoCollider : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _glowingSprite;
        [SerializeField] private Sprite _defaultSprite;

        private void Start()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _spriteRenderer.sprite = _defaultSprite;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _spriteRenderer.sprite = _glowingSprite;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            _spriteRenderer.sprite = _defaultSprite;
        }
    }
}
