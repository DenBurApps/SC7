using System;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace DailyBonus
{
    public class DailyGiftSaver : MonoBehaviour
    {
        [SerializeField] private Button _openGiftButton;
        [SerializeField] private DailyBonusManager _dailyBonusManager;

        private string _savePath;
        private bool _isGiftCollected;

        private void Awake()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "GiftSave.json");
        }

        private void OnEnable()
        {
            _dailyBonusManager.GiftCollected += OnGiftCollected;
            _openGiftButton.onClick.AddListener(OpenDailyGiftPlane);
        }

        private void OnDisable()
        {
            _openGiftButton.onClick.RemoveListener(OpenDailyGiftPlane);
            _dailyBonusManager.GiftCollected -= OnGiftCollected;
        }

        private void Start()
        {
            _dailyBonusManager.gameObject.SetActive(false);
            Load();
        }

        private void OpenDailyGiftPlane()
        {
            _dailyBonusManager.gameObject.SetActive(true);
        }

        private void SaveGiftData()
        {
            DailyGiftInfoSaver wrapper = new DailyGiftInfoSaver(DateTime.Today);
            string json = JsonConvert.SerializeObject(wrapper, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatString = "yyyy-MM-dd"
            });

            if (File.Exists(_savePath) && File.ReadAllText(_savePath) == json) return;

            File.WriteAllText(_savePath, json);
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(_savePath))
                {
                    ResetGift();
                    return;
                }

                var json = File.ReadAllText(_savePath);
                var wrapper = JsonConvert.DeserializeObject<DailyGiftInfoSaver>(json);

                if (wrapper.CollectedGiftDate.Date > DateTime.Today.Date)
                {
                    Debug.LogWarning("Invalid saved date detected. Resetting gift data.");
                    ResetGift();
                    return;
                }

                if (DateTime.Today > wrapper.CollectedGiftDate)
                {
                    ResetGift();
                    return;
                }

                _isGiftCollected = true;
                ToggleCollectButton();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading gift data: {ex}");
                ResetGift();
            }
        }
        
        private void ResetGift()
        {
            ToggleCollectButton();
        }

        private void OnGiftCollected()
        {
            _isGiftCollected = true;
            SaveGiftData();
            ToggleCollectButton();
        }

        private void ToggleCollectButton()
        {
            _openGiftButton.gameObject.SetActive(!_isGiftCollected);
        }
    }

    [Serializable]
    public class DailyGiftInfoSaver
    {
        public DateTime CollectedGiftDate;

        public DailyGiftInfoSaver(DateTime collectedGiftDate)
        {
            CollectedGiftDate = collectedGiftDate;
        }
    }
}