using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioSource audioSource;

    public sbyte Health;
    public bool IsMyPlayer;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        IsMyPlayer = gameObject.name == "P1";
        Health = IsMyPlayer ? Game.P1LIFE : Game.P2LIFE;
    }
    public void UpdateHealthUI()
    {
        if (IsMyPlayer)
        {
            Game.Instance.MapCompo.P1Health.text = $"{Server.MyName} health : {Health}";
        }
        else
        {
            Game.Instance.MapCompo.P2Health.text = $"{Server.OtherName} health : {Health}";
        }
    }
    private IEnumerator OnHit()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void Attack(Transform tra, sbyte damage, float knockback)
    {
        Debug.Log($"{gameObject.name} has damage : {damage}");

        if (damage <= 0)
            audioSource.PlayOneShot(hitSound);
        else 
            audioSource.PlayOneShot(healSound);

        //HP 증감
        Health += damage;
        Debug.Log($"{gameObject.name} has damage : {damage}");

        if (Health <= 0)
        {
            Game.Instance.EndDataSend(Game.Instance.OtherHitCount);
        }

        Game.Instance.SendPlayerHealth(gameObject.name, Health);

        Debug.Log($"{gameObject.name} has damage : {damage}");
        if (!IsMyPlayer && damage < 0)
        {
            Game.Instance.OtherHitCount += 1;
            Game.Instance.MapCompo.HitCountT.text = $"hit count : {Game.Instance.OtherHitCount}";
        }

        UpdateHealthUI();
        StartCoroutine(OnHit());

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 knockbackDir = (transform.position - tra.position).normalized;

            // y좌표 차이에 따라 강제 보정
            float yDiff = transform.position.y - tra.position.y;

            if (yDiff >= 0f)
                knockbackDir.y = 1f;
            else if (yDiff <= -0.5f)
                knockbackDir.y = -1f; 

            knockbackDir.Normalize();

            float baseForceX = knockback * 7f;
            float baseForceY = knockback * 4f;

            Vector2 force = new Vector2(
                knockbackDir.x * baseForceX,
                knockbackDir.y * baseForceY
            ) * 2f;

            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
