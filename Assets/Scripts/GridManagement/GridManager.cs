using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

namespace FGJ2022.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GridCell cellPrefab;

        [SerializeField] private Vector2Int start;
        [SerializeField] private Vector2Int end;

        [SerializeField] private List<Actors.BaseActor> npcs = new List<Actors.BaseActor>();

        private static GridManager instance;
        public static GridManager Instance => instance;

        private Grid currentGrid;


        private void Awake()
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

            GenerateDefaultGrid();
            //GenerateAreas();
        }

        public void ResetCellColors()
        {
            currentGrid.ResetAllColors();
        }

        [Button]
        public void TestFindPath()
        {
            currentGrid.ColorAll(Color.black);
            GridCell from = currentGrid.GetCell(start);
            GridCell to = currentGrid.GetCell(end);
            List<GridCell> path =  GridPathFinding.FindPath(from, to, currentGrid);
            if (path == null) return;
            List<string> pathStrings = new List<string>();
            foreach (var item in path)
            {
                item.SetColor(Color.magenta);
                pathStrings.Add(item.Coordinate.ToString());
            }
            Debug.Log("Path was " + string.Join(", ", pathStrings));
        }

        public List<GridCell> FindPath(GridCell from, GridCell to, out bool foundPath)
        {
            if (from == null || to == null)
            {
                Debug.LogWarning("No path given");
                foundPath = false;
                return new List<GridCell>();
            }
            foundPath = true;
            List<GridCell> path = GridPathFinding.FindPath(from, to, currentGrid);
            //List<string> pathStrings = new List<string>();
            //Debug.Log("Path was " + string.Join(", ", pathStrings));
            if (path == null)
            {
                foundPath = false;
                return new List<GridCell>();
            }
            return path;
        }

        [Button]
        public void GenerateDefaultGrid()
        {
            GenerateGrid(10, 10);
        }


        [Button]
        public void CreateNewAreas(out List<Actors.BaseActor> newActors)
        {
            List<GridCell> newCells = currentGrid.GenerateNewCells();
            newActors = new List<Actors.BaseActor>();
            foreach (var item in npcs)
            {
                newActors.AddRange(currentGrid.GenerateCharacters(item, 3, newCells.First().Coordinate, newCells.Last().Coordinate));
            }
        }
        
        public void GenerateGrid(int x, int y)
        {
            currentGrid = new Grid(cellPrefab, x, y, this.transform);
        }

        public void Initialize()
        {

        }
    }
}