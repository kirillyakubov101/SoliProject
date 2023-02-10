using UnityEngine;

namespace Soli.Card
{
    public class CardAnimation : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_winGameVFX;
        [SerializeField] private CardWrapper m_cardWrapper;
        [SerializeField] private Rigidbody2D m_rb;
        [SerializeField] private PolygonCollider2D bounceCardCollision;
        [SerializeField] private SpriteRenderer m_spriteRenderer;

        private void Start()
        {
            Sprite sprite = m_cardWrapper.GetCard().GetSprite();
            m_winGameVFX.textureSheetAnimation.SetSprite(0, sprite);
        }

        public void InitBounceCard(int sortOrder)
        {
            transform.parent = null;
            m_winGameVFX.Play();
            m_spriteRenderer.sortingOrder = sortOrder;
            m_rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            m_rb.bodyType = RigidbodyType2D.Dynamic;
            m_rb.AddForce(Vector2.left * 5.5f, ForceMode2D.Impulse);
            bounceCardCollision.enabled = true;
        }

    }

}
