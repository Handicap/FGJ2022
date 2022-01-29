using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FGJ2022.Grid
{
    public class Grid
    {
        private List<List<GridCell>> cells = new List<List<GridCell>>();

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
            GameObject baseObject = new GameObject();
            baseObject.name = "Grid-" + xDimension + "-" + yDimension;
            if (parent != null)
            {
                baseObject.transform.SetParent(parent);
            }
            Vector2 cellSize = gridPrefab.Dimensions;
            for (int x = 0; x < xDimension; x++)
            {
                cells.Add(new List<GridCell>());
                for (int y = 0; y < yDimension; y++)
                {
                    GridCell newCell = GameObject.Instantiate(gridPrefab);
                    newCell.transform.position = new Vector3(cellSize.x * x, 0, y);
                    newCell.name = "Cell-" + x + "-" + y;
                    cells[x].Add(newCell);
                    newCell.transform.SetParent(baseObject.transform);
                }
            }
            Color mainColor = Color.red;
            Color secondaryColor = Color.black;
            ColorCheckerBoard(mainColor, secondaryColor);
            ConnectCells();
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
            float seed = Random.Range(0f, 100f);
            CreateBlockers(8, 8, seed: seed);
            Debug.Log("Generated areas with seed " + seed);
        }

        private void CreateBlockers(float perlinX, float perlinY, float threshhold = 0.5f, float seed = 1f)
        {
            ColorCheckerBoard(Color.black, Color.black);
            for (int i = 0; i < cells.Count; i++)
            {
                float iFactor = (float) i / (float)cells.Count;
                for (int j = 0; j < cells[i].Count; j++)
                {
                    float jFactor = (float)j / (float)cells.Count;
                    float perlinSample = Mathf.PerlinNoise(iFactor * seed, jFactor * seed);
                    if (perlinSample > threshhold)
                    {
                        //Debug.Log("Perlin hit " + perlinSample, cells[i][j]);
                        //cells[i][j].SetColor(Color.blue);
                        GetCell(i, j).Passability = CellPassability.Impassable;
                    } else
                    {
                        //Debug.Log("Perlin miss " + perlinSample, cells[i][j]);
                        GetCell(i, j).Passability = CellPassability.Passable;
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