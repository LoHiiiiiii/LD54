using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quitGame : MonoBehaviour
{
    //sulkee editorin playmoden ja buildin .exen

    public void doExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();

        
    }
}
