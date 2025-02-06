using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreen
{
    public class RiskLevelSelector : MonoBehaviour
    {
        public enum RiskType
        {
            Easy,
            Normal,
            Hard
        }

        [SerializeField] private Color _defaultColor, _easyTextColor, _normalTextColor, _hardTextColor;
        [SerializeField] private Sprite _defaultSprite, _easySelectedSprite, _normalSelectedSprite, _hardSelectedSprite;
        [SerializeField] private TMP_Text _easyText, _normalText, _hardText;
        [SerializeField] private Button _easyButton, _normalButton, _hardButton;
        [SerializeField] private Image _easyImage, _normalImage, _hardImage;

        private bool _isInteractable = true;
        private RiskType _currentRiskType;

        public event Action<RiskType> RiskSelected;

        public RiskType CurrentRiskType => _currentRiskType;

        private void OnEnable()
        {
            _easyButton.onClick.AddListener(() => OnRiskButtonClicked(RiskType.Easy));
            _normalButton.onClick.AddListener(() => OnRiskButtonClicked(RiskType.Normal));
            _hardButton.onClick.AddListener(() => OnRiskButtonClicked(RiskType.Hard));
            OnRiskButtonClicked(RiskType.Easy);
        }

        private void OnDisable()
        {
            _easyButton.onClick.RemoveAllListeners();
            _normalButton.onClick.RemoveAllListeners();
            _hardButton.onClick.RemoveAllListeners();
        }

        public void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;
            _easyButton.interactable = interactable;
            _normalButton.interactable = interactable;
            _hardButton.interactable = interactable;

            if (!interactable)
            {
                ResetAllButtonsToDefault();
            }
            else
            {
                ApplyRiskTypeState(_currentRiskType);
            }
        }

        private void ApplyRiskTypeState(RiskType riskType)
        {
            ResetAllButtonsToDefault();

            switch (riskType)
            {
                case RiskType.Easy:
                    UpdateButtonState(_easyImage, _easyText, _easySelectedSprite, _easyTextColor);
                    break;
                case RiskType.Normal:
                    UpdateButtonState(_normalImage, _normalText, _normalSelectedSprite, _normalTextColor);
                    break;
                case RiskType.Hard:
                    UpdateButtonState(_hardImage, _hardText, _hardSelectedSprite, _hardTextColor);
                    break;
            }
        }

        private void OnRiskButtonClicked(RiskType riskType)
        {
            if (!_isInteractable) return;

            _currentRiskType = riskType;
            ApplyRiskTypeState(riskType);
            RiskSelected?.Invoke(riskType);
        }

        private void UpdateButtonState(Image image, TMP_Text text, Sprite sprite, Color textColor)
        {
            image.sprite = sprite;
            text.color = textColor;
        }

        private void ResetAllButtonsToDefault()
        {
            _easyImage.sprite = _defaultSprite;
            _normalImage.sprite = _defaultSprite;
            _hardImage.sprite = _defaultSprite;

            _easyText.color = _defaultColor;
            _normalText.color = _defaultColor;
            _hardText.color = _defaultColor;
        }
    }
}