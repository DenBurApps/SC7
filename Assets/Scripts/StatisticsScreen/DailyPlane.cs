using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StatisticsScreen
{
    public class DailyPlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _betText;
        [SerializeField] private TMP_Text _payoutText;
        [SerializeField] private Image _profitImage;
        [SerializeField] private TMP_Text _profitText;
        [SerializeField] private Color _plusProfitImageColor;
        [SerializeField] private Color _plusProfitTextColor;
        [SerializeField] private Color _minusProfitImageColor;
        [SerializeField] private Color _minusProfitTextColor;

        private RectTransform _rectTransform;
        private DailyStatisticsData _data;
        public bool IsActive { get; private set; }
        public int CurrentProfit => _data?.Profit ?? 0;
        
        public void Initialize(DailyStatisticsData data)
        {
            _rectTransform = GetComponent<RectTransform>();
            _data = data;
            UpdateUI();
            IsActive = true;
            _rectTransform.sizeDelta = new Vector2(830, 120);
        }

        private void UpdateUI()
        {
            if (_data == null) return;

            _timeText.text = _data.Time.ToString("HH:mm");
            _betText.text = _data.Bet.ToString();
            _payoutText.text = _data.Payout.ToString("F2");

            var isProfit = _data.Profit >= 0;
            _profitImage.color = isProfit ? _plusProfitImageColor : _minusProfitImageColor;
            _profitText.color = isProfit ? _plusProfitTextColor : _minusProfitTextColor;

            _profitText.text = isProfit ? "+" + _data.Profit.ToString() : _data.Profit.ToString();
        }

        public DailyStatisticsData GetData() => _data;

        public void SetActive(bool active)
        {
            IsActive = active;
            gameObject.SetActive(active);
        }
    }

    [Serializable]
    public class DailyStatisticsData
    {
        public DateTime Time;
        public int Bet;
        public float Payout;
        public int Profit;

        public DailyStatisticsData(DateTime time, int bet, float payout, int profit)
        {
            Time = time;
            Bet = bet;
            Payout = payout;
            Profit = profit;
        }
    }
}