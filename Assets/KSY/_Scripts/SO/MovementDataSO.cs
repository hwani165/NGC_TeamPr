using UnityEngine;

[CreateAssetMenu(fileName = "MovementDataSO", menuName = "SO/MovementDataSO")]
public class MovementDataSO : ScriptableObject
{
    public float Speed = 5f;
    public float JumpForce = 10f;
    public float Gravity = 9.8f;
    public float DashForce = 12;
    public float DashDuration = 0.2f;
}
