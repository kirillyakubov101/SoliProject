using Soli.Card;

namespace Soli.Stack
{
    public interface IStackable
    {
        public bool TryAddCardToPile(CardWrapper card);
        public void TryRemoveCardFromPile(CardWrapper card);
        public void AddCardToPileFromEvent(CardWrapper card);
    }
}


