using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreen.UI
{
    public class BuyButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Button _button;

        public event Action ButtonClicked;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void Enable(int amount)
        {
            gameObject.SetActive(true);
            
            if (amount >= 1000)
            {
                float thousands = amount / 1000f;
                if (amount % 1000 == 0)
                {
                    _amountText.text = $"{thousands:0}k";
                }
                else
                {
                    _amountText.text = $"{thousands:0.###}k";
                }
            }
            else
            {
                _amountText.text = amount.ToString();
            }
        }

        private void OnButtonClicked()
        {
            ButtonClicked?.Invoke();
        }
    }
}
