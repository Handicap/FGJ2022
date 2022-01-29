using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FGJ2022.Grid
{
    public enum CellPassability
    {
        Passable,
        Impassable
    }
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private GridCell northNeighbour, southNeighbour, westNeighbour, eastNeighbour;
        private CellPassability passability = CellPassability.Passable;
        [SerializeField] private Vector2Int coordinate;
        [SerializeField] private bool highlighted = false;
        [SerializeField] private GameObject highlightGraphic;


        private Color passableColor = new Color(0.40f, 0.75f, 0.65f);
        private Color impassableColor = new Color(0.70f, 0.20f, 0.30f);

        public CellPassability Passability { get => passability; 
            set
            {
                passability = value;
                SetColor(value == CellPassability.Passable ? passableColor : impassableColor);
            }
        }

        public bool Highlighted
        {
            get => highlighted;
            set
            {
                highlighted = value;
                highlightGraphic.SetActive(value);
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

        public void Initialize(GridCell northNeighbour, GridCell southNeighbour, GridCell westNeighbour, GridCell eastNeighbour, Vector2Int coordinate)
        {
            this.northNeighbour = northNeighbour;
            this.southNeighbour = southNeighbour;
            this.westNeighbour = westNeighbour;
            this.eastNeighbour = eastNeighbour;
            this.coordinate = coordinate;
            if (this.highlightGraphic == null)
                Debug.LogError("Cell can't find highlight graphic");
        }

        private void NeighbourLost(GridCell neighbour)
        {
            if (northNeighbour == neighbour) northNeighbour = null;
            if (southNeighbour == neighbour) southNeighbour = null;
            if (westNeighbour == neighbour) westNeighbour = null;
            if (eastNeighbour == neighbour) eastNeighbour = null;
            //Debug.Log(gameObject.name + " lost neighbour " + neighbour.name);
            //SetColor(Color.magenta);
        }

        public List<GridCell> GetAccessibleNeighbours()
        {
            List<GridCell> retList = new List<GridCell>();
            if (northNeighbour?.Passability == CellPassability.Passable) northNeighbour = null;
            if (southNeighbour?.Passability == CellPassability.Passable) southNeighbour = null;
            if (westNeighbour?.Passability == CellPassability.Passable) westNeighbour = null;
            if (eastNeighbour?.Passability == CellPassability.Passable) eastNeighbour = null;
            return retList;
        }

        public void SetColor(Color color)
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var item in renderers)
            {
                item.material.color = color;
            }
        }

        private void OnDestroy()
        {
            northNeighbour?.NeighbourLost(this);
            southNeighbour?.NeighbourLost(this);
            westNeighbour?.NeighbourLost(this);
            eastNeighbour?.NeighbourLost(this);
        }

        public override string ToString()
        {
            return coordinate.ToString();
        }

        private void OnMouseEnter()
        {
            this.Highlighted = true;
            northNeighbour.Highlighted = true;
            westNeighbour.Highlighted = true;   
            eastNeighbour.Highlighted = true;      
            southNeighbour.Highlighted = true; 
        }
        private void OnMouseExit()
        {
            this.Highlighted = false;
            northNeighbour.Highlighted = false;
            westNeighbour.Highlighted = false;
            eastNeighbour.Highlighted = false;
            southNeighbour.Highlighted = false;
        }
    }
}