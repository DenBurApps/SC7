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

        private BallSpawner _ballSpawner;
        
        public event Action<float, Color> CoefficientInteracted;
        
        private void Start()
        {
            UpdateMaxCoefficient();
            InitializeCoefficients();
        }

        public void Initialize(BallSpawner spawner)
        {
            _ballSpawner = spawner;
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
            UpdateMaxCoefficient();
            InitializeCoefficients();
        }
        
        public float GetHighestPossibleCoefficient()
        {
            float highest = 0f;
            foreach (var holder in _coefficientHolders)
            {
                if (holder.CurrentCoefficient > highest)
                {
                    highest = holder.CurrentCoefficient;
                }
            }
            return highest;
        }
        
        private void UpdateMaxCoefficient()
        {
            var maxPossibleMultipliers = CoefficientGenerator.GenerateMultipliers(
                _coefficientHolders.Length - 1,
                CoefficientGenerator.RiskLevel.High 
            );
            _maxCoefficient = (float)maxPossibleMultipliers.Max() * 1.1f;
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

        private void OnCoefficientInteracted(CoefficientHolder holder, Ball ball)
        {
            if (holder != null)
            {
                CoefficientInteracted?.Invoke(holder.CurrentCoefficient, holder.CurrentColor);
                _ballSpawner.ReturnBallToPool(ball);
            }
            
            Debug.Log($"Interacted with coefficient: {holder.CurrentCoefficient}");
        }
    }
}
