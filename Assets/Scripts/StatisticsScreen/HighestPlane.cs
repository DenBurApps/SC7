using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StatisticsScreen
{
    public class HighestPlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private TMP_Text _betText;
        [SerializeField] private TMP_Text _payoutText;
        [SerializeField] private Image _profitImage;
        [SerializeField] private TMP_Text _profitText;
        [SerializeField] private Color _plusProfitImageColor;
        [SerializeField] private Color _plusProfitTextColor;
        [SerializeField] private Color _minusProfitImageColor;
        [SerializeField] private Color _minusProfitTextColor;

        private RectTransform _rectTransform;
        private HighestData _data;
        public bool IsActive { get; private set; }
        public int CurrentProfit => _data.Profit;

        public void Initialize(HighestData data)
        {
            if(data.Profit <= 0)
                return;
            
            _rectTransform = GetComponent<RectTransform>();
            _data = data;
            UpdateUI();
            IsActive = true;
            _rectTransform.sizeDelta = new Vector2(830, 120);
        }

        private void UpdateUI()
        {
            _dateText.text = _data.Date.ToString("dd.MM.yyyy");
            _betText.text = _data.Bet.ToString();
            _payoutText.text = _data.Payout.ToString("F2");
            _profitText.text = "+" + _data.Profit.ToString();

            var isProfit = _data.Profit >= 0;
            _profitImage.color = isProfit ? _plusProfitImageColor : _minusProfitImageColor;
            _profitText.color = isProfit ? _plusProfitTextColor : _minusProfitTextColor;
        }

        public HighestData GetData() => _data;
    }

    [Serializable]
    public class HighestData
    {
        public DateTime Date;
        public int Bet;
        public float Payout;
        public int Profit;

        public HighestData(DateTime date, int bet, float payout, int profit)
        {
            Date = date;
            Bet = bet;
            Payout = payout;
            Profit = profit;
        }
    }
}