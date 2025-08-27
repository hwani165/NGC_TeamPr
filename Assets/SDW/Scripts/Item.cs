using UnityEngine;
namespace SDW
{
    public class Item : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Spawner.itemX = (sbyte)Mathf.RoundToInt(transform.position.x);
            }
        }
    }
}
