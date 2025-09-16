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
            if(collision.gameObject.name == "P1")
            {
                p1MaxLife--;
                if(p1MaxLife <= 0)
                {
                    Game.Instance.EndGame();
                }
            }
            else
            {
                p2MaxLife--;
                if (p2MaxLife <= 0)
                {
                    Game.Instance.EndGame();
                }
            }

            int rand = Random.Range(0, responPos.Length);
            collision.transform.position = responPos[rand].position;
        }
    }
}
