using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Canvas manual;
    public void OpenManual()
    {
        manual.enabled = true;
    }

    public void CloseManual()
    {
        manual.enabled = false;
    }
}
