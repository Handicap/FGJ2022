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
        private TurnOwner currentTurnOwner = TurnOwner.Player;
        private GameManager instance;

        [SerializeField] private AudioClip[] songs;
        private AudioSource audioSource;

        [SerializeField] private Actors.BaseActor playerCharacter;

        public static List<TurnOwner> TurnOrder = new List<TurnOwner>
        {
            TurnOwner.Player,
            TurnOwner.Sheeple
        };

        private void Awake()
        {
            if (instance == null)
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
            audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            Input.InputManager.Instance.OnGridTargetChange += ShowPathTo;
        }

        private void ShowPathTo(Grid.GridCell target)
        {

            Grid.GridManager.Instance.ResetCellColors();
            if (playerCharacter == null) return;
            // slightly brutish
            List<Grid.GridCell> path = Grid.GridManager.Instance.FindPath(playerCharacter.Position, target, out bool success);
            if (success)
            {
                foreach (var item in path)
                {
                    item.SetColor(Color.cyan);
                }
            }
        }

        [Button]
        public void PassTurn()
        {
            int currentTurnIndex = TurnOrder.IndexOf(currentTurnOwner);
            int nextTurnIndex = currentTurnIndex + 1;
            if (currentTurnIndex == TurnOrder.Count)
            {
                nextTurnIndex = 0;
                Debug.Log("Hit last turn");
            }
        }

        public void PassTurn(TurnOwner nextOwner)
        {
            currentTurnOwner = nextOwner;
            Debug.Log("Passed turn to " + nextOwner.ToString());
        }

        [Button]
        public void Restart()
        {
            // yksi funktio kahdella rivillä
            UnityEngine.SceneManagement.SceneManager.LoadScene
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
