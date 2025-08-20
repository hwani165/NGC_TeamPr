using UnityEngine;

public class Entity : MonoBehaviour
{
    public float MaxHP { get; private set; } = 100f;
    public float CurrentHp { get; private set; } = 100f;
    [SerializeField] private HPBar hpbar;
    public void Attack(Transform tra, float damage, float knockback)
    {

        CurrentHp -= damage;
        hpbar.SetHP();

        GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.position.normalized.x - tra.position.normalized.x * knockback *5,knockback * 10),ForceMode2D.Impulse);
    }
}
