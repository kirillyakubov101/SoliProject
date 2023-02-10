using Soli.Card;
using Soli.Utils;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Soli.Events;

namespace Soli.Stack
{
    public class EndStack : MonoBehaviour, IStackable
    {
        public delegate void OnSuiteAssign(Transform pos,bool hasSuit,Card.CardSuits suit);

        public static event OnSuiteAssign OnSuiteAssignEvent;

        [SerializeField] private List<CardWrapper> m_cards = new List<CardWrapper>();
        [SerializeField] private Card.CardSuits m_currentSuit = CardSuits.NONE;

        private int currentRenderPriority = 100;
        private int endRenderPriority = 900;

        public CardSuits CurrentSuit { get => m_currentSuit;  }

        public bool TryAddCardToPile(CardWrapper newCard)
        {
            //if one exsists, do not add
            if (m_cards.Contains(newCard)) { return false; }

            //try to add first card as ACE
            if (m_cards.Count == 0 && m_currentSuit == CardSuits.NONE)
            {
                return AddFirstAce(newCard);
            }

            //if card follows the rules
            else if (IsCardFollowsRules(newCard))
            {
                AddCard(newCard);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The First Card DEFINES THE SUITE of the stack
        /// </summary>
        private bool AddFirstAce(CardWrapper newCard)
        {
            if(newCard.GetCard().CardValue == Card.CardValue.ACE)
            {
                this.m_currentSuit = newCard.GetCard().CardSuit;
                EndStack.OnSuiteAssignEvent?.Invoke(transform, true, this.m_currentSuit);
                AddCard(newCard);
                return true;
            }

            return false;
        }

        private bool AddFirstAceFromEvent(CardWrapper newCard)
        {
            if (newCard.GetCard().CardValue == Card.CardValue.ACE)
            {
                this.m_currentSuit = newCard.GetCard().CardSuit;
                EndStack.OnSuiteAssignEvent?.Invoke(transform, true, this.m_currentSuit);
                AddCardFromEvent(newCard);
                return true;
            }

            return false;
        }

        private bool IsCardFollowsRules(CardWrapper newCard)
        {
            CardWrapper lastCard = m_cards[m_cards.Count - 1];
            bool conditionVal = Card.Card.IsCardLargerByOne(lastCard.GetCard().CardValue, newCard.GetCard().CardValue);
            bool conditionSuit = Card.Card.IsSameSuit(lastCard.GetCard().CardSuit, newCard.GetCard().CardSuit);


            return conditionVal && conditionSuit;
        }

        private void AddCard(CardWrapper newCard)
        {
            if (m_cards.Count > 0)
            {
                //disable collider on the previous card
                m_cards[m_cards.Count - 1].DisableCollider();
            }

            //if the card already has a stack pile
            if (newCard.CurrentCardStack != null)
            {
                EventRecorder.Instance.CreateEvent(newCard.CurrentCardStack, this, newCard); // EVENT TEST

                newCard.CurrentCardStack.TryRemoveCardFromPile(newCard);
            }

            //assign this stack
            newCard.CurrentCardStack = this;

            //assign this card to this stack
            newCard.transform.parent = transform;

            //snap position
            newCard.ParentedPos = transform.position;

            //update sorting order
            newCard.SetRenderPriority(currentRenderPriority++);

            //add card to list
            m_cards.Add(newCard);

            //check if stack is full
            if(m_cards.Count == 13)
            {
                GameManager.Instance.FillStack(true);
            }

        }


        public void TryRemoveCardFromPile(CardWrapper newCard)
        {
            //check if stack WAS full, notify gamemanager
            if (m_cards.Count == 13)
            {
                GameManager.Instance.FillStack(false);
            }

            m_cards.Remove(newCard);

            if(m_cards.Count > 0)
            {
                //enable collider on the previous card
                m_cards[m_cards.Count - 1].EnableCollider();
            }

            //update sorting order
            currentRenderPriority--;

            //last card was removed
            if (m_cards.Count == 0)
            {
                m_currentSuit = CardSuits.NONE;
                EndStack.OnSuiteAssignEvent?.Invoke(transform, false, CardSuits.NONE);
            }
        }

        public void LaunchCard()
        {
            StartCoroutine(LaunchProcess());
        }

        private IEnumerator LaunchProcess()
        {
            foreach(var card in m_cards)
            {
                card.GetComponent<CardAnimation>().InitBounceCard(endRenderPriority++);
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void AddCardFromEvent(CardWrapper newCard)
        {
            if (m_cards.Count > 0)
            {
                //disable collider on the previous card
                m_cards[m_cards.Count - 1].DisableCollider();
            }

            //if the card already has a stack pile
            if (newCard.CurrentCardStack != null)
            {
                newCard.CurrentCardStack.TryRemoveCardFromPile(newCard);
            }

            //assign this stack
            newCard.CurrentCardStack = this;

            //assign this card to this stack
            newCard.transform.parent = transform;

            //snap position
            newCard.ParentedPos = transform.position;

            //update sorting order
            newCard.SetRenderPriority(currentRenderPriority++);

            //add card to list
            m_cards.Add(newCard);

            //check if stack is full
            if (m_cards.Count == 13)
            {
                GameManager.Instance.FillStack(true);
            }
        }

        public bool IsEmptyStack()
        {
            return m_cards.Count == 0;
        }

        public void AddCardToPileFromEvent(CardWrapper newCard)
        {
            //if one exsists, do not add
            if (m_cards.Contains(newCard)) { return; }


            //try to add first card as ACE
            if (m_cards.Count == 0 && m_currentSuit == CardSuits.NONE)
            {
                AddFirstAceFromEvent(newCard);
            }

            //if card follows the rules
            else if (IsCardFollowsRules(newCard))
            {
                AddCardFromEvent(newCard);
            }
        }
    }

}
