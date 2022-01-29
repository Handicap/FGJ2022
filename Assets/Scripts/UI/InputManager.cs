using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FGJ2022.Input
{
    public class InputManager : MonoBehaviour
    {
        public event Action<GameObject> OnClick;
        private InputManager instance;
        [SerializeField] private List<GameObject> currentTargets = new List<GameObject>();
        [SerializeField] private GameObject actorSelectorGraphic;
        [SerializeField] private GameObject actorSelectionGraphic;
        [SerializeField] private GameObject gridSelectorGraphic;
        [SerializeField] private GameObject gridSelectionGraphic;
        [SerializeField] private GameObject currentSelection;

        public event Action<Grid.GridCell> OnGridHit;
        public event Action<Actors.BaseActor> OnActorHit;

        public static readonly string ACTOR_LAYYER = "Actors";
        public static readonly string GRID_LAYER = "Grid";

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            } else
            {
                DestroyImmediate(this);
            }
            actorSelectionGraphic.SetActive(false);
            actorSelectorGraphic.SetActive(false);
            gridSelectionGraphic.SetActive(false);
            gridSelectorGraphic.SetActive(false);
        }

        private void Update()
        {
            TestMouse();
            if (UnityEngine.Input.GetMouseButtonDown(0) && currentTargets.Count > 0)
            {
                OnClick?.Invoke(currentTargets[0]);
                List<string> targets = new List<string>();
                for (int i = 0; i < currentTargets.Count; i++)
                {
                    targets.Add(currentTargets[i].name);
                }
                Debug.Log("Clicked: " + string.Join(", ", targets));
            } else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                OnMouseRelease();
            }
        }

        private void TestMouse()
        {
            // raycast and see if we hit something with priority
            Ray mouseRay = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                Actors.BaseActor actorHit = hitObject.GetComponent<Actors.BaseActor>();
                if (actorHit != null)
                {
                    Debug.Log("Hit an actor! boooyeeyy");
                }

                Grid.GridCell cellHit = hitObject.GetComponent<Grid.GridCell>();
                if (cellHit != null)
                {
                    Debug.Log("Hit a cell! hooray");
                }
            }
            List<GameObject> targets = new List<GameObject>();
            currentTargets = targets;
        }

        private void SetGridSelector(bool active, GameObject gameObject)
        {
            gridSelectorGraphic.SetActive(active);
            gridSelectorGraphic.transform.position = gameObject.transform.position;
        }

        private void SetActorSelector(bool active, GameObject target)
        {
            actorSelectorGraphic.SetActive(active);
            actorSelectorGraphic.transform.position = target.transform.position;
        }
        //TODO: maybe tween from->to
        private void InterpolateGraphics()
        {

        }

        private void OnMouseRelease()
        {
            //Debug.Log("MouseUp");
        }
    }
}
