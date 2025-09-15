using UnityEngine;

public class Return : MonoBehaviour
{
    [SerializeField] private Transform[] responPos;
    [SerializeField] static public byte p1MaxLife = 3;
    [SerializeField] static public byte p2MaxLife = 3;

    // ¿”Ω√¿”
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != LayerMask.NameToLayer("Player")) Destroy(collision.gameObject);
        else
        {
            string pName = collision.gameObject.name;
            if (pName == "P1")
            {
                p1MaxLife--;
                if(p1MaxLife <= 0)
                {
                    Game.Instance.EndGame(pName);
                }
            }
            else
            {
                p2MaxLife--;
                if (p2MaxLife <= 0)
                {
                    Game.Instance.EndGame(pName);
                }
            }

            int rand = Random.Range(0, responPos.Length);
            collision.transform.position = responPos[rand].position;
        }
    }
}
