using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Awake()
    {
//#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
//#endif
    }
}
