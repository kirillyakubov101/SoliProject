using Soli.Card;
using Soli.Stack;
using UnityEngine;

namespace Soli.Utils
{
    public class AutoStack : MonoBehaviour
    {
        private static AutoStack instance;
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

        public static AutoStack Instance { get { return instance; } }

        public bool AutoPutCard(CardWrapper card)
        {
            foreach (var stack in GameManager.Instance.GetEndStacks())
            {
                if (stack.TryAddCardToPile(card))
                {
                    if (card.IsFromDeck)
                    {
                        Deck.Instance().ClearLastCardRef();
                        card.IsFromDeck = false;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}

