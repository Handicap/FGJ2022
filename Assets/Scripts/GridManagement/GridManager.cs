using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace FGJ2022.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Transform gridCenter;
        [SerializeField] private GridCell cellPrefab;

        [SerializeField] private Vector2Int start;
        [SerializeField] private Vector2Int end;

        private static GridManager instance;
        public static GridManager Instance => instance;

        private Grid currentGrid;

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
        public void FindPath()
        {
            currentGrid.ColorAll(Color.black);
            GridCell from = currentGrid.GetCell(start);
            GridCell to = currentGrid.GetCell(end);
            List<GridCell> path =  GridPathFinding.FindPath(from, to, currentGrid);
            List<string> pathStrings = new List<string>();
            foreach (var item in path)
            {
                item.SetColor(Color.magenta);
                pathStrings.Add(item.Coordinate.ToString());
            }
            Debug.Log("Path was " + string.Join(", ", pathStrings));
        }

        [Button]
        public void GenerateDefaultGrid()
        {
            GenerateGrid(10, 10);
        }

        [Button]
        public void GenerateAreas()
        {
            currentGrid.GenerateAreas(new Vector2Int(0, 0), new Vector2Int(10, 10));
        }

        
        public void GenerateGrid(int x, int y)
        {
            currentGrid = new Grid(cellPrefab, x, y, this.transform);
            Debug.Log("asdf");
        }

        public void Initialize()
        {

        }

    }
}