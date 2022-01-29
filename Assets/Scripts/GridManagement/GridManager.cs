using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace FGJ2022.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GameObject gridCenter;
        private static GridManager instance;
        public static GridManager Instance => instance;

        private void Start()
        {
            if (Instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("Multiple grid managers", this);
                Destroy(this);
                return;
            }
        }

        [Button]
        public void GenerateDefaultGrid()
        {
            GenerateGrid(10, 10);
        }

        
        public void GenerateGrid(int x, int y)
        {
            Debug.Log("asdf");
        }

        public void Initialize()
        {

        }

    }
}