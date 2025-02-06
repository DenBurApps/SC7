using System;
using TMPro;
using UnityEngine;

namespace GameScreen.UI
{
    public class BalanceShower : MonoBehaviour
    {
        [SerializeField] private TMP_Text _balanceText;

        private void Start()
        {
            UpdateBalanceText(PlayerBalanceController.CurrentBalance);
        }

        private void OnEnable()
        {
            PlayerBalanceController.OnBalanceChanged += UpdateBalanceText;
        }

        private void OnDisable()
        {
            PlayerBalanceController.OnBalanceChanged -= UpdateBalanceText;
        }

        private void UpdateBalanceText(int balance)
        {
            if (balance >= 1000)
            {
                float thousands = balance / 1000f;
                if (balance % 1000 == 0)
                {
                    _balanceText.text = $"{thousands:0}k";
                }
                else
                {
                    _balanceText.text = $"{thousands:0.###}k";
                }
            }
            else
            {
                _balanceText.text = balance.ToString();
            }
        }
    }
}
