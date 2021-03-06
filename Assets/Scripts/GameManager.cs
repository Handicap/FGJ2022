using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

namespace FGJ2022
{
    public enum TurnOwner
    {
        Player,
        Sheeple
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TurnOwner currentTurnOwner = TurnOwner.Player;
        private static GameManager instance;

        private Dictionary<TurnOwner, List<Actors.BaseActor>> actors = new Dictionary<TurnOwner, List<Actors.BaseActor>>();

        [SerializeField] private AudioClip[] songs;
        private AudioSource audioSource;

        [SerializeField] private Actors.BaseActor playerCharacter;
        [SerializeField] private TMPro.TextMeshProUGUI scoreTextObject; 

        public static List<TurnOwner> TurnOrder = new List<TurnOwner>
        {
            TurnOwner.Player,
            TurnOwner.Sheeple
        };

        [SerializeField] private Transform CameraTransform;
        [SerializeField] private float cameraMovementSpeed = 2f;
        [SerializeField] private AnimationCurve cameraMovementCurve;
        private Coroutine cameraMovementRoutine;

        private bool actionInProgress = false;

        public static GameManager Instance { get => instance; }

        private void Awake()
        {
            if (Instance == null)
            {
                instance = this;
            } else
            {
                Debug.LogError("Game manager already set");
                return;
            }
            Debug.Log("Started gamemanager", this);
        }

        private void Start()
        {
            IEnumerator DelayedStart()
            {
                yield return null;
                yield return new WaitForSeconds(1f);
                Debug.Log("asdf" + 1090);
                if (playerCharacter != null)
                {
                    MoveCameraTo(playerCharacter.Position);
                }
                Input.InputManager.Instance.OnGridTargetChange += ShowPathTo;
                Debug.Log("asdf" + 6);
                Input.InputManager.Instance.OnGridSelected += GridSelected;
                Debug.Log("asdf" + 7);

            }
            actors.Add(TurnOwner.Player, new List<Actors.BaseActor>());
            Debug.Log("asdf" + 1);
            actors[TurnOwner.Player].Add(playerCharacter);
            Debug.Log("asdf" + 2);
            actors.Add(TurnOwner.Sheeple, new List<Actors.BaseActor>());
            Debug.Log("asdf" + 3);
            audioSource = GetComponent<AudioSource>();
            Debug.Log("asdf" + 4);
            audioSource.Play();
            Debug.Log("asdf" + 5);
            StartCoroutine(DelayedStart());
        }

        
        private void MoveCameraTo(Actors.BaseActor actor, Grid.GridCell cell)
        {
            actor.OnMovementEnd -= MoveCameraTo;
            MoveCameraTo(cell);
        }
        private void MoveCameraTo(Grid.GridCell cell)
        {
            Vector3 startPos = CameraTransform.position;
            float phase = 0f;
            IEnumerator MovementRoutine()
            {
                while(phase < 1f)
                {
                    float curveSample = cameraMovementCurve.Evaluate(phase);
                    CameraTransform.position = Vector3.Lerp(startPos, cell.transform.position, curveSample);
                    phase += Time.unscaledDeltaTime * cameraMovementSpeed;
                    yield return null;
                }
                cameraMovementRoutine = null;
            }
            if (cameraMovementRoutine != null)
            {
                Debug.LogError("Multiple camera moves");
                return;
            }
            cameraMovementRoutine = StartCoroutine(MovementRoutine());
        }

        private void StartTurn(TurnOwner turnOwner)
        {
            currentTurnOwner = turnOwner;

            if (actors[currentTurnOwner].Count < 1)
            {
                PassTurn();
                return;
            }
            foreach (var item in actors[currentTurnOwner])
            {
                item.RefreshActionPoints();
            }
            if (turnOwner != TurnOwner.Player)
            {
                MoveNPCs();
            }
        }

        private void GridSelected(Grid.GridCell target)
        {
            if (currentTurnOwner == TurnOwner.Player && !actionInProgress)
            {
                Grid.GridManager.Instance.FindPath(playerCharacter.Position, target, out bool pathable);
                if (pathable)
                {
                    //playerCharacter.MoveTo(target);
                    playerCharacter.MoveTowards(target);
                    playerCharacter.OnMovementEnd += MovementEnded;
                    playerCharacter.OnMovementEnd += CheckIfHitEdge;
                    playerCharacter.OnMovementEnd += MoveCameraTo;
                    playerCharacter.OnMovementEnd += CheckForTurnPass;
                    actionInProgress = true;
                    Debug.Log("Moving player to " + target.Coordinate);
                } else
                {
                    Debug.Log("No path to " + target.Coordinate);
                }
            } else
            {
                Debug.Log("NPC turn");
            }
        }

