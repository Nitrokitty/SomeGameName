using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtton : MonoBehaviour {
        
	
    public void LoadLevel()
    {
         SceneManager.LoadScene("testLevel");
    }
}


