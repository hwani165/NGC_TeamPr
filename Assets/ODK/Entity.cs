using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioSource audioSource;

    public byte Health;
    public bool IsMyPlayer;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();


        IsMyPlayer = gameObject.name == "P1";
        Health = IsMyPlayer ? Game.P1LIFE : Game.P2LIFE;
    }
    private void Update()
    {
        if(Health <= 0)
        {
            Game.Instance.EndGameServer(Game.Instance.OtherHitCount);
        }
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
        yield return new WaitForSeconds(1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void Attack(Transform tra, float damage, float knockback)
    {
        if (damage >= 1)
            audioSource.PlayOneShot(hitSound);
        else 
            audioSource.PlayOneShot(healSound);

        // HP 감소
        Health -= 1;
        Game.Instance.SendPlayerHealth(gameObject.name, Health);

        if(!IsMyPlayer)
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
            ) * 1;

            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
