using UnityEngine;
using TMPro;

namespace Soli.Utils
{
    public class ScoreHandler : MonoBehaviour
    {
        [SerializeField] private int m_score;
        [SerializeField] private int m_baseScoreAdd;
        [SerializeField] private int m_specialScoreAdd;
        [SerializeField] private TMP_Text scoreText;

        private static ScoreHandler instance;


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

        public static ScoreHandler Instance { get { return instance; } }

        public void AddBaseScore()
        {
            m_score += m_baseScoreAdd;
            UpdateTextScore();
        }

        public void AddSpecialScore()
        {
            m_score += m_specialScoreAdd;
            UpdateTextScore();
        }

        private void UpdateTextScore()
        {
            scoreText.text = m_score.ToString();
        }

    }
}

