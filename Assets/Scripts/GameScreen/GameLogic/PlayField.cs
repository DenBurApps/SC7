using UnityEngine;

namespace GameScreen.GameLogic
{
    public class PlayField : MonoBehaviour
    {
        [SerializeField] private CoefficientHolderController _coefficientHolderController;
        [SerializeField] private Transform _holePosition;

        public CoefficientHolderController CoefficientHolderController => _coefficientHolderController;
        public Transform HolePosition => _holePosition;
    }
}