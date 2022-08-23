using System;
using UnityEngine;

namespace _Scripts
{
    public class AfterimageController : MonoBehaviour
    {
        private Afterimage[] _afterImages;

        public void Init(Afterimage[] afterimages)
        {
            _afterImages = afterimages;
        }

        private void Update()
        {
            foreach (var afterimage in _afterImages)
            {
                afterimage.UpdateCharacter();
            }
        }
    }
}