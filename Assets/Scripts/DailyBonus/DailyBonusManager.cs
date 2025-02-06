using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ShopScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DailyBonus
{
    [System.Serializable]
    public class RewardTier
    {
        public float RequiredFillAmount;
        public int Reward;
        public bool IsSkinReward;
    }

    public class DailyBonusManager : MonoBehaviour
    {
        [SerializeField] private Image _progressMeter;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private CircleController _circlesPrefab;
        [SerializeField] private GameObject _rewardPanel;
        [SerializeField] private TMP_Text _rewardAmountText;
        [SerializeField] private RectTransform _gameArea;
        [SerializeField] private float _fillAmountPerPop = 0.02f;

        [SerializeField] private float _gameDuration = 15f;
        [SerializeField] private float _spawnInterval = 0.5f;
        [SerializeField] private int _maxCirclesAtOnce = 50;
        [SerializeField] private RewardTier[] _rewardTiers;
        [SerializeField] private GameObject _ballPrice;
        [SerializeField] private GameObject _coinPrice;
        [SerializeField] private ShopScreenController _shopScreenController;
        [SerializeField] private AudioSource _achievementSound;

        private float _currentTime;
        private float _currentFillAmount;
        private bool _isGameActive;
        private readonly List<CircleController> _activeCircles = new List<CircleController>();

        public event Action GiftCollected;

        private void Start()
        {
            InitializeGame();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            foreach (var circle in _activeCircles)
            {
                if (circle != null)
                {
                    circle.OnCirclePopped -= OnCirclePopped;
                }
            }
        }

        private void InitializeGame()
        {
            _currentTime = _gameDuration;
            _currentFillAmount = 0f;
            _progressMeter.fillAmount = 0f;
            _isGameActive = true;
            _rewardPanel.SetActive(false);
            StartCoroutine(SpawnCirclesRoutine());
            UpdateTimerDisplay();
        }

        private void Update()
        {
            if (!_isGameActive) return;

            _currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (_currentTime <= 0)
            {
                EndGame();
            }
        }

        private void UpdateTimerDisplay()
        {
            float timeToDisplay = Mathf.Max(0, _currentTime);
            _timerText.text = Mathf.Ceil(timeToDisplay).ToString();
        }

        private IEnumerator SpawnCirclesRoutine()
        {
            while (_isGameActive)
            {
                if (_activeCircles.Count < _maxCirclesAtOnce)
                {
                    SpawnCircle();
                }

                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        private void SpawnCircle()
        {
            Vector2 randomPosition = GetRandomPositionInGameArea();
            CircleController circleObj = Instantiate(_circlesPrefab, _gameArea);
            circleObj.GetComponent<RectTransform>().anchoredPosition = randomPosition;

            circleObj.Initialize();
            circleObj.OnCirclePopped += OnCirclePopped;
            _activeCircles.Add(circleObj);
        }

        private Vector2 GetRandomPositionInGameArea()
        {
            float circleRadius = _circlesPrefab.GetComponent<RectTransform>().rect.width / 2;
            float paddingX = 50f + circleRadius;
            float paddingY = 50f + circleRadius;

            float randomX = Random.Range(-_gameArea.rect.width / 2 + paddingX, _gameArea.rect.width / 2 - paddingX);
            float randomY = Random.Range(-_gameArea.rect.height / 2 + paddingY, _gameArea.rect.height / 2 - paddingY);

            return new Vector2(randomX, randomY);
        }

        private void OnCirclePopped(CircleController circle)
        {
            if (_activeCircles.Contains(circle))
            {
                _activeCircles.Remove(circle);
                _currentFillAmount += _fillAmountPerPop;
                _currentFillAmount = Mathf.Clamp01(_currentFillAmount);
                _progressMeter.DOFillAmount(_currentFillAmount, 0.2f).SetEase(Ease.OutQuad);
            }
        }

        private void EndGame()
        {
            _isGameActive = false;
            StopAllCoroutines();

            foreach (var circle in _activeCircles)
            {
                if (circle != null)
                {
                    circle.OnCirclePopped -= OnCirclePopped;
                    Destroy(circle.gameObject);
                }
            }

            _activeCircles.Clear();

            RewardTier achievedTier = GetAchievedRewardTier();
            ShowRewardPanel(achievedTier);
        }

        private RewardTier GetAchievedRewardTier()
        {
            RewardTier highestAchievedTier = _rewardTiers[0];

            for (int i = _rewardTiers.Length - 1; i >= 0; i--)
            {
                if (_currentFillAmount >= _rewardTiers[i].RequiredFillAmount)
                {
                    highestAchievedTier = _rewardTiers[i];
                    break;
                }
            }

            return highestAchievedTier;
        }

        private void ShowRewardPanel(RewardTier achievedTier)
        {
            _rewardPanel.SetActive(true);
            _achievementSound.Play();

            if (achievedTier.IsSkinReward)
            {
               _shopScreenController.UnlockFootballSkin();
                _ballPrice.SetActive(true);
                _coinPrice.SetActive(false);
            }
            else
            {
                _ballPrice.SetActive(false);
                _coinPrice.SetActive(true);
                _rewardAmountText.text = $"+{achievedTier.Reward}";
                PlayerBalanceController.IncreaseBalance(achievedTier.Reward);
            }

            _rewardPanel.transform.localScale = Vector3.zero;
            _rewardPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }

        public void OnCollectButtonPressed()
        {
            GiftCollected?.Invoke();
            gameObject.SetActive(false);
        }
    }
}