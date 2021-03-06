using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGJ2022.Grid;
using System.Linq;

namespace FGJ2022.Actors
{
    public class NpcActor : BaseActor
    {
        
        // Start is called before the first frame update
        void Start()
        {
            OnArrivedToCell += delegate
            {
                if (CheckIfNextToPlayer())
                {
                    GameManager.Instance.EndGame();
                }
            };
            // OnMovementStart += delegate { Debug.Log("Tapahtuu"); };
        }

        /*
        private void OnDestroy()
        {
            OnMovementStart -= MovementStart;
            OnArrivedToCell -= ArrivedToCell;
            OnLeftCell -= LeftCell;
            OnActionStart -= ActionStart;
            OnActionEnd -= ActionEnd;
        }
        */

        public bool CheckIfNextToPlayer()
        {
            List<Grid.GridCell> neighbours = Position.GetAllNeighbours();
            foreach (GridCell item in neighbours)
            {
                if (item.Occupants.Any(x => x.Affiliation == ActorAffiliation.Player))
                {
                    Debug.LogError("NPC next to player!");
                    return true;
                }
            }
            if (Position.Occupants.Any(x => x.Affiliation == ActorAffiliation.Player)) return true;
            return false;
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