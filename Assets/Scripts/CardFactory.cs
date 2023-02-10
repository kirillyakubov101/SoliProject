using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Soli.Utils
{
    public class CardFactory : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_cards;
        [SerializeField] private GameObject[] m_shuffled = new GameObject[52];

        [SerializeField] private List<GameObject> list;

        private static CardFactory instance;
        private int currentCardIndex = 0;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public static CardFactory Instance()
        {
            if(instance == null)
            {
                instance = FindObjectOfType<CardFactory>();
            }

            return instance;
        }

        public IEnumerator Start()
        {
            m_cards = Resources.LoadAll<GameObject>("Cards/");
            list = m_cards.ToList();

            ShuffleCards();

            yield return null;
        }

        private void ShuffleCards()
        {
            int index;

            //52 cards in a deck
            for (int i = 0; i < 52; i++)
            {
                index = Random.Range(0, list.Count);
                m_shuffled[i] = list[index];
                list.Remove(list[index]);
            }

            list = null;
        }

        public GameObject TakeOneCard()
        {
            GameObject card = Instantiate(m_shuffled[currentCardIndex]);
            currentCardIndex++;

            return card;
        }
    }
}

