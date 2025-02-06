using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using GameScreen.GameLogic;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace StatisticsScreen
{
    public class StatisticsManager : MonoBehaviour
    {
        private const string DailySaveFile = "daily_stats.json";
        private const string HighestSaveFile = "highest_stats.json";
        private const int MaxHighestRecords = 20;

        [SerializeField] private Transform _dailyPlanesContainer;
        [SerializeField] private Transform _highestPlanesContainer;
        [SerializeField] private GameObject _daily;
        [SerializeField] private GameObject _highest;
        [SerializeField] private DailyPlane _dailyPlanePrefab;
        [SerializeField] private HighestPlane _highestPlanePrefab;
        [SerializeField] private Button _dailyButton;
        [SerializeField] private Button _highestButton;
        [SerializeField] private TMP_Text _dailyButtonText;
        [SerializeField] private TMP_Text _highestButtonText;
        [SerializeField] private Color _selectedButtonColor;
        [SerializeField] private Color _unselectedButtonColor;
        [SerializeField] private Color _selectedTextColor;
        [SerializeField] private Color _unselectedTextColor;
        [SerializeField] private GameController _gameController;
        [SerializeField] private AudioControlButton _audioControlButton;

        private List<DailyPlane> _dailyPlanes = new List<DailyPlane>();
        private List<HighestPlane> _highestPlanes = new List<HighestPlane>();
        private DateTime _currentDate;
        private ScreenVisabilityHandler _screenVisabilityHandler;
        private string SavePath => Path.Combine(Application.persistentDataPath, "Statistics");

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            Directory.CreateDirectory(SavePath);
            _currentDate = DateTime.Now.Date;
            InvokeRepeating(nameof(CheckDateChange), 60f, 60f);
        }

        private void Start()
        {
            SetupButtons();
            LoadData();
            ShowDailyView();
            DisableScreen();
        }

        private void OnEnable()
        {
            _gameController.GamePlayed += HandleGameCompleted;
        }

        private void OnDisable()
        {
            _gameController.GamePlayed -= HandleGameCompleted;
            CancelInvoke();
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
            _audioControlButton.UpdateSprite();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void CheckDateChange()
        {
            var now = DateTime.Now.Date;
            if (now != _currentDate)
            {
                ClearDailyData();
                _currentDate = now;
            }
        }

        private void SetupButtons()
        {
            if (_dailyButton != null) _dailyButton.onClick.AddListener(ShowDailyView);
            if (_highestButton != null) _highestButton.onClick.AddListener(ShowHighestView);
        }

        private void HandleGameCompleted(int bet, float payout, int profit)
        {
            var now = DateTime.Now;
            var dailyData = new DailyStatisticsData(now, bet, payout, profit);
            CreateDailyPlane(dailyData);
            SaveDailyData();

            if (profit <= 0) return;
            
            bool isDuplicate = _highestPlanes.Any(p =>
                p.CurrentProfit == profit
            );

            if (isDuplicate) return;

            if (_highestPlanes.Count < MaxHighestRecords || profit > GetLowestHighestProfit())
            {
                var highestData = new HighestData(now, bet, payout, profit);
                CreateHighestPlane(highestData);
                MaintainTopHighest();
                SaveHighestData();
            }
        }

        private void ShowDailyView()
        {
            _daily.gameObject.SetActive(true);
            _dailyPlanesContainer.gameObject.SetActive(true);
            _highest.SetActive(false);
            _highestPlanesContainer.gameObject.SetActive(false);
            UpdateButtonColors(_dailyButton, _highestButton);
        }

        private void ShowHighestView()
        {
            _dailyPlanesContainer.gameObject.SetActive(false);
            _daily.SetActive(false);
            _highestPlanesContainer.gameObject.SetActive(true);
            _highest.SetActive(true);
            UpdateButtonColors(_highestButton, _dailyButton);
            SortHighestPlanes();
        }

        private void UpdateButtonColors(Button selectedButton, Button unselectedButton)
        {
            selectedButton.image.color = _selectedButtonColor;
            unselectedButton.image.color = _unselectedButtonColor;
            _dailyButtonText.color = selectedButton == _dailyButton ? _selectedTextColor : _unselectedTextColor;
            _highestButtonText.color = selectedButton == _highestButton ? _selectedTextColor : _unselectedTextColor;
        }

        private void SortHighestPlanes()
        {
            var sortedPlanes = _highestPlanes.OrderByDescending(p => p.CurrentProfit).ToList();
            for (int i = 0; i < sortedPlanes.Count; i++)
            {
                sortedPlanes[i].transform.SetSiblingIndex(i);
            }
        }

        private void CreateDailyPlane(DailyStatisticsData data)
        {
            if (_dailyPlanePrefab == null || _dailyPlanesContainer == null) return;

            var plane = Instantiate(_dailyPlanePrefab, _dailyPlanesContainer);
            plane.Initialize(data);
            _dailyPlanes.Add(plane);
        }

        private void CreateHighestPlane(HighestData data)
        {
            if (_highestPlanePrefab == null || _highestPlanesContainer == null) return;

            var plane = Instantiate(_highestPlanePrefab, _highestPlanesContainer);
            plane.Initialize(data);
            _highestPlanes.Add(plane);
        }

        private int GetLowestHighestProfit() =>
            _highestPlanes.Count > 0 ? _highestPlanes.Min(p => p.CurrentProfit) : int.MinValue;

        private void MaintainTopHighest()
        {
            if (_highestPlanes.Count <= MaxHighestRecords) return;

            var lowestPlane = _highestPlanes
                .OrderBy(p => p.CurrentProfit)
                .First();

            _highestPlanes.Remove(lowestPlane);
            Destroy(lowestPlane.gameObject);
        }

        private void ClearDailyData()
        {
            foreach (var plane in _dailyPlanes)
            {
                if (plane != null && plane.gameObject != null)
                    Destroy(plane.gameObject);
            }

            _dailyPlanes.Clear();
            SaveDailyData();
        }

        private void SaveDailyData() =>
            SaveToJson(DailySaveFile, _dailyPlanes.Select(p => p.GetData()).ToList());

        private void SaveHighestData() =>
            SaveToJson(HighestSaveFile, _highestPlanes.Select(p => p.GetData()).ToList());

        private void LoadData()
        {
            LoadDailyData();
            LoadHighestData();
        }

        private void LoadDailyData()
        {
            var data = LoadFromJson<List<DailyStatisticsData>>(DailySaveFile);
            if (data == null) return;

            foreach (var gameData in data)
            {
                if (gameData.Time.Date == DateTime.Now.Date)
                    CreateDailyPlane(gameData);
            }
        }

        private void LoadHighestData()
        {
            var data = LoadFromJson<List<HighestData>>(HighestSaveFile);
            if (data == null) return;

            foreach (var gameData in data)
                CreateHighestPlane(gameData);

            SortHighestPlanes();
        }

        private void SaveToJson<T>(string fileName, T data)
        {
            try
            {
                var path = Path.Combine(SavePath, fileName);
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving {fileName}: {e}");
            }
        }

        private T LoadFromJson<T>(string fileName)
        {
            try
            {
                var path = Path.Combine(SavePath, fileName);
                if (!File.Exists(path)) return default;

                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading {fileName}: {e}");
                return default;
            }
        }
    }
}