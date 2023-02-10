using System.Threading.Tasks;
using UnityEngine;
using Soli.Stack;
using System.Collections.Generic;
using Soli.Card;

namespace Soli.Utils
{
    public class StackCreator : MonoBehaviour
    {
        private const int C_MaxWorkStaks = 7;
        private int numberOfCardsForStack = 7;

        [SerializeField] private CardStack[] m_workStacks = new CardStack[C_MaxWorkStaks];

        public async Task CreateStacks()
        {
            foreach(CardStack stack in m_workStacks)
            {
                PopulateStack(stack);
                await Task.Delay(100);
            }

            await Task.Delay(100);
        }

        private void PopulateStack(CardStack cardStack)
        {
            List<CardWrapper> listOfCards = new List<CardWrapper>();

            for(int i = 0; i < numberOfCardsForStack; i++)
            {
                GameObject newCardGameObject = CardFactory.Instance().TakeOneCard();
                CardWrapper newCard = newCardGameObject.GetComponent<CardWrapper>();
                listOfCards.Add(newCard);
            }

            cardStack.AppendStartingCards(listOfCards);

            numberOfCardsForStack--;
        }


    }

}
