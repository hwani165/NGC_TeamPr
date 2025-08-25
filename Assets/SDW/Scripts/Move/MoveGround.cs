using UnityEngine;
namespace SDW
{
    public class MoveGround : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveDistanceX = 10f;
        [SerializeField] private float moveDistanceY = 0f;

        private Vector3 startPosition;
        private Vector3 targetPosition;
        private bool movingToTarget = true;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            startPosition = transform.position;
            targetPosition = new Vector3(
                startPosition.x + moveDistanceX,
                startPosition.y + moveDistanceY,
                startPosition.z
            );
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            Vector3 destination = movingToTarget ? targetPosition : startPosition;
            Vector3 newPos = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                rb.MovePosition(destination);
                movingToTarget = !movingToTarget;
            }
        }
    }
}