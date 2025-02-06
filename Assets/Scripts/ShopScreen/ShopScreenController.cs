using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ShopScreen
{
    public class ShopScreenController : MonoBehaviour
    {
        [SerializeField] private List<SkinElement> _skinElements;
        [SerializeField] private string _saveFileName = "skinsData.json";
        [SerializeField] private GameObject _notEnoughScreen;
        [SerializeField] private AudioControlButton _audioControlButton;

        private SkinElement _currentlySelectedSkin;
        private SkinsData _skinsData;
        private Dictionary<int, Action<SkinElement>> _unlockHandlers;
        private Dictionary<int, Action<SkinElement>> _selectHandlers;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        public event Action<SkinElement> OnSkinSelected;

        public SkinElement CurrentSelectedElement => _currentlySelectedSkin;

        private void Awake()
        {
            _unlockHandlers = new Dictionary<int, Action<SkinElement>>();
            _selectHandlers = new Dictionary<int, Action<SkinElement>>();
            LoadSkinsData();
            InitializeSkinElements();

            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            for (int i = 0; i < _skinElements.Count; i++)
            {
                var index = i;
                _skinElements[i].OnUnlockAttempted += _unlockHandlers[index];
                _skinElements[i].OnSkinClicked += _selectHandlers[index];
                _skinElements[i].gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _skinElements.Count; i++)
            {
                var index = i;
                _skinElements[i].OnUnlockAttempted -= _unlockHandlers[index];
                _skinElements[i].OnSkinClicked -= _selectHandlers[index];
            }
        }

        private void Start()
        {
            DisableScreen();
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

        private void InitializeSkinElements()
        {
            for (int i = 0; i < _skinElements.Count; i++)
            {
                var skinElement = _skinElements[i];
                var index = i;
                
                bool isInitiallyUnlocked = i < 4;
                
                var skinState = _skinsData.Skins.Find(s => s.Index == index);
                if (skinState == null)
                {
                    skinState = new SkinState
                    {
                        Index = index,
                        IsUnlocked = isInitiallyUnlocked,
                        IsSelected = index == 0
                    };
                    _skinsData.Skins.Add(skinState);
                }
                
                _unlockHandlers[index] = _ => TryUnlockSkin(index);
                _selectHandlers[index] = _ => SelectSkin(index);
       
                UpdateSkinElementUI(skinElement, skinState);
                
                if (skinState.IsSelected)
                {
                    _currentlySelectedSkin = skinElement;
                }
            }
            
            SaveSkinsData();
        }

        private void UpdateSkinElementUI(SkinElement skinElement, SkinState skinState)
        {
            bool isLocked = !skinState.IsUnlocked;
            skinElement.IsLocked = isLocked;
            skinElement.SetSelected(skinState.IsSelected);
        }

        private void SelectSkin(int index)
        {
            if (_currentlySelectedSkin != null)
            {
                var previousState = _skinsData.Skins.Find(s => s.Index == _skinElements.IndexOf(_currentlySelectedSkin));
                if (previousState != null)
                {
                    previousState.IsSelected = false;
                    UpdateSkinElementUI(_currentlySelectedSkin, previousState);
                }
            }
            
            var skinElement = _skinElements[index];
            var skinState = _skinsData.Skins.Find(s => s.Index == index);
            
            skinState.IsSelected = true;
            _currentlySelectedSkin = skinElement;
            
            UpdateSkinElementUI(skinElement, skinState);
            SaveSkinsData();
            
            OnSkinSelected?.Invoke(skinElement);
        }

        public void UnlockFootballSkin()
        {
            TryUnlockSkin(6);
        }
        
        private void TryUnlockSkin(int index)
        {
            var skinElement = _skinElements[index];
            
            if (PlayerBalanceController.HasEnoughBalance(skinElement.OpenPrice))
            {
                PlayerBalanceController.DecreaseBalance(skinElement.OpenPrice);
                
                var skinState = _skinsData.Skins.Find(s => s.Index == index);
                skinState.IsUnlocked = true;
                
                skinElement.PlayUnlockAnimation();
                UpdateSkinElementUI(skinElement, skinState);
                SaveSkinsData();
            }
            else
            {
                _notEnoughScreen.SetActive(true);
            }
        }

        private void LoadSkinsData()
        {
            string path = Path.Combine(Application.persistentDataPath, _saveFileName);
            
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                _skinsData = JsonConvert.DeserializeObject<SkinsData>(json);
            }
            else
            {
                _skinsData = new SkinsData();
            }
        }

        private void SaveSkinsData()
        {
            string path = Path.Combine(Application.persistentDataPath, _saveFileName);
            string json = JsonConvert.SerializeObject(_skinsData);
            File.WriteAllText(path, json);
        }

        [System.Serializable]
        private class SkinsData
        {
            public List<SkinState> Skins = new List<SkinState>();
        }
        
        [System.Serializable]
        private class SkinState
        {
            public int Index;
            public bool IsUnlocked;
            public bool IsSelected;
        }
    }
}