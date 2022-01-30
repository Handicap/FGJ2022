using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace FGJ2022.Input
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;
        [SerializeField] private List<Grid.GridCell> currentTargets = new List<Grid.GridCell>();
        [SerializeField] private GameObject actorSelectorGraphic;
        [SerializeField] private GameObject actorSelectionGraphic;
        [SerializeField] private GameObject gridSelectorGraphic;
        [SerializeField] private GameObject gridSelectionGraphic;
        [SerializeField] private GameObject currentSelection;

        public event Action<Grid.GridCell> OnGridSelected;
        public event Action<Actors.BaseActor> OnActorSelected;
        public event Action<Actors.BaseActor> OnActorTargetChange;
        public event Action<Grid.GridCell> OnGridTargetChange;

        public static InputManager Instance => instance;


        private Dictionary<string, int> layerValues = new Dictionary<string, int>();

        // raycast in this order
        private static List<LayerMask> LayerPriority = new List<LayerMask>();

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            } else
            {
                DestroyImmediate(this);
            }
            LayerPriority.Add(LayerMask.GetMask("Actors"));
            LayerPriority.Add(LayerMask.GetMask("Grid"));
        }

        private void Update()
        {
            TestMouse();
            if (UnityEngine.Input.GetMouseButtonDown(0) && currentTargets.Count > 0)
            {
                //OnClick?.Invoke(currentTargets[0]);
                List<string> targets = new List<string>();
                for (int i = 0; i < currentTargets.Count; i++)
                {
                    targets.Add(currentTargets[i].name);
                }
                OnGridSelected?.Invoke(currentTargets.First());
            } else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                OnMouseRelease();
            }
        }

        private void TestMouse()
        {
            // raycast and see if we hit something with priority
            Ray mouseRay = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            List<Grid.GridCell> targets = new List<Grid.GridCell>();
            List<string> targetStrings = new List<string>();
            for (int i = 0; i < LayerPriority.Count; i++)
            {
                if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, LayerPriority[i], QueryTriggerInteraction.UseGlobal))
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    Grid.GridCell cell = hitObject.GetComponent<Grid.GridCell>();
                    if (cell != null && !currentTargets.Contains(cell))
                    {
                        targets.Add(cell);
                        targetStrings.Add(hitInfo.collider.gameObject.name);
                        gridSelectorGraphic.transform.position = hitObject.transform.position;
                        OnGridTargetChange?.Invoke(cell);
                        if (cell.Occupants.Count > 0)
                        {
                            OnActorTargetChange?.Invoke(cell.Occupants[0]);
                        }
                    }
                }
            }
            currentTargets = targets;
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
