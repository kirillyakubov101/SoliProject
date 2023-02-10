using UnityEngine;
using Soli.Controls;
using Soli.Utils;
using Soli.Audio;

namespace Soli.Core
{
    public class Selector : MonoBehaviour
    {
        private static Selector instance;

        [SerializeField] private LayerMask cardLayer = new LayerMask();
        [SerializeField] private LayerMask cardStackLayerMask = new LayerMask();
        [SerializeField] private Vector2 boxCastSize = new Vector2();
        [field:SerializeField] public bool IsBeingDragged { get; set; } = false;

        [SerializeField] private Draggable m_currentDraggedCard;

        private bool m_isMouseDragged = false;
        private bool isGameReady = false;


        public Draggable CurrentDraggedCard { get => m_currentDraggedCard; set => m_currentDraggedCard = value; }

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            InputControl.GetInstance().OnMouseDownEvent += MouseDownEventHander;
            InputControl.GetInstance().OnMouseUpEvent += MouseUpEventHandler;
            GameManager.OnGameReady += GameReadyHandler;
        }

        private void OnDestroy()
        {
            InputControl.GetInstance().OnMouseDownEvent -= MouseDownEventHander;
            InputControl.GetInstance().OnMouseUpEvent -= MouseUpEventHandler;
            GameManager.OnGameReady -= GameReadyHandler;
        }

        private void GameReadyHandler(bool val)
        {
            isGameReady = val;
        }

        private void Update()
        {
            if (!isGameReady) { return; }
            if (m_isMouseDragged && m_currentDraggedCard != null)
            {
                m_currentDraggedCard.UpdateCardPosition(InputControl.GetInstance().InputPosition);
            }
        }

        private void MouseDownEventHander(bool isDoubleTap)
        {
            if (!isGameReady) { return; }
            //if it's double tap

            if(m_currentDraggedCard != null) { return; }

            if (isDoubleTap)
            {
                SimpleRayCast(InputControl.GetInstance().InputPosition);
                if(m_currentDraggedCard == null) { return; }

                bool canBeAdded = AutoStack.Instance.AutoPutCard(m_currentDraggedCard.GetCardWrapper());
                if (canBeAdded)
                {
                    this.m_currentDraggedCard.StopDraggingCard();
                }

                m_currentDraggedCard = null;

            }
            //if it's just a press
            else
            {
                AdvancedRayCast(InputControl.GetInstance().InputPosition);
                m_isMouseDragged = true;
                if (m_currentDraggedCard != null && m_isMouseDragged)
                {
                    m_currentDraggedCard.DragCard();
                }
            }
        }

        private void MouseUpEventHandler()
        {
            if (!isGameReady) { return; }

            if (m_currentDraggedCard == null) { return; }

            DropCard();
        }

        private void DropCard()
        {   
           
            var hit = Physics2D.BoxCast(m_currentDraggedCard.transform.position, boxCastSize, 0f, Vector2.zero, Mathf.Infinity, cardStackLayerMask);


            //if it hit a stack of cards
            if (hit.collider != null)
            {
                //Task 1: try to append new card | it will consider a card placement only if it's available
                if (this.m_currentDraggedCard.TryAppendCardToStack(hit))
                {
                    //play sound of card being placed
                    AudioManager.Instance.Play_PlaceCardSound();

                    //Add Score
                    ScoreHandler.Instance.AddBaseScore();
                }
            }

            //Task 2: Reset position of the card
            this.m_currentDraggedCard.StopDraggingCard();

            m_currentDraggedCard = null;
            m_isMouseDragged = false;

        }

        private void AdvancedRayCast(Vector2 mousePos)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layerMask: cardLayer);

            if (hit.collider != null)
            {
                //if the clicked area is a card from a stack (endStack or WorkStack)
                if(hit.transform.TryGetComponent(out Draggable draggable))
                {
                    //do the action
                    this.m_currentDraggedCard = draggable;

                    //play sound
                    AudioManager.Instance.Play_TakeCardSound();
                }

                //if the clicked area is the Deck
                else if(hit.transform.TryGetComponent(out Deck deck))
                {
                    //do the action
                    deck.TakeCardFromDeck();

                    //if the deck still has cards
                    if (!deck.IsDeckEmpty())
                    {
                        //play sound
                        AudioManager.Instance.Play_ClickDeckSound();
                    }
                    
                }
            }
        }

        private void SimpleRayCast(Vector2 mousePos)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layerMask: cardLayer);

            if (hit.collider != null)
            {
                if (hit.transform.TryGetComponent(out Draggable draggable))
                {
                    //do the action
                    this.m_currentDraggedCard = draggable;

                }
            }

        }
    }
}

