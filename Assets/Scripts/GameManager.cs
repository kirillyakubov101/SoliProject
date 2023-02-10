using Soli.Card;
using Soli.Stack;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Soli.Utils
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private StackCreator m_stackCreator;
        [SerializeField] private Deck m_deck;

        [SerializeField] private EndStack[] m_endStacks;
        [SerializeField] private CardStack[] m_workStacks;

        [SerializeField] private GameObject m_autoWinBtn;
        [SerializeField] private Button m_restartBtn;
        [SerializeField] private GameObject winPanel;

        public static event Action<bool> OnGameReady;

        private int faceDownCards = 0;
        private int winningConditionsCount = 0;
        private int filledStacks = 0;

        private static GameManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private IEnumerator Start()
        {
            yield return CardFactory.Instance().Start();
            Run();
        }

        private async void Run()
        {
            await m_stackCreator.CreateStacks();
            await m_deck.CreateDeck();

            OnGameReady?.Invoke(true); //notify the Selector to recieve Input
            m_restartBtn.interactable = true;
        }

        private void OnEnable()
        {
            CardWrapper.OnFaceCardUp += HandleFaceUpCard;
            CardWrapper.OnFaceCardDown += HandleFaceDownCardAddition;
            Deck.OnDeckEmpty += HandleDeckEmpty;

        }

        private void OnDestroy()
        {
            CardWrapper.OnFaceCardUp -= HandleFaceUpCard;
            CardWrapper.OnFaceCardDown -= HandleFaceDownCardAddition;
            Deck.OnDeckEmpty -= HandleDeckEmpty;
        }


        private void HandleFaceDownCardAddition()
        {
            faceDownCards++;
        }

        private void HandleFaceUpCard()
        {
            faceDownCards--;
            if (faceDownCards == 0)
            {
                winningConditionsCount++;
                IsGameOver();
            }
        }

        private void HandleDeckEmpty()
        {
            winningConditionsCount++;
            IsGameOver();
        }

        private void IsGameOver()
        {
            if (winningConditionsCount == 2)
            {
                DisplayAutoWinBtn();
            }
        }

        private void DisplayAutoWinBtn()
        {
            m_autoWinBtn.SetActive(true);
        }

        private void WinGame()
        {
            m_autoWinBtn.GetComponent<Button>().enabled = false;
            DisableInput();

            foreach(var stack in m_endStacks)
            {
                stack.LaunchCard();
            }

            //winPanel.SetActive(true);
        }

        public CardStack[] GetCardsStacks()
        {
            return m_workStacks;
        }

        public static void DisableInput()
        {
            OnGameReady?.Invoke(false);
        }

        public static void EnableInput()
        {
            OnGameReady?.Invoke(true);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(0);
        }

        public void AutoWin()
        {
            foreach (var stack in m_workStacks)
            {
                if (stack.Cards.Count == 0) { continue; }

                foreach (var card in stack.Cards)
                {
                    AnimationHandler.Instance().MoveCardsToWin(card);
                    card.SetRenderPriority(card.EndRenderOrder);
                }
            }

            WinGame();
        }

        public void FillStack(bool isStackFull)
        {
            if (isStackFull)
            {
                filledStacks++;
                if (filledStacks == 4)
                {
                    WinGame();
                }
            }

            else
            {
                filledStacks--;
            }
        }

        public static GameManager Instance
        {
            get { return instance; }
            private set { }
        }

        public EndStack[] GetEndStacks()
        {
            return m_endStacks;
        }
    }
}
