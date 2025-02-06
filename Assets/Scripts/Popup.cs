using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private float _startYPosition = -1000f;
    [SerializeField] private Ease _easeType = Ease.OutBack;
        
    private RectTransform _rectTransform;
    private Vector2 _targetPosition;
    private Tween _currentTween;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _targetPosition = _rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        _currentTween?.Kill();
        _rectTransform.anchoredPosition = new Vector2(_targetPosition.x, _startYPosition);
        
        _currentTween = _rectTransform.DOAnchorPosY(_targetPosition.y, _animationDuration)
            .SetEase(_easeType)
            .OnComplete(() => _currentTween = null);
    }

    private void OnDisable()
    {
        _currentTween?.Kill();
        _currentTween = null;
        
        _rectTransform.anchoredPosition = _targetPosition;
    }
}
