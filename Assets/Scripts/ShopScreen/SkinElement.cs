using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ShopScreen
{
    public class SkinElement : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        internal Image _selectedFrame;

        [SerializeField] internal Image _selectedToggle;
        [SerializeField] internal Button _openButton;
        [SerializeField] internal Button _buttonButton;
        [SerializeField] internal Image _priceImage;
        [SerializeField] private Image _skinImage;

        [field: SerializeField] public Sprite SkinSprite { get; private set; }

        [field: SerializeField] public int OpenPrice { get; private set; }

        public event Action<SkinElement> OnSkinClicked;
        public Action<SkinElement> OnUnlockAttempted;

        private bool _isLocked;

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                _isLocked = value;
                UpdateUIState();
            }
        }

        private void Awake()
        {
            InitializeUI();
        }

        private void OnEnable()
        {
            _openButton.onClick.AddListener(HandleOpenButtonClick);
            _buttonButton.onClick.AddListener(HandleSelectButtonClick);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(HandleOpenButtonClick);
            _buttonButton.onClick.RemoveListener(HandleSelectButtonClick);
        }

        private void InitializeUI()
        {
            UpdateUIState();
        }

        private void UpdateUIState()
        {
            _openButton.gameObject.SetActive(_isLocked);
            _buttonButton.enabled = !_isLocked;
            _priceImage.gameObject.SetActive(_isLocked);
        }

        public void SetSelected(bool selected)
        {
            if (_selectedFrame != null)
            {
                _selectedFrame.gameObject.SetActive(selected);
            }

            if (_selectedToggle != null)
            {
                _selectedToggle.gameObject.SetActive(selected);
            }
        }

        private void HandleOpenButtonClick()
        {
            OnUnlockAttempted?.Invoke(this);
        }

        private void HandleSelectButtonClick()
        {
            OnSkinClicked?.Invoke(this);
        }

        public void PlayUnlockAnimation()
        {
            Sequence unlockSequence = DOTween.Sequence();

            Vector3 originalPriceScale = _priceImage.transform.localScale;
            Vector3 originalButtonScale = _openButton.transform.localScale;

            unlockSequence.Append(_priceImage.transform.DOScale(originalPriceScale * 1.2f, 0.3f))
                .Join(_openButton.transform.DOScale(originalButtonScale * 1.2f, 0.3f))
                .AppendInterval(0.1f)
                .Append(_priceImage.transform.DOScale(Vector3.zero, 0.4f))
                .Join(_openButton.transform.DOScale(Vector3.zero, 0.4f));

            unlockSequence.Join(transform.DOScale(1.1f, 0.2f))
                .Join(_skinImage.DOColor(Color.white * 1.5f, 0.2f))
                .AppendInterval(0.1f)
                .Append(transform.DOScale(1f, 0.3f))
                .Join(_skinImage.DOColor(Color.white, 0.3f));

            unlockSequence.Join(_skinImage.transform.DORotate(new Vector3(0, 0, 360), 0.7f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutBack));
            unlockSequence.AppendCallback(() => _buttonButton.gameObject.SetActive(true));
            unlockSequence.Append(_buttonButton.transform.DOScale(0, 0))
                .Append(_buttonButton.transform.DOScale(1.2f, 0.3f))
                .Append(_buttonButton.transform.DOScale(1f, 0.2f))
                .SetEase(Ease.OutBack);

            unlockSequence.OnComplete(() => { });

            unlockSequence.Play();
        }
    }
}