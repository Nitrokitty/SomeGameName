using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**This controls the Playing of Background Music in the Game

*/
public class BGMController : MonoBehaviour {
	public GameObject bgm; //Background Music
  public GameObject connWaiting; //Connection Waiting Music
  public GameObject bgmFinal; //Final Fight Music
  
  private Manager manager; //Get the Manager of the game to check the state
  
  public bool isPlaying;
  
  private GameObject currPlaying; //Currently Playing Audio
  // Use this for initialization
	void Start () {
		PlayAudio(connWaiting);
    isPlaying = false;
    manager = GameObject.FindWithTag("GameManager").GetComponent<Manager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPlaying && Manager.CurrentState == States.GameIsGoing ) {
      isPlaying = true;
      PlayAudio(bgm);
    }
	}
  
  void OnPlayerConnected(NetworkPlayer player) {
    if (isPlaying) return;
    isPlaying = true;
    PlayAudio(bgm);
  }
  
  //Plays the AudioSource Object inside of the GameObject
  void PlayAudio(GameObject ga) {
    if (currPlaying != null) {
      StopAudio(currPlaying);
    }
    try {
      ga.GetComponent<AudioSource>().Play();
      currPlaying = ga;
    } catch (System.NullReferenceException e) {
      print("There was an Error...");
    }
  }
  
  //Stops the AudioSource inside of a GameObject
  void StopAudio(GameObject ga) {
    try {
      ga.GetComponent<AudioSource>().Stop();
      currPlaying = null;
    } catch (System.NullReferenceException e) {
      print("There was an Error...");
    }
  }
  
  //Plays the BGM
  public void PlayBGM() {
    PlayAudio(bgm);
  }
  
  //Plays Intermission
  public void PlayInter() {
    PlayAudio(connWaiting);
  }
  
  //Plays Endgame Music
  public void PlayBGMFinal() {
    PlayAudio(bgmFinal);
  }
}
