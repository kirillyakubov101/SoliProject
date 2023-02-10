using UnityEngine;

namespace Soli.Card
{
    public enum CardSuits
    {
        HEART,
        DIAMOND,
        SPADE,
        CLUBS,
        NONE
    }

    public enum CardValue
    {
        ACE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        J,
        Q,
        K
    }


    [CreateAssetMenu(fileName = "NewCard", menuName = "CreateCard/NewCard", order = 1)]
    public class Card : ScriptableObject
    {
        [SerializeField] private CardSuits _cardSuits;
        [SerializeField] private CardValue _cardValue;
        [SerializeField] private Sprite _sprite;

        private static Sprite _faceDownSprite = null;

        public CardSuits CardSuit { get => _cardSuits; }
        public CardValue CardValue { get => _cardValue; }

        public Sprite GetSprite()
        {
            return _sprite;
        }

        public static Sprite GetFaceDownSprite()
        {
            if(_faceDownSprite == null)
            {
               Texture2D cardBackText = (Texture2D)Resources.Load("card_back");
               _faceDownSprite =  Sprite.Create(cardBackText, new Rect(0f, 0f, 64f, 64f), new Vector2(0.5f,0.5f));
            }

            return _faceDownSprite;
        }

        public string GetCardSuitName()
        {
            return _cardSuits.ToString();
        }

        public string GetCardValueName()
        {
            return _cardValue.ToString();
        }


        public static bool IsOppositeSuitColor(CardSuits card_1, CardSuits card_2)
        {
            if((card_1 == CardSuits.HEART || card_1 == CardSuits.DIAMOND) && (card_2 == CardSuits.SPADE || card_2 == CardSuits.CLUBS))
            {
                return true;
            }
            else if((card_2 == CardSuits.HEART || card_2 == CardSuits.DIAMOND) && (card_1 == CardSuits.SPADE || card_1 == CardSuits.CLUBS))
            {
                return true;
            }

            return false;
           
        }

        public static bool IsValueSmaller(CardValue stackedCard, CardValue newCard)
        {
            return stackedCard - newCard == 1;
        }

        public static bool IsCardLargerByOne(CardValue stackedCard, CardValue newCard)
        {
            return newCard - stackedCard  == 1;
        }

        public static bool IsSameSuit(CardSuits stackedCard, CardSuits newCard)
        {
            return stackedCard == newCard;
        }

        public static bool IsKing(CardValue newCard)
        {
            return newCard == CardValue.K;
        }
    }

}

