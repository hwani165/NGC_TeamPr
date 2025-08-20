using UnityEngine;
namespace SDW
{
    public class MoveGround : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveDistanceX = 10f;
        [SerializeField] private float moveDistanceY = 0f;

        private Vector3 startPosition;
        private Vector3 targetPosition;
        private bool movingToTarget = true;

        private void Awake()
        {
            startPosition = transform.position;
            targetPosition = new Vector3(
                startPosition.x + moveDistanceX,
                startPosition.y + moveDistanceY,
                startPosition.z
            );
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            Vector3 destination = movingToTarget ? targetPosition : startPosition;
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                transform.position = destination;
                movingToTarget = !movingToTarget;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                collision.transform.SetParent(transform, true);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                collision.transform.SetParent(null);
            }
        }
    }
}