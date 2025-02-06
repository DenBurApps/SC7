using System;
using UnityEngine;
using UnityEngine.UI;

namespace Achievemnts
{
    public class Achievement : MonoBehaviour
    {
        [SerializeField] private Image _mainImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _lockImage;
        [field: SerializeField] public int Id { get; private set; }

        public Sprite ImageSprite => _mainImage.sprite;
        public bool IsLocked { get; private set; }

        private void Awake()
        {
            IsLocked = true;
            _frameImage.enabled = true;
            _lockImage.gameObject.SetActive(true);
        }

        public void Unlock()
        {
            _frameImage.enabled = false;
            _lockImage.gameObject.SetActive(false);
            IsLocked = false;
        }
        
        public void Reset()
        {
            IsLocked = true;
            _frameImage.enabled = true;
            _lockImage.gameObject.SetActive(true);
        }
    }
}
