using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{

    private GameObject ExitPrompt;
    private bool firstpass = true;

    // Start is called before the first frame update
    void Awake()
    {
        ExitPrompt = GameObject.Find("Exit Prompt");
    }

    // Update is called once per frame
    void Update()
    {
        if (firstpass)
        {
            ExitPrompt.SetActive(false);
            firstpass = false;
        }
    }

    void OpenExitPrompt()
    {
        ExitPrompt.SetActive(true);
    }

    void ExitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void ClosePrompt()
    {
        ExitPrompt.SetActive(false);
    }
}
