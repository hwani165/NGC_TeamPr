using UnityEngine;

public class Object : MonoBehaviour
{
    // �ӽ���
    private bool isCollected = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCollected == false) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Spawner.itemCount--;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //������ �������� ȹ��
            isCollected = true;
        }
    }

    private void OnDestroy()
    {
            Spawner.itemCount--;
    }
}
