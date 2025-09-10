using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float damage = 20;
    public float knockbackmulti = 3;

    public float lifetime = 0.2f;
    public float invisibletime = 0.1f;
    public LayerMask layermask;
    public Transform preowner;

    private Dictionary<Entity, float> lastHitTime = new Dictionary<Entity, float>();
    private CircleCollider2D circle; // 폭발 범위 콜라이더 (isTrigger = true)

    private void Start()
    {
        circle = GetComponent<CircleCollider2D>();
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (circle == null) return;

        // 현재 Trigger 범위 안에 있는 모든 콜라이더 가져오기
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circle.radius, layermask);

        float now = Time.time;
        foreach (var hit in hits)
        {
            Entity player = hit.GetComponent<Entity>();
            if (player == null) continue;

            if (!lastHitTime.ContainsKey(player) || now - lastHitTime[player] >= invisibletime)
            {
                lastHitTime[player] = now;

                if (hit.transform == preowner)
                    player.Attack(preowner, damage / 1.5f, knockbackmulti);
                else
                    player.Attack(preowner, damage, knockbackmulti);
            }
        }
    }
}
