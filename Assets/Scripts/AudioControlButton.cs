using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioControlButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Sprite _audioOnSprite;
    [SerializeField] private Sprite _audioOffSprite;

    private bool _isAudioOn = true;

    private void Awake()
    {
        _button.image.sprite = _audioOnSprite;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ToggleAudio);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ToggleAudio);
    }

    private void ToggleAudio()
    {
        _isAudioOn = !_isAudioOn;
        AudioListener.volume = _isAudioOn ? 1f : 0f;
        _button.image.sprite = _isAudioOn ? _audioOnSprite : _audioOffSprite;
    }

    public void UpdateSprite()
    {
        bool isOn = AudioListener.volume > 0f;
        _isAudioOn = isOn;
        _button.image.sprite = isOn ? _audioOnSprite : _audioOffSprite;
    }
}