using System;
using GameScreen.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreen.GameLogic
{
    public class GameController : MonoBehaviour
    {
        private const int MaxActiveBalls = 5;
        
        [SerializeField] private PlayField[] _playFields;
        [SerializeField] private LineButton[] _linesButtons;
        [SerializeField] private RiskLevelSelector _riskLevelSelector;
        [SerializeField] private BetInputController _betInputController;
        [SerializeField] private BallSpawner _ballSpawner;
        [SerializeField] private GameObject _lockedObject;
        [SerializeField] private TMP_Text _unlockValue;
        [SerializeField] private Button _playButton;
        [SerializeField] private BuyButton _buyButton;
        [SerializeField] private GameObject _notEnoughBalance;
        [SerializeField] private WinPopup _winPopupPrefab;
        [SerializeField] private Canvas _gameCanvas;
        [SerializeField] private Transform _winPopupSpawnPoint;
        [SerializeField] private float _bigWinThreshold = 50f;
        [SerializeField] private BigWin _bigWinDisplay;
        [SerializeField] private GameObject _gameElements;
        [SerializeField] private LowerPlane _lowerPlane;
        [SerializeField] private ScreenVisabilityHandler _screenVisabilityHandler;
        [SerializeField] private AudioSource _backgroundSound;
        [SerializeField] private AudioSource _scoreSound;
        [SerializeField] private Onboarding _onboarding;
        [SerializeField] private AudioControlButton _audioControlButton;

        private LineButton _currentSelectedButton;
        private PlayField _currentSelectedField;

        private bool _isBallInPlay;
        private int _currentBet;
        private bool _isWin;

        public event Action OnGamePlayed;
        public event Action<float, float> OnCoefficientWin;
        public event Action OnGameLost;
        public event Action<int, float, int> GamePlayed;
        
        public PlayField CurrentSelectedField => _currentSelectedField;

        private void OnEnable()
        {
            _onboarding.OnboardingComplete += EnableScreen;
            _onboarding.OnboardingComplete += _backgroundSound.Play;
            
            InitializeLineButtons();
            InitializeControllers();
            _lockedObject.SetActive(false);
            _isBallInPlay = false;
            
            foreach (var field in _playFields)
            {
                if (field != null && field.CoefficientHolderController != null)
                {
                    field.CoefficientHolderController.Initialize(_ballSpawner);
                    field.CoefficientHolderController.CoefficientInteracted += OnCoefficientInteracted;
                }
            }
        }

        private void OnDisable()
        {
            _onboarding.OnboardingComplete -= EnableScreen;
            _onboarding.OnboardingComplete -= _backgroundSound.Play;
            
            foreach (var button in _linesButtons)
            {
                button.LineButtonClicked -= OnLineButtonClicked;
            }

            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            _buyButton.ButtonClicked -= OnBuyButtonClicked;
            _riskLevelSelector.RiskSelected -= OnRiskLevelSelected;
            
            foreach (var field in _playFields)
            {
                if (field != null && field.CoefficientHolderController != null)
                {
                    field.CoefficientHolderController.CoefficientInteracted -= OnCoefficientInteracted;
                }
            }
        }

        private void Start()
        {
            _ballSpawner.Initialize(this);
            OnLineButtonClicked(_linesButtons[0]);
            _linesButtons[0].SetSelectedColor();
            
            if(_onboarding.isActiveAndEnabled)
                DisableScreen();
            
            UpdatePlayButtonState();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
            _gameElements.SetActive(false);
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
            _gameElements.SetActive(true);
            _audioControlButton.UpdateSprite();
        }
        
        private void SetLineButtonsInteractable(bool interactable)
        {
            foreach (var button in _linesButtons)
            {
                button.SetInteractable(interactable);
            }
        }
        
        private void UpdatePlayButtonState()
        {
            bool canPlay = _ballSpawner.ActiveBallsCount < MaxActiveBalls;
            _playButton.interactable = canPlay;
        }

        private void InitializeLineButtons()
        {
            foreach (var button in _linesButtons)
            {
                button.LineButtonClicked += OnLineButtonClicked;
            }
        }

        private void InitializeControllers()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _buyButton.ButtonClicked += OnBuyButtonClicked;
            _riskLevelSelector.RiskSelected += OnRiskLevelSelected;
        }

        private void OnLineButtonClicked(LineButton button)
        {
            if (_currentSelectedButton != null)
            {
                _currentSelectedButton.SetDefaultColor();
            }

            _currentSelectedButton = button;

            if (button.IsLocked)
            {
                HandleLockedButton(button);
                return;
            }

            HandleUnlockedButton(button);
        }

        private void HandleLockedButton(LineButton button)
        {
            ActivatePlayField(button.Level - 8);

            _lockedObject.SetActive(true);
            SetButtonPrice(button.UnlockPrice);
            _playButton.gameObject.SetActive(false);
            _buyButton.Enable(button.UnlockPrice);
        }

        private void SetButtonPrice(int price)
        {
            if (price >= 1000)
            {
                float thousands = price / 1000f;
                if (price % 1000 == 0)
                {
                    _unlockValue.text = $"{thousands:0}k";
                }
                else
                {
                    _unlockValue.text = $"{thousands:0.###}k";
                }
            }
            else
            {
                _unlockValue.text = price.ToString();
            }
        }

        private void HandleUnlockedButton(LineButton button)
        {
            _lockedObject.SetActive(false);
            _playButton.gameObject.SetActive(true);
            _buyButton.gameObject.SetActive(false);
            button.SetSelectedColor();

            ActivatePlayField(button.Level - 8);
            _currentSelectedField.CoefficientHolderController.UpdateRiskLevel(ConvertToGeneratorRiskLevel(_riskLevelSelector.CurrentRiskType));
        }

        private void ActivatePlayField(int index)
        {
            if (index < 0 || index >= _playFields.Length)
                return;

            foreach (var field in _playFields)
            {
                field.gameObject.SetActive(false);
            }

            _playFields[index].gameObject.SetActive(true);
            _currentSelectedField = _playFields[index];
        }

        private void OnPlayButtonClicked()
        {
            if (_currentSelectedField == null || _currentSelectedButton == null || _currentSelectedButton.IsLocked)
                return;

            _currentBet = _betInputController.CurrentBet;

            if (!PlayerBalanceController.HasEnoughBalance(_currentBet))
            {
                _notEnoughBalance.SetActive(true);
                return;
            }

            _notEnoughBalance.SetActive(false);
            PlayerBalanceController.DecreaseBalance(_currentBet);
            SpawnBall();
        }

        private void OnCoefficientInteracted(float coefficient, Color color)
        {
            _scoreSound.Play();
            
            float winAmount = _currentBet * coefficient;
            var roundToInt = Mathf.RoundToInt(winAmount);
            PlayerBalanceController.IncreaseBalance(roundToInt);
            
            GamePlayed?.Invoke(_currentBet, coefficient, roundToInt - _currentBet);

            if (_winPopupPrefab != null && _gameCanvas != null)
            {
                WinPopup popup = Instantiate(_winPopupPrefab, _gameCanvas.transform);
                popup.ShowPopup(_winPopupSpawnPoint.position, winAmount);
            }

            if (coefficient >= _bigWinThreshold && _bigWinDisplay != null)
            {
                _bigWinDisplay.ShowBigWin(winAmount);
            }
            OnCoefficientWin?.Invoke(coefficient, winAmount);
        }
        
        private void SpawnBall()
        {
            if (_currentSelectedField == null)
                return;
            
            if (_ballSpawner.ActiveBallsCount >= MaxActiveBalls)
                return;

            _isBallInPlay = true;
            _riskLevelSelector.SetInteractable(!_isBallInPlay);
            _lowerPlane.ToggleAllButtons(!_isBallInPlay);
            SetLineButtonsInteractable(false);
            _ballSpawner.SpawnBall(_currentSelectedField.HolePosition.position);
            
            UpdatePlayButtonState();
        }
        
        public void OnBallFinished()
        {
            _isBallInPlay = false;
            _riskLevelSelector.SetInteractable(!_isBallInPlay);
            _lowerPlane.ToggleAllButtons(!_isBallInPlay);
            SetLineButtonsInteractable(true);
            OnGamePlayed?.Invoke();
        
            if (!_isWin)
            {
                OnGameLost?.Invoke();
            }
            
            UpdatePlayButtonState();
        }

        private void OnRiskLevelSelected(RiskLevelSelector.RiskType riskType)
        {
            if (_currentSelectedField != null)
            {
                var riskLevel = ConvertToGeneratorRiskLevel(riskType);
                _currentSelectedField.CoefficientHolderController.UpdateRiskLevel(riskLevel);
            }
        }

        private CoefficientGenerator.RiskLevel ConvertToGeneratorRiskLevel(RiskLevelSelector.RiskType riskType)
        {
            return riskType switch
            {
                RiskLevelSelector.RiskType.Easy => CoefficientGenerator.RiskLevel.Low,
                RiskLevelSelector.RiskType.Normal => CoefficientGenerator.RiskLevel.Medium,
                RiskLevelSelector.RiskType.Hard => CoefficientGenerator.RiskLevel.High,
                _ => CoefficientGenerator.RiskLevel.Medium
            };
        }

        private void OnBuyButtonClicked()
        {
            if (_currentSelectedButton == null || !_currentSelectedButton.IsLocked)
                return;

            int unlockPrice = _currentSelectedButton.UnlockPrice;

            if (!PlayerBalanceController.HasEnoughBalance(unlockPrice))
            {
                _notEnoughBalance.SetActive(true);
                return;
            }

            PlayerBalanceController.DecreaseBalance(unlockPrice);
            _currentSelectedButton.Unlock();

            HandleUnlockedButton(_currentSelectedButton);
        }
    }
}