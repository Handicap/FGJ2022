using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGJ2022.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GameObject gridPoint;
        private static GridManager instace;
        public static GridManager Instance => instace;

        private void Start()
        {
            if (Instance == null)
            {
                instace = this;
            }
            else
            {
                Debug.LogError("Multiple grid managers", this);
                Destroy(this);
                return;
            }
        }

        private void GenerateGrid(int x, int y)
        {
            Debug.Log("asdf");
        }

        public void Initialize()
        {

        }

    }
}