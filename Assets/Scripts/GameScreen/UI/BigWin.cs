using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreen.UI
{
    public class BigWin : MonoBehaviour
    {
        [SerializeField] private TMP_Text _winAmountText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _scaleAnimDuration = 0.5f;
        [SerializeField] private AudioSource _bigWinSound;

        private void Awake()
        {
            if (_winAmountText != null)
                _winAmountText.alpha = 0f;
            if (_backgroundImage != null)
                _backgroundImage.color = new Color(1f, 1f, 1f, 0f);

            transform.localScale = Vector3.zero;
        }

        public void ShowBigWin(float winAmount)
        {
            gameObject.SetActive(true);
            
            _bigWinSound.Play();

            transform.localScale = Vector3.zero;
            if (_winAmountText != null)
                _winAmountText.alpha = 0f;
            if (_backgroundImage != null)
                _backgroundImage.color = new Color(1f, 1f, 1f, 0f);

            _winAmountText.text = $"+{winAmount}";

            Sequence sequence = DOTween.Sequence();

            if (_backgroundImage != null)
            {
                sequence.Join(_backgroundImage.DOFade(1f, _fadeInDuration));
            }

            sequence.Join(transform.DOScale(1.2f, _scaleAnimDuration).SetEase(Ease.OutBack));
            sequence.Append(transform.DOScale(1f, _scaleAnimDuration * 0.5f));

            sequence.Join(_winAmountText.DOFade(1f, _fadeInDuration));

            sequence.AppendInterval(_displayDuration);

            sequence.Append(_winAmountText.DOFade(0f, _fadeInDuration));
            if (_backgroundImage != null)
            {
                sequence.Join(_backgroundImage.DOFade(0f, _fadeInDuration));
            }

            sequence.Join(transform.DOScale(0f, _fadeInDuration).SetEase(Ease.InBack));

            sequence.OnComplete(() => gameObject.SetActive(false));
        }
    }
}