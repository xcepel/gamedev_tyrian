using UnityEngine;

public class JoystickVisibility : MonoBehaviour
{
    public GameObject joystick;

    void Start()
    {
        // PC input (keyboard) with windows
        #if UNITY_STANDALONE
            joystick.SetActive(false);
            Destroy(joystick);
        #endif
    }
}