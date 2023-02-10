using UnityEngine;

namespace Soli.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        [Header("Card From Deck")]
        [SerializeField] private AudioSource m_cardDeckTake_ADS;
        [Header("Place Card")]
        [SerializeField] private AudioSource m_cardPlace_ADS;
        [SerializeField] private float cardPlacePositionPlayback = 0.2f;
        [Header("Take Card")]
        [SerializeField] private AudioSource m_cardTake_ADS;
        [Header("Intro Shuffle")]
        [SerializeField] private AudioSource m_cardShuffle_ADS;
        [SerializeField] private float startTimeCardShufflePositionPlayback = 0.3f;


        #region Singleton
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

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AudioManager>();
                }

                return instance;
            }
        }
        #endregion

        private void Start()
        {
            m_cardShuffle_ADS.time = startTimeCardShufflePositionPlayback;
        }

        public void Play_ClickDeckSound()
        {
            m_cardDeckTake_ADS.Play();
        }

        public void Play_PlaceCardSound()
        {
            m_cardPlace_ADS.time = cardPlacePositionPlayback;
            m_cardPlace_ADS.Play();
        }

        public void Play_TakeCardSound()
        {
            m_cardTake_ADS.Play();
        }

        

    }

}

