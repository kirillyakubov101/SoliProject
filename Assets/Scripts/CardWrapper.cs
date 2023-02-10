using UnityEngine;
using Soli.Stack;
using System;

namespace Soli.Card
{ 
    public class CardWrapper : MonoBehaviour
    {
        [SerializeField] private Card m_card = null;
        [SerializeField] private SpriteRenderer m_spriteRenderer = null;
        [SerializeField] private BoxCollider2D boxCollider;

        public static event Action OnFaceCardDown;
        public static event Action OnFaceCardUp;

        private IStackable m_currentCardStack = null; //the stack it is attached to

        private int m_currentRenderOrder; //the render order of the card
        private Vector3 m_parentedPos = Vector3.zero;  //the position of the stack with/without offset
        private Sprite m_currentSprite; 
        private bool isRenderOrderUpdated = false;
        private Vector2 m_currentOffset, m_currentSize;

        //collider offset
        private Vector2 offsetAbtracted = new Vector2(-0.001784801f, 0.6845416f);
        private Vector2 sizeAbtracted = new Vector2(1.15404f,0.327f);

        //is face down card
        private bool isFaceDown = false;
        private bool isOriginallyFaceDown = false;

        //end stack pos with the same suit
        private Vector3 endPositionForAutoWin;
        private int endRenderOrder;

        public Vector3 ParentedPos { get => m_parentedPos; set => m_parentedPos = value; }
        public IStackable CurrentCardStack { get => m_currentCardStack; set => m_currentCardStack = value; }
        public bool IsFromDeck { get; set; } = false;
        public bool IsFaceDown { get => isFaceDown; }
        public Vector3 EndPositionForAutoWin { get => endPositionForAutoWin; }
        public int EndRenderOrder { get => endRenderOrder; }
        public bool IsOriginallyFaceDown { get => isOriginallyFaceDown; }



        //Nodes for Cards
        [SerializeField] private CardWrapper m_parentCard = null;
        [SerializeField] private CardWrapper m_childCard = null;

        private void Start()
        {
            m_currentRenderOrder = m_spriteRenderer.sortingOrder;
            m_parentedPos = transform.position;

            //cache normal colliders
            m_currentSize = boxCollider.size;
            m_currentOffset = boxCollider.offset;
        }

        private void OnEnable()
        {
            EndStack.OnSuiteAssignEvent += AssignEndStackForAutoWin;
        }

        private void OnDestroy()
        {
            EndStack.OnSuiteAssignEvent -= AssignEndStackForAutoWin;
        }

        private void AssignEndStackForAutoWin(Transform pos, bool hasSuit, CardSuits cardSuit)
        {
            if(hasSuit == false)
            {
                endPositionForAutoWin = Vector3.zero;
                return;
            }

            //if same suit than save pos
            if(cardSuit == this.m_card.CardSuit)
            {
                endPositionForAutoWin = pos.position;

                //making sure that the King will be the first card shown in the finish pile
                if(this.m_card.CardValue == CardValue.K)
                {
                    endRenderOrder = 999;
                }
                else
                {
                    endRenderOrder = m_spriteRenderer.sortingOrder;
                }
            }
           
        }

        /// <summary>
        /// This method is only on the endstack cards
        /// </summary>
        public void DisableCollider()
        {
            boxCollider.enabled = false;
        }

        public void EnableCollider()
        {
            boxCollider.enabled = true;
        }

        public void NotifyFaceDown()
        {
            isFaceDown = true;
            isOriginallyFaceDown = true;
            OnFaceCardDown?.Invoke();
        }

        public void NotifyFaceUp()
        {
            isFaceDown = false;
            OnFaceCardUp?.Invoke();
        }
        
        public void PutCardFaceDown()
        {
            m_currentSprite = Card.GetFaceDownSprite();
            m_spriteRenderer.sprite = m_currentSprite;
            gameObject.layer = 2; //default layer
        }

        public void PutCardFaceUp()
        {
            m_currentSprite = m_card.GetSprite();
            m_spriteRenderer.sprite = m_currentSprite;
            gameObject.layer = 7; //Card layer

        }

        public CardWrapper GetChildCard()
        {
            if(m_childCard == null)
            {
                return null;
            }
            else
            {
                return m_childCard;
            }
        }

        public Card GetCard()
        {
            return m_card;
        }

        public CardWrapper GetParentCard()
        {
            return m_parentCard;
        }

        public void SetParent(CardWrapper newParent)
        {
            this.m_parentCard = newParent;
        }

        public void SetChild(CardWrapper newChild)
        {
            this.m_childCard = newChild;
        }

        public void ResetCardPosition()
        {
            transform.position = m_parentedPos;
        }

        public void MinimizeCollider()
        {
            this.boxCollider.size = sizeAbtracted;
            this.boxCollider.offset = offsetAbtracted;
        }

        public void ResetCollider()
        {
            this.boxCollider.size = m_currentSize;
            this.boxCollider.offset = m_currentOffset;
        }

        public void ResetRenderPriority()
        {
            CardWrapper current = this;

            do
            {
                current.m_spriteRenderer.sortingOrder = current.m_currentRenderOrder;
                current = current.m_childCard;                                           //increment child

            } while (current != null);

            isRenderOrderUpdated = false;
        }

        //whlie dragged, the card and its children are the highest render order on the field
        public void SetHighestRenderPriority()
        {
            if (isRenderOrderUpdated) { return; }
            CardWrapper current = this;
            int highOrder = 600;

            do
            {
                current.m_currentRenderOrder = current.m_spriteRenderer.sortingOrder; //cache the previous value
                current.m_spriteRenderer.sortingOrder = highOrder++;                  //update value to the new high order value
                current = current.m_childCard;                                        //increment child

            } while(current != null);

            isRenderOrderUpdated = true;                                              //insure that this iteration is done only once for the parent and its children
        }

        //this function assign a specific render order
        //to the card
        public void SetRenderPriority(int amount)
        {
            m_currentRenderOrder = amount;
            m_spriteRenderer.sortingOrder = amount;
        }
    }
}