        private void CheckIfHitEdge(Actors.BaseActor actor, Grid.GridCell cell)
        {
            Debug.Log("Checking if " + actor.name + " hit edge: " + cell.Type.ToString() + " " + cell.ToString());
            actor.OnMovementEnd -= CheckIfHitEdge;
            if (cell.Type == Grid.CellType.Edge)
            {
                List<Actors.BaseActor> newActors = new List<Actors.BaseActor>();
                Grid.GridManager.Instance.CreateNewAreas(out newActors);
                actors[TurnOwner.Sheeple].AddRange(newActors);
            }
        }

        private void MovementEnded(Actors.BaseActor actor, Grid.GridCell cell)
        {
            actor.OnMovementEnd -= MovementEnded;
            actionInProgress = false;
            Debug.Log("End of movement for " + actor.name);
            SetScoreDistance(cell.Coordinate.y.ToString());
        }

        private void ShowPathTo(Grid.GridCell target)
        {

            Grid.GridManager.Instance.ResetCellColors();
            if (playerCharacter == null) return;
            // slightly brutish
            List<Grid.GridCell> path = Grid.GridManager.Instance.FindPath(playerCharacter.Position, target, out bool success);
            if (success)
            {
                int stepsLeft = playerCharacter.MovementRange < path.Count ? playerCharacter.MovementRange : path.Count;
                for (int i = 0; i < stepsLeft; i++)
                {
                    path[i].SetColor(Color.cyan);
                }
            }
        }

        private void CheckForTurnPass()
        {
            if (actionInProgress)
            {
                return;
            }
            foreach (var item in actors[currentTurnOwner])
            {
                if (item.ActionPointsLeft > 0)
                {
                    Debug.Log("Can't pass turn: " + item.name + " has " + item.ActionPointsLeft + " AP left");
                    return;
                }
            }
            Debug.Log("All moves done for " + currentTurnOwner.ToString() + ", passing turn");
            PassTurn();
        }

        // If value is bigger than previous highscore value, update the score
        private void SetScoreDistance(string value)
        {
            int oldValue = 0;
            int newValue = 0;
            if (int.TryParse(scoreTextObject.text, out oldValue) && int.TryParse(value, out newValue) 
                    && oldValue < newValue )
            {
                scoreTextObject.text = value;
            }
        }
        private void CheckForTurnPass(Actors.BaseActor actor, Grid.GridCell cell)
        {
            Debug.Log("EndCheck");
            actor.OnMovementEnd -= CheckForTurnPass;
            CheckForTurnPass();
        }

        private void MoveNPCs()
        {
            IEnumerator MovementCoroutine()
            {
                List<Actors.BaseActor> actorsLeft = new List<Actors.BaseActor>(actors[TurnOwner.Sheeple]);
                foreach (var item in actors[TurnOwner.Sheeple])
                {
                    item.OnMovementEnd += MovementEnded;
                    //item.OnMovementEnd += CheckIfHitEdge;
                    item.OnMovementEnd += MoveCameraTo;
                    item.OnMovementEnd += CheckForTurnPass;
                }
                while(actorsLeft.Count > 0)
                {
                    if (!actionInProgress)
                    {
                        Actors.BaseActor nextActor = actorsLeft.First();
                        Grid.GridCell cell = nextActor.GetRandomDestination();
                        bool canReachDestination = nextActor.MoveTowards(cell);
                        if (canReachDestination)
                        {
                            actionInProgress = true;
                        }
                        actorsLeft.Remove(nextActor);
                        Debug.Log("Moving " + nextActor, nextActor);
                    } else
                    {
                        Debug.Log("Action in progress");
                    }
                    yield return null;
                }
                Debug.Log("Ended movementroutine");
                if (currentTurnOwner == TurnOwner.Sheeple)
                {
                    PassTurn();
                }
            }
            Debug.Log("Moving NPCs");
            StartCoroutine(MovementCoroutine());
        }

        [Button]
        public void PassTurn()
        {
            int currentTurnIndex = TurnOrder.IndexOf(currentTurnOwner);
            int nextTurnIndex = currentTurnIndex + 1;
            if (currentTurnIndex == TurnOrder.Count - 1)
            {
                nextTurnIndex = 0;
                Debug.Log("Hit last turn");
                
            }
            StartTurn(TurnOrder[nextTurnIndex]);
            Debug.Log("Passed turn to " + currentTurnOwner.ToString());
        }

        public void PassTurn(TurnOwner nextOwner)
        {
            currentTurnOwner = nextOwner;
            Debug.Log("Passed turn to " + nextOwner.ToString());
        }

        [Button]
        public void Restart()
        {
            // yksi funktio kahdella rivill?
            UnityEngine.SceneManagement.SceneManager.LoadScene
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void EndGame()
        {
            Debug.Log("Game over");
        }
    }
}
