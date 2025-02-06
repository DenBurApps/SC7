using System.Collections.Generic;
using ShopScreen;
using UnityEngine;

namespace GameScreen.GameLogic
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private int _initialPoolSize = 50;
        [SerializeField] private bool _expandPoolIfNeeded = true;
        [SerializeField] private ShopScreenController _shopController;

        private GameController _gameController;
        private Queue<Ball> _availableBalls;
        private HashSet<Ball> _activeBalls;
        private Sprite _currentBallSprite;
        private Dictionary<Ball, bool> _ballStates; 

        public int ActiveBallsCount => _activeBalls.Count;
        
        private void Awake()
        {
            _availableBalls = new Queue<Ball>();
            _activeBalls = new HashSet<Ball>();
            _ballStates = new Dictionary<Ball, bool>();

            InitializePool();
        }

        private void OnEnable()
        {
            if (_shopController != null)
            {
                _shopController.OnSkinSelected += HandleSkinChanged;
            }

            HandleSkinChanged(_shopController.CurrentSelectedElement);
        }

        private void OnDisable()
        {
            if (_shopController != null)
            {
                _shopController.OnSkinSelected -= HandleSkinChanged;
            }

            ReturnAllBallsToPool();
            _availableBalls.Clear();
            _activeBalls.Clear();
        }

        private void HandleSkinChanged(SkinElement selectedSkin)
        {
            _currentBallSprite = selectedSkin.SkinSprite;

            foreach (var ball in _activeBalls)
            {
                ball.UpdateSprite(_currentBallSprite);
            }

            foreach (var ball in _availableBalls)
            {
                ball.UpdateSprite(_currentBallSprite);
            }
        }

        public void Initialize(GameController gameController)
        {
            _gameController = gameController;
        }

        public Ball SpawnBall(Vector3 position)
        {
            Ball ball = GetBallFromPool();
            if (ball == null) return null;

            if (_activeBalls.Contains(ball))
            {
                return null;
            }

            ball.transform.position = position;
            
            if (_currentBallSprite != null)
            {
                ball.UpdateSprite(_currentBallSprite);
            }
            
            ball.gameObject.SetActive(true);
            ball.ResetBall();
            ball.DropBall();

            _activeBalls.Add(ball);
            _ballStates[ball] = true;

            return ball;
        }

        public void ReturnBallToPool(Ball ball)
        {
            if (ball == null || !_activeBalls.Contains(ball))
            {
                return;
            }
            
            if (!_ballStates.TryGetValue(ball, out bool isActive) || !isActive)
            {
                return;
            }

            ball.gameObject.SetActive(false);
            ball.transform.SetParent(transform);
            _activeBalls.Remove(ball);
            _availableBalls.Enqueue(ball);
            CheckAllBallsFinished();
        }

        public void ReturnAllBallsToPool()
        {
            var ballsToReturn = new List<Ball>(_activeBalls);
            foreach (var ball in ballsToReturn)
            {
                ReturnBallToPool(ball);
            }
        }

        private void InitializePool()
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                CreateNewBall();
            }
        }

        private Ball CreateNewBall()
        {
            Ball ball = Instantiate(_ballPrefab, transform);
            
            if (_currentBallSprite != null)
            {
                ball.UpdateSprite(_currentBallSprite);
            }
            
            ball.gameObject.SetActive(false);
            _ballStates[ball] = false;
            _availableBalls.Enqueue(ball);
            return ball;
        }

        private Ball GetBallFromPool()
        {
            if (_availableBalls.Count == 0)
            {
                if (_expandPoolIfNeeded)
                {
                    return CreateNewBall();
                }
                return null;
            }

            Ball ball = _availableBalls.Dequeue();

            if (_activeBalls.Contains(ball))
            {
                return CreateNewBall();
            }

            return ball;
        }

        private void CheckAllBallsFinished()
        {
            if (_activeBalls.Count == 0 && _gameController != null)
            {
                _gameController.OnBallFinished();
            }
        }
    }
}