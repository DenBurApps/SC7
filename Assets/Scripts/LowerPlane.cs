using System;
using System.Collections;
using System.Collections.Generic;
using Achievemnts;
using GameScreen.GameLogic;
using ShopScreen;
using StatisticsScreen;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class LowerPlane : MonoBehaviour
{
    [SerializeField] private Button _gameButton;
    [SerializeField] private Button _achievementsButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _statisticsButton;
    [SerializeField] private Button _settingsButton;

    [SerializeField] private GameController _gameController;
    [SerializeField] private AchievementsHolder _achievementsHolder;
    [SerializeField] private ShopScreenController _shopScreenController;
    [SerializeField] private StatisticsManager _statisticsManager;
    [SerializeField] private Settings _settings;

    private void OnEnable()
    {
        _gameButton.onClick.AddListener(OnGameButtonClick);
        _achievementsButton.onClick.AddListener(OnAchievementsButtonClick);
        _shopButton.onClick.AddListener(OnShopButtonClick);
        _statisticsButton.onClick.AddListener(OnStatisticsButtonClick);
        _settingsButton.onClick.AddListener(OnSettingsButtonClick);
    }

    private void OnDisable()
    {
        _gameButton.onClick.RemoveListener(OnGameButtonClick);
        _achievementsButton.onClick.RemoveListener(OnAchievementsButtonClick);
        _shopButton.onClick.RemoveListener(OnShopButtonClick);
        _statisticsButton.onClick.RemoveListener(OnStatisticsButtonClick);
        _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
    }

    public void ToggleAllButtons(bool status)
    {
        _gameButton.enabled = status;
        _achievementsButton.enabled = status;
        _shopButton.enabled = status;
        _settingsButton.enabled = status;
        _statisticsButton.enabled = status;
    }
    
    private void OnGameButtonClick()
    {
        DisableAllWindows();
        _gameController.EnableScreen();
    }

    private void OnAchievementsButtonClick()
    {
        DisableAllWindows();
        _achievementsHolder.EnableScreen();
    }

    private void OnShopButtonClick()
    {
        DisableAllWindows();
        _shopScreenController.EnableScreen();
    }

    private void OnStatisticsButtonClick()
    {
        DisableAllWindows();
        _statisticsManager.EnableScreen();
    }

    private void OnSettingsButtonClick()
    {
        DisableAllWindows();
        _settings.ShowSettings();
    }

    private void DisableAllWindows()
    {
        _gameController.DisableScreen();
        _achievementsHolder.DisableScreen();
        _shopScreenController.DisableScreen();
        _statisticsManager.DisableScreen();
        _settings.DisableSettings();
    }
}
