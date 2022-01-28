using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGJ2022.Grid
{
    public class Grid
    {
        private List<GameObject> gridPointX = new List<GameObject>();
        private List<GameObject> gridPointY = new List<GameObject>();

        public Grid(List<GameObject> gridPointX, List<GameObject> gridPointY)
        {
            this.gridPointX = gridPointX;
            this.gridPointY = gridPointY;
        }
    }
}