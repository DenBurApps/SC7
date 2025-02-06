using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DailyBonus
{
    public class CircleController : MonoBehaviour
    {
        [SerializeField] private float _popDuration = 0.5f;
        [SerializeField] private float _maxScale = 1.5f;
        [SerializeField] private Button _circleButton;

        private bool _isPopped;
        private System.Action<float> _onPopCallback;
        private RectTransform _rectTransform;

        public event System.Action<CircleController> OnCirclePopped;

        private void Awake()
        {
            _circleButton.onClick.AddListener(OnCircleClicked);
        }

        public void Initialize()
        {
            _rectTransform = GetComponent<RectTransform>();
        
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        private void OnEnable()
        {
            StartCoroutine(AutoDestroyRoutine());
            _circleButton.interactable = true;
        }

        private void OnDisable()
        {
            if (_circleButton != null)
                _circleButton.onClick.RemoveListener(OnCircleClicked);
        }

        private IEnumerator AutoDestroyRoutine()
        {
            yield return new WaitForSeconds(3f);
            if (!_isPopped)
            {
                Destroy(gameObject);
            }
        }

        private void OnCircleClicked()
        {
            OnPointerDown();
        }

        private void OnPointerDown()
        {
            if (_isPopped) return;
        
            _isPopped = true;
            PopAnimation();
            OnCirclePopped?.Invoke(this);
            _circleButton.interactable = false;
        }

        private void PopAnimation()
        {
            transform.DOScale(_maxScale, _popDuration/2)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    transform.DOScale(0f, _popDuration/2)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() => {
                            Destroy(gameObject);
                        });
                });
        }
    }
}