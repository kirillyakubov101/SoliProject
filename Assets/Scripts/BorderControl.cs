using Soli.Stack;
using Soli.Utils;
using UnityEngine;

namespace Soli.Border
{
    public class BorderControl : MonoBehaviour
    {
        [SerializeField] private GameManager m_gameManager;

        private CardStack[] m_workStacks;

        private void OnEnable()
        {
            CardStack.OnCardModified += UpdateDictionary;
        }

        private void OnDestroy()
        {
            CardStack.OnCardModified -= UpdateDictionary;
        }

        private void Start()
        {
            m_workStacks = m_gameManager.GetCardsStacks();
        }

        private void UpdateDictionary(CardStack cardStack,int amountOfCards)
        {
            int maxCards = -1;

            for (int i = 0; i < m_workStacks.Length; i++)
            {
                if(cardStack == m_workStacks[i]) { continue; }
                if (m_workStacks[i].GetCardsCount() > maxCards)
                {
                    maxCards = m_workStacks[i].GetCardsCount();
                }        
            }

            if (amountOfCards > maxCards)
            {
                UpdateBorderPos(cardStack.LastCardPos());
            }
        }

        private void UpdateBorderPos(Vector3 newPos)
        {
            transform.position = newPos;
        }
    }

}
