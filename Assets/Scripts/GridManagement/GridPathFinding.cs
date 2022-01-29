using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FGJ2022.Grid
{
    public class GridStep
    {
        private GridCell cameFrom;
        private GridCell ownTile;
        private int distanceTo;
        public GridStep(GridCell cameFrom, GridCell ownTile)
        {
            this.cameFrom = cameFrom;
            this.ownTile = ownTile;
        }

        public int DistanceTo { get => distanceTo; }
        public GridCell CameFrom { get => cameFrom; }
        public GridCell OwnTile { get => ownTile; }

        public void SetDistance(GridCell target)
        {
            distanceTo = Mathf.Abs(target.Coordinate.x - OwnTile.Coordinate.x) + Mathf.Abs(target.Coordinate.y - OwnTile.Coordinate.y);
        }
    }
    public static class GridPathFinding
    {
        public static List<GridCell> FindPath(GridCell from, GridCell to, Grid grid)
        {
            List<List<AStarSharp.Node>> pathNodes = new List<List<AStarSharp.Node>>();
            for (int i = 0; i < grid.Size.x; i++)
            {
                pathNodes.Add(new List<AStarSharp.Node>());
                for (int j = 0; j < grid.Size.y; j++)
                {
                    GridCell cell = grid.GetCell(i, j);
                    AStarSharp.Node newNode = new AStarSharp.Node(cell.Coordinate, cell.Passability == CellPassability.Passable);
                    pathNodes[i].Add(newNode);
                }
            }
            AStarSharp.Astar astar = new AStarSharp.Astar(pathNodes);
            Stack<AStarSharp.Node> astarPath = astar.FindPath(from.Coordinate, to.Coordinate);
            List<GridCell> path = new List<GridCell>();
            if (astarPath == null) return null;
            while (astarPath.Count > 0)
            {
                AStarSharp.Node node = astarPath.Pop();
                GridCell cell = grid.GetCell(node.Position);
                path.Add(cell);
            }
            return path;
        }

    }
}