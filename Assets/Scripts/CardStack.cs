using System.Collections.Generic;
using UnityEngine;
using Soli.Card;
using System;
using Soli.Events;

namespace Soli.Stack
{
    public class CardStack : MonoBehaviour,IStackable
    {
        [SerializeField] private List<CardWrapper> m_cards = new List<CardWrapper> ();
        [SerializeField] private BoxCollider2D boxCollider;

        private Vector3 offset = Vector3.zero;
        private const float c_offsetY = 0.4f;
        private int currentRenderPriority = 101;

        //event to notify the GameManager that a card was added to the stack
        //he will determine if the bottom border should be adjusted
        public static event Action<CardStack,int> OnCardModified;

        public List<CardWrapper> Cards { get => m_cards; }

        public void AppendStartingCards(List<CardWrapper> listOfCards)
        {
            m_cards = listOfCards;

            for (int i = 0; i < m_cards.Count; i++)
            {
                if(i != m_cards.Count - 1)
                {
                    m_cards[i].PutCardFaceDown();
                    m_cards[i].NotifyFaceDown();
                }
                else
                {
                    m_cards[i].PutCardFaceUp();
                }
               
                BruteAddCardToPile(m_cards[i],i);
                
            }

            //event to notify gamemanager
            OnCardModified.Invoke(this, m_cards.Count);
        }

        /// <summary>
        /// This is done to start game initialize the stack with the manually assigned size of the stack with cards
        /// </summary>
        /// <param name="newCard"></param>
        /// <param name="index"></param>
        private void BruteAddCardToPile(CardWrapper newCard,int index)
        {
            //Make a connection from the last card to to first card of the single of multiple cards
            if (index > 0)
            {
                CardWrapper lastCard = m_cards[index - 1];

                //assing child
                lastCard.SetChild(newCard);
                newCard.transform.parent = lastCard.gameObject.transform;

                //assign parent
                newCard.SetParent(lastCard);
            }

            //if first index
            else if (index == 0)
            {
                newCard.transform.parent = transform;
            }

            newCard.ParentedPos = transform.position + offset;
            newCard.transform.position = newCard.ParentedPos;

            //assign this stack
            newCard.CurrentCardStack = this;

            //Snap position
            newCard.ParentedPos = transform.position + offset;
            offset.y -= c_offsetY;

            boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - 0.2f);
            boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y + 0.3f);

            //update sorting order
            newCard.SetRenderPriority(currentRenderPriority++);

        }

        public bool TryAddCardToPile(CardWrapper newCard)
        {
            //if one exsists, do not add
            if (m_cards.Contains(newCard)) { return false; }

            //if card follows the rules
            if (!IsCardFollowsTheRusles(newCard)) { return false; }


            //if the card already has a stack pile
            if (newCard.CurrentCardStack != null)
            {
                IStackable prev = newCard.CurrentCardStack;

                prev.TryRemoveCardFromPile(newCard);

                EventRecorder.Instance.CreateEvent(prev, this, newCard); // EVENT TEST
            }

            //add the card and all of its children if exist
            AddingNewCards(newCard);

            return true;
        }

        private bool IsCardFollowsTheRusles(CardWrapper newCard)
        {
            if (m_cards.Count == 0)
            {
                if (Card.Card.IsKing(newCard.GetCard().CardValue)) { return true; }
                else { return false; }
            }
              

            Card.Card lastCard =  m_cards[m_cards.Count - 1].GetCard();
            Card.Card checkedCard = newCard.GetCard();

            
            if (Card.Card.IsOppositeSuitColor(lastCard.CardSuit, checkedCard.CardSuit) &&
                        Card.Card.IsValueSmaller(stackedCard: lastCard.CardValue,newCard: checkedCard.CardValue))
            {
                return true;
            }

            return false;
        }

        private void AddingNewCards(CardWrapper newCard)
        {
            //Make a connection from the last card to to first card of the single of multiple cards
            if (m_cards.Count > 0)
            {
                CardWrapper lastCard = m_cards[m_cards.Count - 1];

                //assign child
                lastCard.SetChild(newCard);
                lastCard.MinimizeCollider();
                newCard.transform.parent = lastCard.gameObject.transform;

                //assign parent
                newCard.SetParent(lastCard);

                //TODO:REMOVE THIS 
                //////if the added card is from the deck
                //if (newCard.IsFromDeck)
                //{
                //    print("this was not called");
                //    newCard.IsFromDeck = false;
                //    Deck.Instance().ClearLastCardRef();
                //}
            }

            CardWrapper currentCard = newCard;

            do
            {
                //if the stack pile is empty
                if(m_cards.Count == 0)
                {
                    newCard.transform.parent = transform;
                }
                else
                {
                    currentCard.ParentedPos = transform.position + offset;
                }

                //new card to the list
                m_cards.Add(currentCard);

                //assign this stack
                currentCard.CurrentCardStack = this;

                //Snap position
                currentCard.ParentedPos = transform.position + offset;
                offset.y -= c_offsetY;

                boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - 0.2f);
                boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y + 0.3f);

                //update sorting order
                currentCard.SetRenderPriority(currentRenderPriority++);
                
                if(currentCard.GetChildCard() != null)
                {
                    //update each card box collider
                    currentCard.MinimizeCollider();
                }

                //increment to next child
                currentCard = currentCard.GetChildCard();

            } while (currentCard!= null);

            //event to notify gamemanager
            OnCardModified.Invoke(this, m_cards.Count);

        }
      
        public void TryRemoveCardFromPile(CardWrapper cardToRemove)
        {
            if(m_cards.Count == 0) { return; }

            //Remove a connection from the last card to to first card of the single of multiple cards
            if (cardToRemove.GetParentCard() != null)
            {
                CardWrapper lastCard = cardToRemove.GetParentCard();
               
                lastCard.SetChild(null);
                lastCard.PutCardFaceUp();
                lastCard.ResetCollider();
                if (lastCard.IsFaceDown)
                {
                    lastCard.NotifyFaceUp();
                }
            }

            cardToRemove.SetParent(null);

            CardWrapper currentCard = cardToRemove;

            do
            {
                currentCard.ParentedPos = Vector3.zero;

                //new card to the list
                m_cards.Remove(currentCard);

                //update sorting order
                currentRenderPriority--;
                offset.y += c_offsetY;

                boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y + 0.2f);
                boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y - 0.3f);

                //increment to next child
                currentCard = currentCard.GetChildCard();



            } while (currentCard != null);

            //event to notify gamemanager
            OnCardModified.Invoke(this, m_cards.Count);
        }


        public Vector3 LastCardPos()
        {
            if(m_cards.Count == 0) { return Vector3.zero; }
            return m_cards[m_cards.Count - 1].transform.position;
        }

        public int GetCardsCount()
        {
            return m_cards.Count;
        }

        public void AddCardToPileFromEvent(CardWrapper newCard)
        {
            //if one exsists, do not add
            if (m_cards.Contains(newCard)) { return; }

            //if the card already has a stack pile
            if (newCard.CurrentCardStack != null)
            {
                newCard.CurrentCardStack.TryRemoveCardFromPile(newCard);
                
            }

            if(m_cards.Count != 0)
            {
                CardWrapper lastCard = m_cards[m_cards.Count - 1];

                //if the last card was faceDOWN before
                if (lastCard != null && lastCard.IsOriginallyFaceDown)
                {
                    lastCard.PutCardFaceDown();
                }
            }

            //add the card and all of its children if exist
            AddingNewCards(newCard);

            //event to notify gamemanager
            OnCardModified.Invoke(this, m_cards.Count);
        }
    }
}

