using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGJ2022.Actors;

namespace FGJ2022.Grid
{
    public enum CellPassability
    {
        Passable,
        Impassable
    } 
    public enum CellType
    {
        Default,
        Edge
    }

    public enum Direction
    {
        North,
        South,
        East,
        West
    }

    public class GridCell : MonoBehaviour
    {
        private Dictionary<Direction, GridCell> neighbours = new Dictionary<Direction, GridCell>
        {
            { Direction.North, null },
            { Direction.South, null },
            { Direction.East, null },
            { Direction.West, null }
        };
        //[SerializeField] private GridCell northNeighbour, southNeighbour, westNeighbour, eastNeighbour;
        [SerializeField] private CellPassability passability = CellPassability.Passable;
        [SerializeField] private CellType type = CellType.Default;
        [SerializeField] private Vector2Int coordinate;
        [SerializeField] private bool highlighted = false;
        [SerializeField] private GameObject highlightGraphic;

        [SerializeField] private List<Actors.BaseActor> occupants = new List<Actors.BaseActor>();


        private Color passableColor = new Color(0.40f, 0.75f, 0.65f);
        private Color impassableColor = new Color(0.70f, 0.20f, 0.30f);
        private Color edgeColor = new Color(0.70f, 0.80f, 0.30f);

        private void Start()
        {
           if (passability == CellPassability.Impassable)
                GetComponent<BuildingGraphicsInstancer>().SetBuildingVisibility(true);
        }

        public CellPassability Passability { get => passability; 
            set
            {
                passability = value;
                SetColor(value == CellPassability.Passable ? passableColor : impassableColor);
                GetComponent<BuildingGraphicsInstancer>().SetBuildingVisibility(value == CellPassability.Impassable);
            }
        }

        public bool Highlighted
        {
            get => highlighted;
            set
            {
                highlighted = value;
                highlightGraphic.SetActive(value);
                if (!value)
                    ResetColor();
            }
        }
        public Vector2 Dimensions
        {
            get
            {
                MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
                Bounds bounds = new Bounds();
                foreach (var item in renderers)
                {
                    bounds.Encapsulate(item.bounds);
                }
                return new Vector2(bounds.size.x, bounds.size.z);
            }
        }

        public Vector2Int Coordinate { get => coordinate; }
        public List<BaseActor> Occupants { get => occupants; }

        public void Initialize(Vector2Int coordinate)
        {
            this.coordinate = coordinate;
            if (this.highlightGraphic == null)
                Debug.LogError("Cell can't find highlight graphic");
        }

        public GridCell GetNeighbour(Direction dir)
        {
            neighbours.TryGetValue(dir, out GridCell value);
            return value;
        }

        public List<GridCell> GetAllNeighbours()
        {
            List<GridCell> activeNeighbours = new List<GridCell> {
                neighbours[Direction.North],
                neighbours[Direction.South],
                neighbours[Direction.West],
                neighbours[Direction.East]
            };
            activeNeighbours.RemoveAll(x => x == null);
            return activeNeighbours;
        }

        public GridCell GetRandomNeighbour(GridCell exclude = null)
        {
            List<GridCell> validCells = new List<GridCell>(neighbours.Values);
            foreach (var item in neighbours.Values)
            {
                if (item == null) validCells.Remove(item);
            }
            if (exclude != null)
            {
                validCells.Remove(exclude);
            }
            if (validCells.Count == 0) return null;
            return validCells[UnityEngine.Random.Range(0, validCells.Count - 1)];
        }

        private void NeighbourLost(GridCell neighbour)
        {
            /*
            if (NorthNeighbour == neighbour) NorthNeighbour = null;
            if (SouthNeighbour == neighbour) SouthNeighbour = null;
            if (WestNeighbour == neighbour) WestNeighbour = null;
            if (EastNeighbour == neighbour) EastNeighbour = null;
            //Debug.Log(gameObject.name + " lost neighbour " + neighbour.name);
            //SetColor(Color.magenta);
            */
        }

        public void SetNeighbour(Direction dir, GridCell cell)
        {
            neighbours[dir] = cell;
        }

        public int NeighbourCount
        {
            get
            {
                int count = 0;
                foreach (var item in neighbours)
                {
                    if (item.Value != null) count++;
                }
                return count;
            }
        }

        public CellType Type { get => type; set => type = value; }

        public void SetColor(Color color)
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var item in renderers)
            {
                if(item.tag == "ColorChangingPart")
                    item.material.color = color;
            }
            //Debug.Log("Set color to " + color, gameObject);
        }

        public void SetHighlightColor(Color color)
        {
            MeshRenderer[] renderers = highlightGraphic.GetComponents<MeshRenderer>();
            foreach (var item in renderers)
            {
                if (item.tag == "ColorChangingPart")
                    item.material.color = color;
            }
        }

        public void ResetColor()
        {
            Color color = passability == CellPassability.Passable ? passableColor : impassableColor;
            color = Type == CellType.Default ? color : edgeColor;
            SetColor(color);
        }

        private void SetHighlighted(bool highlighted)
        {
            Highlighted = highlighted;
            // Highlighted getter resets colors automatically
        }

        private void SetHighlighted(bool highlighted, Color color)
        {
            Highlighted = highlighted;
            if(highlighted)
                SetColor(color);
            // Highlighted getter resets colors automatically
        }

        private void SetNeighborsHighlightedRecursively(bool highlighted, int iterations)
        {
            Highlighted = highlighted;
            // Highlighted getter resets colors automatically

            if (iterations <= 0) return;
            //NorthNeighbour?.SetNeighborsHighlightedRecursively(highlighted, iterations - 1);
            //WestNeighbour?.SetNeighborsHighlightedRecursively(highlighted, iterations - 1);
            //EastNeighbour?.SetNeighborsHighlightedRecursively(highlighted, iterations - 1);
            //SouthNeighbour?.SetNeighborsHighlightedRecursively(highlighted, iterations - 1);
        }

        // Sets cell and neighboring cells to selected color, or if highlighted is false, to original color
        private void SetNeighborsHighlightedRecursively(bool highlighted, Color color, int iterations)
        {
            this.Highlighted = highlighted;

            if(highlighted)
                SetHighlightColor(color);
            // Highlighted getter resets colors automatically

            if (iterations <= 0) return;
            /*
            NorthNeighbour?.SetNeighborsHighlightedRecursively(highlighted, color, iterations - 1);
            WestNeighbour?.SetNeighborsHighlightedRecursively(highlighted, color, iterations - 1);
            EastNeighbour?.SetNeighborsHighlightedRecursively(highlighted, color, iterations - 1);
            SouthNeighbour?.SetNeighborsHighlightedRecursively(highlighted, color, iterations - 1);
            */
        }

        private void OnDestroy()
        {
            foreach (var item in neighbours)
            {
                if (item.Value != null) item.Value.NeighbourLost(this);
            }
        }

        public override string ToString()
        {
            return coordinate.ToString();
        }
        /*
        private void OnMouseEnter()
        {
            //SetNeighborsHighlightedRecursively(true, Color.yellow, 2);
            SetHighlighted(true, Color.cyan);
        }
        private void OnMouseExit()
        {
            SetHighlighted(false, Color.white);
            //SetNeighborsHighlightedRecursively(false, Color.white, 2);
        }
        */
    }
}