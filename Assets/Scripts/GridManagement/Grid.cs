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
                List<GridCell> cells = new List<GridCell>();
                for (int i = 0; i < cells.Count; i++)
                {
                    for (int j = 0; j < cells.Count; j++)
                    {
                        cells.Add(this.cells[i][j]);
                    }
                }
                return cells;
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

        // push new rows to the stack
        public void GenerateNewCells()
        {
            int startRow = cells.First().Count;
            for (int i = 0; i < Size.x; i++)
            {
                List<GridCell> column = cells[i];
                column.AddRange(CreateGridColumn(Size.x, gridPrefab));
            }
            CreateBlockers(new Vector2Int(0, startRow), new Vector2Int(Size.x, cells.First().Count));
            ConnectCells();
            SetCellTransforms();
        }

        private void ConnectCells()
        {
            GridCell northCell = null, southCell = null, westCell = null, eastCell = null;

            for (int i = 0; i < cells.Count; i++)
            {   
                for (int j = 0; j < cells[i].Count; j++)
                {
                    if (j > 0)
                    {
                        //Debug.Log("I have southern neighbour " + cells[i][j].name, cells[i][j]);
                        southCell = cells[i][j -1];
                    }
                    if (i > 0)
                    {
                        //Debug.Log("I have western neighbour " + cells[i][j].name, cells[i][j]);
                        westCell = cells[i - 1][j];
                    }
                    if (j < cells[i].Count - 1)
                    {
                        //Debug.Log("I have northern neighbour " + cells[i][j].name, cells[i][j]);
                        northCell = cells[i][j + 1];
                    }
                    if (i < cells.Count - 1)
                    {
                        //Debug.Log("I have eastern neighbour " + cells[i][j].name, cells[i][j]);
                        eastCell = cells[i + 1][j];
                    }
                    GetCell(i, j).Initialize(northCell, southCell, westCell, eastCell, new Vector2Int(i, j));
                }
            }
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


        public void GenerateAreas(Vector2Int startPoint, Vector2Int endPoint)
        {
            float seed = Random.Range(100f, 200f);
            CreateBlockers(startPoint, endPoint, seed: seed, threshhold: 0.70f);
            Debug.Log("Generated areas with seed " + seed);
        }

        private void CreateBlockers(Vector2Int startPosition, Vector2Int endPosition, float threshhold = 0.5f, float seed = 1f)
        {
            ColorCheckerBoard(Color.black, Color.black);

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