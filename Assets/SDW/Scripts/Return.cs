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

            if(e.gameObject.name == "P1")
            {
                sbyte c = Game.Instance.OtherHitCount -= 1;
                Game.Instance.MapCompo.HitCountT.text = $"Hit Count : {c}";
            }

            int rand = Random.Range(0, responPos.Length);
            Rigidbody2D _rb = collision.GetComponent<Rigidbody2D>();
            _rb.linearVelocity = Vector2.zero;
            _rb.MovePosition(responPos[rand].position);
        }
    }
}
