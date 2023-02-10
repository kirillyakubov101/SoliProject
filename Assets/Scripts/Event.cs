using Soli.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soli.Stack;
using Soli.Utils;

namespace Soli.Events
{
    public class Event
    {
        public IStackable prevStack;
        public IStackable newStack;
        public CardWrapper currentCard;

        //Deck Events
        public bool isFromDeck = false;
        public CardWrapper previewCard;

        //for stacks
        public Event(IStackable prevStack, IStackable newStack, CardWrapper currentCard)
        {
            this.prevStack = prevStack;
            this.newStack = newStack;
            this.currentCard = currentCard;
        }

        //for deck
        public Event(CardWrapper previewCard, CardWrapper currentCard, bool isFromDeck = true)
        {
            this.currentCard = currentCard;
            this.isFromDeck = isFromDeck;
            this.previewCard = previewCard;
        }

        public void InitEvent()
        {
            prevStack.AddCardToPileFromEvent(currentCard);
            currentCard.ResetCardPosition();
        }

        public void PutCardBackInDeck()
        {
            Deck.Instance().RevertCardToDeck(previewCard,currentCard);
        }

      

        
    }

}
