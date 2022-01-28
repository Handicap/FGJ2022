using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGJ2022.Grid
{
    public class GridManager : MonoBehaviour
    {
        private static GridManager instace;
        public static GridManager Instance => instace;
        public Vector2[] GameGrid => gameGrid;
        private Vector2[] gameGrid;

        private void Start()
        {
            if (Instance != null)
            {
                instace = this;
            }
            else
            {
                Debug.LogError("Multiple", this);
            }
        }

        public void Initialize()
        {

        }

    }
}