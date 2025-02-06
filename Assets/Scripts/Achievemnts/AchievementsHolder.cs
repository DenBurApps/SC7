using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening.Plugins.Core.PathCore;
using GameScreen.GameLogic;
using UnityEngine;
using Path = System.IO.Path;

namespace Achievemnts
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AchievementsHolder : MonoBehaviour
    {
        private const int SameCoefficientThreshold = 5;
        private const int GamesPlayedStartID = 1;
        private const int WinAmountsStartID = 7;
        private const int WinStreakStartID = 15;
        private const int SameCoefficientID = 13;
        private const int HighestCoefficientID = 14;
        private const string SaveKey = "AchievementsSaveData.json";

        [SerializeField] private List<Achievement> _achievements;
        [SerializeField] private int[] _winsThresholds;
        [SerializeField] private int[] _gamesPlayedThresholds = { 1, 10, 100, 500, 1000, 5000 };
        [SerializeField] private int[] _winStreakThresholds = { 3, 5, 10 };
        [SerializeField] private GameController _gameController;
        [SerializeField] private AchievementPopup _achievementPopup;
        [SerializeField] private LowerPlane _lowerPlane;
        [SerializeField] private AudioControlButton _audioControlButton;

        private int _gamesCounter;
        private bool _winStreakStart = false;
        private int _winStreakCount;
        private float _winCoefficient;
        private float _lastWinCoefficient;
        private int _sameCoefficientHitCount;
        private string _savePath;

        private AchievementSaveData _saveData;
        private ScreenVisabilityHandler _screenVisabilityHandler;

        private PlayField _currentSelectedField => _gameController.CurrentSelectedField;
        
        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _savePath = Path.Combine(Application.persistentDataPath, SaveKey);
        }

        private void OnEnable()
        {
            SubscribeToGameEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromGameEvents();
            SaveProgress();
        }

        private void Start()
        {
            LoadProgress();
            DisableScreen();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
            _audioControlButton.UpdateSprite();
        }
        
        private void ResetAllProgress()
        {
            _gamesCounter = 0;
            _winStreakCount = 0;
            _winStreakStart = false;
            _lastWinCoefficient = 0f;
            _sameCoefficientHitCount = 0;

            _saveData = new AchievementSaveData();

            foreach (var achievement in _achievements)
            {
                achievement.Reset();
            }

            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
        }

        private void LoadProgress()
        {
            if (!File.Exists(_savePath))
            {
                ResetAllProgress();
                return;
            }

            try
            {
                string jsonData = File.ReadAllText(_savePath);
                _saveData = JsonUtility.FromJson<AchievementSaveData>(jsonData);
                RestoreFromSaveData();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading achievement data: {e.Message}");
                ResetAllProgress();
            }
        }

        private void RestoreFromSaveData()
        {
            _gamesCounter = _saveData.GamesCounter;

            foreach (int id in _saveData.UnlockedAchievementIds)
            {
                UnlockAchievement(id, true);
            }
        }

        private void SaveProgress()
        {
            try
            {
                UpdateSaveData();
                string jsonData = JsonUtility.ToJson(_saveData);
                File.WriteAllText(_savePath, jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving achievement data: {e.Message}");
            }
        }

        private void UpdateSaveData()
        {
            _saveData.GamesCounter = _gamesCounter;
        }

        private void SubscribeToGameEvents()
        {
            _gameController.OnGamePlayed += HandleGamePlayed;
            _gameController.OnCoefficientWin += HandleCoefficientWin;
            _gameController.OnGameLost += HandleGameLost;
        }

        private void UnsubscribeFromGameEvents()
        {
            _gameController.OnGamePlayed -= HandleGamePlayed;
            _gameController.OnCoefficientWin -= HandleCoefficientWin;
            _gameController.OnGameLost -= HandleGameLost;
        }

        private void HandleGamePlayed()
        {
            _gamesCounter++;
            CheckGamesPlayedAchievements();
            SaveProgress();
        }

        private void HandleGameLost()
        {
            ResetWinStreak();
        }

        private void HandleCoefficientWin(float coefficient, float winAmount)
        {
            if (coefficient > 1f)
            {
                CheckWinAmountAchievements(winAmount);
                HandleWinStreak();
                CheckSameCoefficientAchievement(coefficient);
                CheckHighestCoefficientAchievement(coefficient);
                SaveProgress();
            }
            else
            {
                HandleGameLost();
            }
        }

        private void CheckGamesPlayedAchievements()
        {
            for (int i = 0; i < _gamesPlayedThresholds.Length; i++)
            {
                if (_gamesCounter == _gamesPlayedThresholds[i])
                {
                    UnlockAchievement(GamesPlayedStartID + i);
                }
            }
        }

        private void CheckWinAmountAchievements(float winAmount)
        {
            for (int i = 0; i < _winsThresholds.Length; i++)
            {
                if (winAmount >= _winsThresholds[i])
                {
                    UnlockAchievement(WinAmountsStartID + i);
                }
            }
        }

        private void CheckHighestCoefficientAchievement(float hitCoefficient)
        {
            if (_currentSelectedField != null &&
                _currentSelectedField.CoefficientHolderController != null)
            {
                float highestPossibleCoefficient =
                    _currentSelectedField.CoefficientHolderController.GetHighestPossibleCoefficient();
                if (Math.Abs(hitCoefficient - highestPossibleCoefficient) < 0.001f)
                {
                    UnlockAchievement(HighestCoefficientID);
                }
            }
        }

        private void HandleWinStreak()
        {
            if (!_winStreakStart)
            {
                _winStreakStart = true;
                _winStreakCount = 1;
            }
            else
            {
                _winStreakCount++;
                CheckWinStreakAchievements();
            }
        }

        private void CheckWinStreakAchievements()
        {
            for (int i = 0; i < _winStreakThresholds.Length; i++)
            {
                if (_winStreakCount == _winStreakThresholds[i])
                {
                    UnlockAchievement(WinStreakStartID + i);
                }
            }
        }

        private void CheckSameCoefficientAchievement(float coefficient)
        {
            if (Math.Abs(_lastWinCoefficient - coefficient) < 0.001f)
            {
                _sameCoefficientHitCount++;
                if (_sameCoefficientHitCount == SameCoefficientThreshold)
                {
                    UnlockAchievement(SameCoefficientID);
                }
            }
            else
            {
                _sameCoefficientHitCount = 1;
            }

            _lastWinCoefficient = coefficient;
        }

        private void ResetWinStreak()
        {
            _winStreakStart = false;
            _winStreakCount = 0;
            SaveProgress();
        }

        private void UnlockAchievement(int id, bool isLoading = false)
        {
            Achievement achievement = _achievements.Find(a => a.Id == id);
            if (achievement != null && achievement.IsLocked)
            {
                achievement.Unlock();

                if (!isLoading && _achievementPopup != null)
                {
                    ShowAchievementPopup(achievement);
                }

                if (!_saveData.UnlockedAchievementIds.Contains(id))
                {
                    _saveData.UnlockedAchievementIds.Add(id);
                    SaveProgress();
                }
            }
        }

        private void ShowAchievementPopup(Achievement achievement)
        {
            _achievementPopup.ShowAchievement(achievement.ImageSprite);
        }
    }

    [Serializable]
    public class AchievementSaveData
    {
        public int GamesCounter;
        public List<int> UnlockedAchievementIds = new List<int>();
    }
}