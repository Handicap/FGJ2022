using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FGJ2022.Grid
{
    public class Grid
    {
        private List<List<GridCell>> cells = new List<List<GridCell>>();
        private GameObject baseObject;
        private GridCell gridPrefab;

        public Vector2 CellSize { get
            {
                return gridPrefab.Dimensions;
            }
        }

        public List<GridCell> AllCells { get
            {
                List<GridCell> retCells = new List<GridCell>();
                for (int i = 0; i < cells.Count; i++)
                {
                    for (int j = 0; j < cells[i].Count; j++)
                    {
                        retCells.Add(cells[i][j]);
                    }
                }
                return retCells;
            } 
        }

        public Vector2Int Size => new Vector2Int(cells.Count, cells[0].Count);

        public Grid(GridCell gridPrefab, int xDimension, int yDimension, Transform parent = null)
        {
            baseObject = new GameObject();
            baseObject.name = "Grid-" + xDimension + "-" + yDimension;
            if (parent != null)
            {
                baseObject.transform.SetParent(parent);
            }
            for (int x = 0; x < xDimension; x++)
            {
                List<GridCell> column = CreateGridColumn(yDimension, gridPrefab);
                cells.Add(column);                
            }
            //Color mainColor = Color.red;
            //Color secondaryColor = Color.black;
            //ColorCheckerBoard(mainColor, secondaryColor);
            SetCellTransforms();
            ConnectCells();
            float seed = Random.Range(100f, 200f);
            CreateBlockers(new Vector2Int(0, 0), new Vector2Int(Size.x, Size.y), seed: seed, threshhold: 0.70f);
        }

        private void SetCellTransforms()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    GridCell cell = cells[i][j];
                    cell.transform.position = new Vector3(CellSize.x * i, 0, j);
                    cell.transform.SetParent(baseObject.transform);
                }
            }
        }

        private List<GridCell> CreateGridColumn(int length, GridCell gridPrefab)
        {
            this.gridPrefab = gridPrefab;
            List<GridCell> column = new List<GridCell>();
            for (int y = 0; y < length; y++)
            {
                GridCell newCell = GameObject.Instantiate(gridPrefab);
                //newCell.transform.position = new Vector3(cellSize.x * x, 0, y);
                newCell.name = "Cell-" + y;
                //cells[x].Add(newCell);
                column.Add(newCell);
                //newCell.transform.SetParent(baseObject.transform);
            }
            return column;
        }
        private void SetNeighbours()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    GridCell cell = cells[i][j];
                    if (i != 0)
                    {
                        cell.WestNeighbour = cells[i - 1][j];
                    }
                    if (i != cells.Count - 1)
                    {
                        cell.EastNeighbour = cells[i + 1][j];
                    }
                    if (j != 0)
                    {
                        cell.SouthNeighbour = cells[i][j - 1];
                    }
                    if (j != cells[0].Count - 1)
                    {
                        cell.NorthNeighbour = cells[i][j + 1];
                    }
                }
            }
        }
        private void SetEdgeAreas()
        {
            foreach (var item in AllCells)
            {
                if (item.NorthNeighbour == null)
                {
                    item.Type = CellType.Edge;
                } else
                {
                    item.Type = CellType.Default;
                }
                //item.Type = item.NeighbourCount < 4 ? CellType.Edge : CellType.Default;
            }
        }

        // push new rows to the stack
        public List<GridCell> GenerateNewCells()
        {
            int startingCellCount = AllCells.Count;
            int startRow = cells.First().Count;
            List<GridCell> newCells = new List<GridCell>();
            for (int i = 0; i < Size.x; i++)
            {
                List<GridCell> column = cells[i];
                List<GridCell> newColumn = CreateGridColumn(Size.x, gridPrefab);
                column.AddRange(newColumn);
                newCells.AddRange(newColumn);
            }

            float seed = Random.Range(100f, 200f);
            CreateBlockers(new Vector2Int(0, startRow), new Vector2Int(Size.x, cells.First().Count), threshhold: 0.7f, seed: seed);
            ConnectCells();
            SetCellTransforms();
            int endCellCount = AllCells.Count;
            Debug.Log("Generated new area with seed " + seed + " " + startingCellCount + " -> " + endCellCount);
            return newCells;
        }

        public List<Actors.BaseActor> GenerateCharacters(Actors.BaseActor actor, int amount, Vector2Int startCorner, Vector2Int endCorner)
        {
            List<Actors.BaseActor> newCharacters = new List<Actors.BaseActor>();
            while (newCharacters.Count < amount)
            {
                Vector2Int randomPoint = new Vector2Int(Random.Range(startCorner.x, endCorner.x), Random.Range(startCorner.y, endCorner.y));
                GridCell cell = GetCell(randomPoint);
                if (cell.Passability == CellPassability.Passable)
                {
                    Actors.BaseActor newActor = GameObject.Instantiate(actor);
                    //newActor.transform.position = newActor.
                    newActor.SnapToGrid(cell);
                    newCharacters.Add(newActor);
                }
            }
            Debug.Log("Created " + amount + " new characters");
            return newCharacters;
        }

        private void ConnectCells()
        {
            for (int i = 0; i < cells.Count; i++)
            {   
                for (int j = 0; j < cells[i].Count; j++)
                {
                    GetCell(i, j).Initialize(new Vector2Int(i, j));
                }
            }
            SetNeighbours();
            SetEdgeAreas();
        }

        // just some sugar to stay on track
        public GridCell GetCell(int x, int y)
        {
            return cells[x][y];
        }
        // just some sugar to stay on track
        public GridCell GetCell(Vector2Int position)
        {
            return cells[position.x][position.y];
        }


        private void CreateBlockers(Vector2Int startPosition, Vector2Int endPosition, float threshhold = 0.5f, float seed = 1f)
        {
            
            for (int x = startPosition.x; x < endPosition.x; x++)
            {
                for (int y = startPosition.y; y < endPosition.y; y++)
                {
                    float xFactor = (float)x / (float)cells.Count;
                    float yFactor = (float)y / (float)cells.Count;
                    float perlinSample = Mathf.PerlinNoise(xFactor * seed, yFactor * seed);

                    if (perlinSample > threshhold)
                    {
                        GetCell(x, y).Passability = CellPassability.Impassable;
                    }
                    else
                    {
                        GetCell(x, y).Passability = CellPassability.Passable;
                    }
                }
            }
        }

        public void ResetAllColors()
        {
            foreach (var item in AllCells)
            {
                item.ResetColor();
            }
            //Debug.Log("Reset colors");
        }
        public void ColorAll(Color color)
        {
            foreach (var item in AllCells)
            {
                item.SetColor(color);
            }
        }

        private void ColorCheckerBoard(Color mainColor, Color secondaryColor)
        {
            bool useMainColor = true;
            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    GetCell(i, j).SetColor(useMainColor ? mainColor : secondaryColor);
                    useMainColor = !useMainColor;
                }
                useMainColor = !useMainColor;
            }
        }
    }
}