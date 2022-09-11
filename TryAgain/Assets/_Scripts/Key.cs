using System;
using UnityEngine;

namespace _Scripts
{
    public class Key : MonoBehaviour
    {
        public static event Action<int> OnKeyPickUp;
        public static event Action OnPickUp;
        private int _id;

        public void Init(int id)
        {
            _id = id;
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            var obj = col.GetComponent<Player>();
            if (!obj) return;
            OnKeyPickUp?.Invoke(_id);
            OnPickUp?.Invoke();
            gameObject.SetActive(false);
        }
    }
}