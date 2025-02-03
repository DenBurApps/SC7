using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreen.UI
{
    public class CoefficientHistoryItem : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _coefficientText;

        public void Setup(float coefficient, Color color)
        {
            _background.color = color;
            _coefficientText.text = coefficient.ToString("F1") + "x";
        }
    }
}
