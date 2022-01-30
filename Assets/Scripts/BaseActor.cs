using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGJ2022.Grid;
using System;
using NaughtyAttributes;
using System.Linq;

namespace FGJ2022.Actors
{
    public class BaseActor : MonoBehaviour
    {
        [SerializeField] private GridCell position;
        [SerializeField] private GridCell targetCell;

        [SerializeField] private float movementSpeed = 1f;

        public event Action<GridCell> OnMovementStart;
        public event Action<GridCell> OnMovementEnd;
        public event Action<GridCell> OnArrivedToCell;
        public event Action<GridCell> OnLeftCell;
        public event Action OnActionStart;
        public event Action OnActionEnd;

        private Stack<GridCell> currentPath = null;

        private Coroutine traversalStepRoutine = null;

        public bool MoveTo(GridCell target)
        {
            List<GridCell> path = GridManager.Instance.FindPath(position, target, out bool pathFound);
            if (!pathFound)
            {
                Debug.LogWarning("No path from " + position + " to " + target);
                return false;
            } else
            {
                foreach (GridCell item in path)
                {
                    item.SetColor(Color.cyan);
                }
            }
            path.Reverse();
            currentPath = new Stack<GridCell>();
            for (int i = 0; i < path.Count; i++)
            {
                currentPath.Push(path[i]);
            }
            currentPath.Push(position);
            TraversePath();
            return pathFound;
        }

        private void TraversePath()
        {
            Debug.Log("Starting traversal" + gameObject.name);
            OnMovementStart?.Invoke(position);
            Debug.Log("Path for traversal is " + string.Join(", ", currentPath));
            IEnumerator TraversalRoutine()
            {
                while(currentPath.Count > 1)
                {
                    if (traversalStepRoutine == null)
                    {
                        traversalStepRoutine = StartCoroutine(TraverseStep(currentPath.Pop(), currentPath.Peek()));
                    }
                    yield return null;
                }
            }
            StartCoroutine(TraversalRoutine());
            OnMovementEnd?.Invoke(position);
        }

        private IEnumerator TraverseStep(GridCell from, GridCell to)
        {
            OnLeftCell?.Invoke(from);
            float phase = 0f;
            while ( phase < 1f)
            {
                gameObject.transform.position = Vector3.Lerp(from.transform.position, to.transform.position, phase);
                phase += Time.deltaTime * movementSpeed;
                yield return null;
            }
            phase = 1f;
            gameObject.transform.position = to.transform.position;
            OnArrivedToCell?.Invoke(to);
            position = to;
            traversalStepRoutine = null;
        }

        [Button]
        public void SnapToGrid()
        {
            Ray downwardsRay = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(downwardsRay, out RaycastHit hitInfo))
            {
                Debug.Log("Hit " + hitInfo.collider.gameObject.name);
                GridCell cell = hitInfo.collider.gameObject.GetComponent<GridCell>();
                if (cell == null)
                {
                    Debug.LogError("Could not snap to grid, target did not have GridCell component");
                    return;
                }
                position = cell;
                transform.position = cell.transform.position;
                Debug.Log("snapped to " + cell);
            } else
            {
                Debug.LogError("Actor " + name + " was left free floating", gameObject);
            }
        }

        [Button]
        public void TestMoveTo()
        {
            if (!targetCell) { 
                Debug.LogError("Moving to null cell!");
                return; 
            }
            targetCell.SetColor(Color.yellow);
            position.SetColor(Color.yellow);
            MoveTo(targetCell);
        }

        private void Start()
        {
            SnapToGrid();
        }
    }
}