using UnityEngine;

namespace _Scripts
{
    public class LockedDoor : MonoBehaviour
    {
        private int _id;

        public void Init(int id)
        {
            _id = id;
        }
        
        private void Awake()
        {
            Key.OnKeyPickUp += Open;
        }

        private void Open(int keyId)
        {
            if (keyId != _id) return;

            Key.OnKeyPickUp -= Open;
            gameObject.SetActive(false);
        }
    }
}