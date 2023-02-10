using Soli.Card;
using Soli.Stack;
using Soli.Utils;
using UnityEngine;

namespace Soli.Core
{
    public class Draggable : MonoBehaviour
    {
        [SerializeField] private CardWrapper m_cardWrapper;

        public void UpdateCardPosition(Vector2 newPos)
        {
            transform.position = newPos;
        }

        public void StopDraggingCard()
        {
            m_cardWrapper.ResetCardPosition();
            m_cardWrapper.ResetRenderPriority();
        }

        //displays the dragged cards as the highest render order
        public void DragCard()
        {
            m_cardWrapper.SetHighestRenderPriority();
        }

        /// <summary>
        /// Tries to add the dragged card to the stack it has hit
        /// </summary>
        /// <param name="hit"></param>
        public bool TryAppendCardToStack(RaycastHit2D hit)
        {
            if (hit.transform.TryGetComponent(out IStackable cardStack))
            {
                if (cardStack.TryAddCardToPile(this.m_cardWrapper))
                {
                    if (m_cardWrapper.IsFromDeck)
                    {
                        Deck.Instance().ClearLastCardRef();
                        m_cardWrapper.IsFromDeck = false;
                    }

                    return true;
                }
            }

            return false;
        }

        public CardWrapper GetCardWrapper()
        {
            return m_cardWrapper;
        }
    }

}
