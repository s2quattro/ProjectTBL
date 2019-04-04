using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_Manager : MonoBehaviour {

    public GameObject helpImage;
    private bool isHelpOn = false;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {	

	}

    public void LoadScene()
    {
        SceneManager.LoadScene("Loading");
    }

    public void LoadHelp()
    {
        if(isHelpOn)
        {
            isHelpOn = false;
            helpImage.SetActive(false);
        }
        else
        {
            isHelpOn = true;
            helpImage.SetActive(true);
        }        
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
