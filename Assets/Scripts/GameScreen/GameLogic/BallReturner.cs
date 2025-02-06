using System;
using UnityEngine;

namespace GameScreen.GameLogic
{
    public class BallReturner : MonoBehaviour
    {
        [SerializeField] private BallSpawner _ballSpawner;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.TryGetComponent(out Ball ball))
                _ballSpawner.ReturnBallToPool(ball);
        }
    }
}
