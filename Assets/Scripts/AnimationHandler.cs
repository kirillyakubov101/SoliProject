using System.Collections;
using UnityEngine;
using Soli.Card;
using System.Threading.Tasks;

namespace Soli.Utils
{
    public class AnimationHandler : MonoBehaviour
    {
        [SerializeField] private CardWrapper m_card;
        [SerializeField] private float smoothTime = 2f;
        [SerializeField] private float animTime = 2f;

        [SerializeField] private float endAnimationTime = 5f;
        [SerializeField] private float endAnimationsmoothTime = 50f;


        private static AnimationHandler instance;

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

        public static AnimationHandler Instance()
        {
            return instance;
        }

        public void AnimatePreivewCard(CardWrapper _animObject,Vector3 goalPos)
        {
            this.m_card = _animObject;
            StartCoroutine(MoveCardToShowCase(_animObject, goalPos));
        }

        public void ShowCardInShowCase(CardWrapper _animObject, Vector3 goalPos)
        {
            _animObject.transform.position = goalPos;
            _animObject.ParentedPos = _animObject.transform.position;
            _animObject.PutCardFaceUp();
        }

        private IEnumerator MoveCardToShowCase(CardWrapper _animObject, Vector3 goalPos)
        {
            float time = 0f;
            GameManager.DisableInput();

            while(time < animTime)
            {
                _animObject.transform.position =  Vector3.Lerp(_animObject.transform.position, goalPos, Time.deltaTime * smoothTime);


                time += Time.deltaTime;
                yield return null;
            }
            _animObject.transform.position = goalPos;
            _animObject.ParentedPos = _animObject.transform.position;
            _animObject.PutCardFaceUp();

            m_card = null;
            GameManager.EnableInput();
        }

        public void MoveCardsToWin(CardWrapper cardToMove)
        {
            MoveCard(cardToMove.transform, cardToMove.EndPositionForAutoWin);
        }

        private async Task MoveCard(Transform card, Vector3 pos)
        {
            float t = 0f;
            while (t < endAnimationTime)
            {
                card.transform.position = Vector3.Lerp(card.transform.position, pos, Time.deltaTime * endAnimationsmoothTime);
                t += Time.deltaTime;
                await Task.Yield();
            }
        }
    }
}

