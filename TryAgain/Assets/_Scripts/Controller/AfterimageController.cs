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
            
            if(afterimages is null)
                _afterImages = new Afterimage[] { };
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