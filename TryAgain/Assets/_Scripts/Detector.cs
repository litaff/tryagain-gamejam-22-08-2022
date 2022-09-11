using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts
{
    public class Detector : MonoBehaviour
    {
        public event Action<Character> OnCharacterEnter;
        [SerializeField] private LayerMask allowedLayers;
        private List<Collider2D> _currentCollider2Ds;

        private void Awake()
        {
            _currentCollider2Ds = new List<Collider2D>();
        }

        public void RotateDetection(float degree)
        {
            transform.eulerAngles = new Vector3(0f, 0f, degree);
        }

        public IEnumerable<T> GetAllOfType<T>()
        {
            return _currentCollider2Ds.Select(
                currentCollider2D => currentCollider2D.GetComponent<T>()).Where(
                current => current is { }).ToList();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(((1 << other.gameObject.layer) & allowedLayers) == 0) return;
            
            _currentCollider2Ds.Add(other);
            
            var character = other.GetComponent<Character>();
            if (character)
            {
                OnCharacterEnter?.Invoke(character);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(((1 << other.gameObject.layer) & allowedLayers) == 0) return;
            
            _currentCollider2Ds.Remove(other);
        }
    }
}