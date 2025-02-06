using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreen
{
    public class LineButton : MonoBehaviour
    {
        [SerializeField] private Color _defaultButtonColor;
        [SerializeField] private Color _selectedButtonColor;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private Image _lockImage;
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;

        private readonly int _availableLevel = 12;
        
        public event Action<LineButton> LineButtonClicked;

        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public int UnlockPrice { get; private set; }
        public bool IsLocked { get; private set; } = false;
        
        private string SaveKey => $"Level{Level}";

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void Start()
        {
            InitializeButton();
        }
        
        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }
        
        private void InitializeButton()
        {
            _numberText.text = Level.ToString();
            
            IsLocked = Level > _availableLevel && PlayerPrefs.GetInt(SaveKey) != 1;
            
            UpdateButtonState();
        }
        
        private void UpdateButtonState()
        {
            _lockImage.enabled = IsLocked;
        }
        
        public void SetSelectedColor()
        {
            if (IsLocked) return;
            _buttonImage.color = _selectedButtonColor;
        }

        public void SetDefaultColor()
        {
            _buttonImage.color = _defaultButtonColor;
        }

        public void Unlock()
        {
            if (!IsLocked)
                return;

            IsLocked = false;
            PlayerPrefs.SetInt(SaveKey, 1);
            UpdateButtonState();
        }

        private void OnButtonClicked()
        {
            LineButtonClicked?.Invoke(this);
        }
    }
}