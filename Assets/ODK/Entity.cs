using System.Collections;
using TMPro;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private GameObject gameOverUI;

    public byte PlayerLife;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        PlayerLife = (byte)Random.Range(1, 6);
    }
    private void Update()
    {
        if(PlayerLife <= 0)
        {

            Game.Instance.EndGameServer(Server.Instance.GetOtherData().Value.nickname);
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
        else if (damage < 0)
                audioSource.PlayOneShot(healSound);

        // HP 감소
        PlayerLife -= 1;
        Debug.Log(PlayerLife);
        OnHit();

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
