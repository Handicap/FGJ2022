using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FGJ2022.Input
{
    public class InputSystem : MonoBehaviour
    {
        private bool clicked = false;
        public event Action OnClick;
        private InputSystem instance;

        private void Start()
        {
            if (instance != null)
            {
                instance = this;
            } else
            {
                DestroyImmediate(this);
            }
        }
        private void OnMouseDown()
        {
            clicked = true;
        }

        private void OnMouseUp()
        {
            clicked = false;
            OnClick?.Invoke();
        }
    }
}
