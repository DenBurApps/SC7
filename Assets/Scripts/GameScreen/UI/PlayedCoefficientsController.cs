using System;
using System.Collections.Generic;
using System.Linq;
using GameScreen.GameLogic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace GameScreen.UI
{
    public class PlayedCoefficientsController : MonoBehaviour
    {
        [SerializeField] private CoefficientHolderController[] _coefficientManagers;
        [SerializeField] private CoefficientHistoryItem _coefficientHistoryPrefab;
        [SerializeField] private Transform _historyContainer;
        [SerializeField] private int _maxHistoryItems = 20;
        [SerializeField] private List<CoefficientHistoryItem> _allItems;

        private Queue<CoefficientHistoryItem> _activeItems = new Queue<CoefficientHistoryItem>();
        private Stack<CoefficientHistoryItem> _inactiveItems;

        private void Start()
        {
            InitializeHistoryItems();
        }

        private void OnEnable()
        {
            foreach (var manager in _coefficientManagers)
            {
                manager.CoefficientInteracted += OnCoefficientPlayed;
            }
        }

        private void OnDisable()
        {
            foreach (var manager in _coefficientManagers)
            {
                manager.CoefficientInteracted -= OnCoefficientPlayed;
            }
        }

        private void InitializeHistoryItems()
        {
            _inactiveItems = new Stack<CoefficientHistoryItem>();
            
            foreach (var item in _allItems)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(false);
                    _inactiveItems.Push(item);
                }
            }
        }


        private void OnCoefficientPlayed(float coefficient, Color coefficientColor)
        {
            CoefficientHistoryItem historyItem;

            if (_activeItems.Count >= _maxHistoryItems)
            {
                historyItem = _activeItems.Dequeue();
                historyItem.transform.SetSiblingIndex(_historyContainer.childCount - 1);
            }
            else if (_inactiveItems.Count > 0)
            {
                historyItem = _inactiveItems.Pop();
                historyItem.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("No available history items in pool!");
                return;
            }

            historyItem.Setup(coefficient, coefficientColor);
            _activeItems.Enqueue(historyItem);
        }


        public void ClearHistory()
        {
            while (_activeItems.Count > 0)
            {
                CoefficientHistoryItem item = _activeItems.Dequeue();
                item.gameObject.SetActive(false);
                _inactiveItems.Push(item);
            }
        }
    }
}