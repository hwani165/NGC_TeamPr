using UnityEngine;

public class Return : MonoBehaviour
{
    [SerializeField] private Transform[] responPos;
    // ¿”Ω√¿”
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != LayerMask.NameToLayer("Player")) Destroy(collision.gameObject);
        else
        {
            Entity e = collision.gameObject.GetComponent<Entity>();

            Game.Instance.OtherHitCount -= 1;

            int rand = Random.Range(0, responPos.Length);
            collision.transform.position = responPos[rand].position;
        }
    }
}
