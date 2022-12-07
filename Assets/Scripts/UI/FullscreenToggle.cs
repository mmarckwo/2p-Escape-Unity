using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    private bool screenToggle = false;

    public void ToggleFullScreen()
    {
        screenToggle = !screenToggle;

        // set to fullscreen.
        if (screenToggle == true)
        {
            Resolution monitorResolution = Screen.currentResolution;
            Screen.SetResolution(monitorResolution.width, monitorResolution.height, true);
        }
        // turn off fullscreen.
        else
        {
            Screen.SetResolution(1024, 768, false);
        }
    }
}
