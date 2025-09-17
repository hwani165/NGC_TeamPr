using System.Collections;
using UnityEngine;

public class Stone : Item
{
    public sbyte damage = -1;
    public float knockbackmulti = 1;
    public override IEnumerator Attacking(GameObject target)
    {
        base.Attacking(target);

        if (target.GetComponent<Entity>() != null)
        {
            target.GetComponent<Entity>().Attack(preowner, damage, knockbackmulti);
        }
        Destroy(gameObject);
        yield return null;
        
    }
}
