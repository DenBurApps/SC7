using System;
using TMPro;
using UnityEngine;

namespace GameScreen.GameLogic
{
    public class CoefficientHolder : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coefficientText;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private Color _greenColor;
        [SerializeField] private Color _yellow1Color;
        [SerializeField] private Color _yellow2Color;
        [SerializeField] private Color _redColor;

        private float _maxCoefficient;

        public event Action<CoefficientHolder, Ball> Interacted;

        public float CurrentCoefficient { get; private set; }

        public Color CurrentColor => _spriteRenderer.color;

        public void SetCoefficient(float value, float maxCoefficient)
        {
            _maxCoefficient = maxCoefficient;
            CurrentCoefficient = value;
            _coefficientText.text = "x" + CurrentCoefficient.ToString();

            float normalizedValue = CurrentCoefficient / _maxCoefficient;

            Color targetColor;
            if (normalizedValue <= 0.03f)
                targetColor = Color.Lerp(_greenColor, _yellow1Color, normalizedValue * 4f);
            else if (normalizedValue <= 0.04f)
                targetColor = Color.Lerp(_yellow1Color, _yellow2Color, (normalizedValue - 0.25f) * 10f);
            else if (normalizedValue <= 0.10f)
                targetColor = Color.Lerp(_yellow2Color, _redColor, (normalizedValue - 0.35f) * 6.67f);
            else
                targetColor = _redColor;

            _spriteRenderer.color = targetColor;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Ball ball))
                Interacted?.Invoke(this, ball);
        }
    }
}