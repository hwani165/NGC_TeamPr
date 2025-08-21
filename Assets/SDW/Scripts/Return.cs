using UnityEngine;

public class Return : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != LayerMask.NameToLayer("UI")) return;
        collision.transform.position = new Vector2(-3, 2);
    }
}
