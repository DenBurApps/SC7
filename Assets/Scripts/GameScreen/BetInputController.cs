using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreen
{
    public class BetInputController : MonoBehaviour
    {
        private const int StartBet = 10;

        [SerializeField] private TMP_Text _betText;
        [SerializeField] private Button _plusButton, _minusButton;
        [SerializeField] private Button _minButton, _maxButton;
        [SerializeField] private int _betIncreaseValue = 5;

        public int CurrentBet { get; private set; }

        private void OnEnable()
        {
            _plusButton.onClick.AddListener(IncreaseBet);
            _minusButton.onClick.AddListener(DecreaseBet);
            _minButton.onClick.AddListener(SetMinBet);
            _maxButton.onClick.AddListener(SetMaxBet);
        }

        private void OnDisable()
        {
            _plusButton.onClick.RemoveAllListeners();
            _minusButton.onClick.RemoveAllListeners();
            _minButton.onClick.RemoveAllListeners();
            _maxButton.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            CurrentBet = StartBet;
            UpdateBetText();
            UpdateButtonStates();
        }

        private void IncreaseBet()
        {
            int newBet = CurrentBet + _betIncreaseValue;
            SetBet(newBet);
        }

        private void DecreaseBet()
        {
            int newBet = CurrentBet - _betIncreaseValue;
            SetBet(newBet);
        }

        private void SetMinBet()
        {
            SetBet(StartBet);
        }

        private void SetMaxBet()
        {
            SetBet(PlayerBalanceController.CurrentBalance);
        }

        private void SetBet(int newBet)
        {
            newBet = Mathf.Clamp(newBet, StartBet, PlayerBalanceController.CurrentBalance);

            CurrentBet = newBet;
            UpdateBetText();
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            _minButton.interactable = CurrentBet > StartBet;
            _maxButton.interactable = CurrentBet < PlayerBalanceController.CurrentBalance;
            _plusButton.interactable = CurrentBet + _betIncreaseValue <= PlayerBalanceController.CurrentBalance;
            _minusButton.interactable = CurrentBet - _betIncreaseValue >= StartBet;
        }

        private void UpdateBetText()
        {
            _betText.text = CurrentBet.ToString();
        }
    }
}