using UnityEngine;
using System.Collections;

public class WinScript : MonoBehaviour {

    public AudioClip winMusic;

	// Use this for initialization
	void Start () {
        GameObject.Find("MusicPlayer").GetComponent<AudioSource>().Stop();
        GameObject.Find("MusicPlayer").GetComponent<AudioSource>().PlayOneShot(winMusic);
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Start"))
        {
            //GameObject.Find("Timer").gameObject.GetComponent<TimerScript>().counting = true;
            //GameObject.Find("TimerText").SetActive(true);
            Application.Quit();
        }
	
	}
}
