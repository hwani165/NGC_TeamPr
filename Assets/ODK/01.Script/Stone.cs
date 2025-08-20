using System.Collections;
using UnityEngine;

public class Stone : Item
{
    public float damage = 5;
    public float knockbackmulti = 1;
    public override IEnumerator Attacking(GameObject target)
    {
        base.Attacking(target);

        if (target.GetComponent<Entity>() != null)
        {
            target.GetComponent<Entity>().Attack(transform, damage, knockbackmulti);
        }
        Destroy(gameObject);
        yield return null;
        
    }
}
