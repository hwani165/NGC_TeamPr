using UnityEditor.U2D.Aseprite;
using UnityEngine;

namespace SDW
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private LayerMask breakableLayer; // ����� �ı��� �� �ִ� ���̾�
        [SerializeField] private bool breakable = true; // ��� �ı� ���� ����
        [SerializeField] private Rigidbody2D childBlock;  // �ٷ� �Ʒ� ���

        private int blockLayer; // ��� ���̾�
        private Joint2D joint;

        private void Awake()
        {
            blockLayer = Mathf.RoundToInt(Mathf.Log(breakableLayer.value, 2));
            joint = GetComponent<Joint2D>();
        }

        private void Hit()
        {
            if (!breakable) return;

            // �Ʒ� ����� ��� Joint ����
            if (childBlock != null)
            {
                // ���� ����� Block ��ũ��Ʈ ��������
                Joint2D[] joints = childBlock.GetComponents<Joint2D>();
                foreach (var j in joints)
                    Destroy(j);
            }

            // �ڱ� �ڽ��� Joint ����
            if (joint != null)
                Destroy(joint);

            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.layer != blockLayer) return;

            Hit();
        }
    }
}