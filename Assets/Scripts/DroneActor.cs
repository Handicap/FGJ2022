using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGJ2022.Grid;

namespace FGJ2022.Actors
{
    public class DroneActor : BaseActor
    {
        // Start is called before the first frame update
        void Start()
        {

            OnMovementStart += MovementStart;
            OnArrivedToCell += ArrivedToCell;
            OnLeftCell += LeftCell;
            OnActionStart += ActionStart;
            OnActionEnd += ActionEnd;
            // OnMovementStart += delegate { Debug.Log("Tapahtuu"); };
        }

        private void OnDestroy()
        {
            OnMovementStart -= MovementStart;
            OnArrivedToCell -= ArrivedToCell;
            OnLeftCell -= LeftCell;
            OnActionStart -= ActionStart;
            OnActionEnd -= ActionEnd;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void MovementStart(GridCell cell)
        {

        }
        void ArrivedToCell (GridCell cell) { }

        void LeftCell(GridCell cell) { }

        void ActionStart ()
        {

        }

        void ActionEnd() { }
    }
}