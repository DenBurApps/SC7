using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private GameObject _privacyCanvas;
    [SerializeField] private GameObject _termsCanvas;
    [SerializeField] private GameObject _contactCanvas;
    [SerializeField] private TMP_Text _versionText;
    [SerializeField] private AudioControlButton _audioControlButton;

    private void Awake()
    {
        _settingsCanvas.SetActive(false);
        _privacyCanvas.SetActive(false);
        _termsCanvas.SetActive(false);
        _contactCanvas.SetActive(false);
        SetVersion();
    }

    private void SetVersion()
    {
        _versionText.text = "v" + Application.version;
    }

    public void ShowSettings()
    {
        _settingsCanvas.SetActive(true);
        _audioControlButton.UpdateSprite();
    }

    public void DisableSettings()
    {
        _settingsCanvas.SetActive(false);
    }

    public void RateUs()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#endif
    }
}
