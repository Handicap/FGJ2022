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
            List<GameObject> targets = new List<GameObject>();
            List<string> targetStrings = new List<string>();
            for (int i = 0; i < LayerPriority.Count; i++)
            {
                if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, LayerPriority[i], QueryTriggerInteraction.UseGlobal))
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    targets.Add(hitObject);
                    targetStrings.Add(hitInfo.collider.gameObject.name);
                    // this is fucking stupid
                    if (hitObject.GetComponent<Grid.GridCell>())
                    {
                        gridSelectorGraphic.transform.position = hitObject.transform.position;
                    } else if (hitObject.GetComponent<Actors.BaseActor>())
                    {
                        actorSelectorGraphic.transform.position = hitObject.transform.position;
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
