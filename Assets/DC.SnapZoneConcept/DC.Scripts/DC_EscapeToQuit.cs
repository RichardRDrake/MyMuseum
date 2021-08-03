using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_EscapeToQuit : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
