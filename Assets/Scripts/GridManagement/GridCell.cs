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

        public CellPassability Passability { get => passability; 
            set
            {
                passability = value;
                SetColor(value == CellPassability.Passable ? Color.green : Color.red);
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
    }
}