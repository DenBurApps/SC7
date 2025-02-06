using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Achievemnts
{
    public class AchievementPopup : MonoBehaviour
    {
        [SerializeField] private Image _achievementIcon;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showDuration = 3f;
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private AudioSource _achievementSound;

        private void OnDisable()
        {
            DOTween.Kill(transform);
            DOTween.Kill(_canvasGroup);
        }

        public void ShowAchievement(Sprite icon = null)
        {
            if (icon != null)
            {
                _achievementIcon.sprite = icon;
            }
        
            _canvasGroup.alpha = 0f;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            
            _achievementSound.Play();
        
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(transform.DOScale(1f, _fadeDuration).SetEase(Ease.OutBack))
                .Join(_canvasGroup.DOFade(1f, _fadeDuration))
                .AppendInterval(_showDuration)
                .Append(_canvasGroup.DOFade(0f, _fadeDuration))
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
