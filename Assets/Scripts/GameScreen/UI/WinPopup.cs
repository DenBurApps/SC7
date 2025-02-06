using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameScreen.UI
{
    public class WinPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _popupText;
        [SerializeField] private float _moveDistance = 2f;
        [SerializeField] private float _duration = 2f;

        public void ShowPopup(Vector3 startPosition, float winAmount)
        {
            transform.position = startPosition;
            _popupText.text = $"+{winAmount:N2}";
            _popupText.alpha = 1f;
            transform.localScale = Vector3.one;
            
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));
            sequence.Append(transform.DOScale(1f, 0.1f));
         
            sequence.Join(transform.DOMoveY(startPosition.y + _moveDistance, _duration)
                .SetEase(Ease.OutQuad));
            
            float fadeDuration = _duration * 0.5f;
            sequence.Insert(_duration - fadeDuration, _popupText.DOFade(0f, fadeDuration).SetEase(Ease.InQuad));
            sequence.Insert(_duration - fadeDuration, transform.DOScale(0.8f, fadeDuration).SetEase(Ease.InQuad));
            
            sequence.OnComplete(() => Destroy(gameObject));
        }
    }
}