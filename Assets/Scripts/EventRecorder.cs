using Soli.Card;
using System.Collections.Generic;
using UnityEngine;
using Soli.Stack;

namespace Soli.Events
{
    public class EventRecorder : MonoBehaviour
    {
        private static EventRecorder instance;

        private Stack<Event> m_events = new Stack<Event>();
        private Event m_currentEvent = null;

        private const int MAX_REVERT_AMOUNT = 5;
        private static int s_revertAmount  = 0;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public static EventRecorder Instance { get { return instance; } }

        private bool CanRevert()
        {
            return s_revertAmount <= MAX_REVERT_AMOUNT;
        }

        //Stack to stack
        public void CreateEvent(IStackable previousStack, IStackable newStack,CardWrapper currentCard)
        {
            if (!CanRevert()) { return; }
            m_currentEvent = new Event(previousStack, newStack, currentCard);
            m_events.Push(m_currentEvent);   
        }

        //Deck to Stack
        public void CreateEvent(CardWrapper previewCard, CardWrapper currentCard)
        {
            if (!CanRevert()) { return; }
            m_currentEvent = new Event(previewCard, currentCard);
            m_events.Push(m_currentEvent);
        }

        public void RevertEvent()
        {
            if(m_events.Count <= 0 || !CanRevert()) { return; }

            Event revertedEvent = m_events.Pop();

            s_revertAmount++;

            if (revertedEvent.isFromDeck)
            {
                revertedEvent.PutCardBackInDeck();
            }
            else
            {
                revertedEvent.InitEvent();
            }
            
        }  
    }

}
