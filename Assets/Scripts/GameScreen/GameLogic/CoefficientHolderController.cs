using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameScreen.GameLogic
{
    public class CoefficientHolderController : MonoBehaviour
    {
        [SerializeField] private CoefficientHolder[] _coefficientHolders;
        [SerializeField] private CoefficientGenerator.RiskLevel _riskLevel;
        [SerializeField] private float _maxCoefficient;

        public event Action<float, Color> CoefficientInteracted;
        
        private void Start()
        {
            InitializeCoefficients();
        }

        private void InitializeCoefficients()
        {
            if (_coefficientHolders == null || _coefficientHolders.Length == 0)
            {
                return;
            }

            List<double> multipliers = CoefficientGenerator.GenerateMultipliers(
                _coefficientHolders.Length - 1, 
                _riskLevel
            );

            for (int i = 0; i < _coefficientHolders.Length; i++)
            {
                if (_coefficientHolders[i] != null)
                {
                    _coefficientHolders[i].SetCoefficient((float)multipliers[i], _maxCoefficient);
                }
            }
        }
        
        public void UpdateRiskLevel(CoefficientGenerator.RiskLevel newRiskLevel)
        {
            _riskLevel = newRiskLevel;
            InitializeCoefficients();
        }
        
        private void OnEnable()
        {
            foreach (var holder in _coefficientHolders)
            {
                if (holder != null)
                {
                    holder.Interacted += OnCoefficientInteracted;
                }
            }
        }

        private void OnDisable()
        {
            foreach (var holder in _coefficientHolders)
            {
                if (holder != null)
                {
                    holder.Interacted -= OnCoefficientInteracted;
                }
            }
        }

        private void OnCoefficientInteracted(CoefficientHolder holder)
        {
            if (holder != null)
            {
                CoefficientInteracted?.Invoke(holder.CurrentCoefficient, holder.CurrentColor);
            }
            
            Debug.Log($"Interacted with coefficient: {holder.CurrentCoefficient}");
        }
    }
}
