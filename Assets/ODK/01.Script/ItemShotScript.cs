using System.Collections;
using UnityEngine;

public class ItemShotScript : MonoBehaviour
{
    [SerializeField] private Transform holdTransform;
    [SerializeField] private GameObject holdObject;
    private Rigidbody2D rb;

    [SerializeField] private float shootPower = 80f;
    [SerializeField] private float upwardForce = 30f;
    [SerializeField] private float playerRecoil = 40f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // �߻� �Է� üũ
        if ((Input.GetKeyDown(KeyCode.UpArrow) ||
             Input.GetKeyDown(KeyCode.DownArrow) ||
             Input.GetKeyDown(KeyCode.LeftArrow) ||
             Input.GetKeyDown(KeyCode.RightArrow)) && holdObject != null)
        {
            StartCoroutine(Shoot());
        }
    }

    private Vector2 GetInputDirection()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow)) dir.x -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) dir.x += 1f;
        if (Input.GetKey(KeyCode.UpArrow)) dir.y += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) dir.y -= 1f;

        if (dir != Vector2.zero) dir.Normalize();
        return dir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Item holditem = collision.gameObject.GetComponent<Item>();
        if (holditem == null) return;

        // �̹� �� �������̸� �浹 ����
        if (holditem.owner == gameObject)
        {
            Collider2D myCol = GetComponent<Collider2D>();
            Collider2D itemCol = holditem.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(myCol, itemCol, true);
            return;
        }

        // ������ ���� ���� ����
        if (holditem.owner == null && !holditem.iscooldown && !holditem.isshooting)
        {
            // ���� ��� �ִ� ������ ó��
            if (holdObject != null)
            {
                holdObject.GetComponent<Item>().owner = null;
                Rigidbody2D oldRb = holdObject.GetComponent<Rigidbody2D>();
                oldRb.simulated = true;
                oldRb.transform.parent = null;
                holdObject.GetComponent<Item>().CooldownActive();
            }

            // �� ������ ���
            holdObject = holditem.gameObject;
            holditem.owner = gameObject;

            Rigidbody2D rbh = holditem.GetComponent<Rigidbody2D>();
            rbh.simulated = false;
            rbh.transform.parent = holdTransform;
            holditem.transform.localPosition = Vector2.zero;
        }
    }

    private IEnumerator Shoot()
    {
        if (holdObject == null) yield break;


        Item itemScript = holdObject.GetComponent<Item>();
        itemScript.isshooting = true;
        Rigidbody2D hrb = holdObject.GetComponent<Rigidbody2D>();
        // �θ� ���� & ���� ��ġ�� �̵�
        holdObject.transform.parent = null;
        holdObject.transform.position = transform.position + (Vector3)(GetInputDirection() * 1.25f);

        
        itemScript.CooldownActive();
        yield return new WaitForSeconds(0.02f);
        hrb.simulated = true;
        hrb.linearVelocity = Vector2.zero;
        Vector2 dir = GetInputDirection();
        if (dir == Vector2.zero) dir = Vector2.right;

        // �߻�
        hrb.AddForce(dir * shootPower + new Vector2(0, upwardForce), ForceMode2D.Impulse);
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = dir.x * 20;
        // �÷��̾� �ݵ�
        rb.AddForce(-dir * playerRecoil, ForceMode2D.Impulse);

        // ��ٿ� & �տ��� ����
        
        holdObject = null;
    }
}