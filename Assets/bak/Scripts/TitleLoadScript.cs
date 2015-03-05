using UnityEngine;
using System.Collections;

public class TitleLoadScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //GameObject.Find("TimerText").SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Start"))
        {
            //GameObject.Find("Timer").gameObject.GetComponent<TimerScript>().counting = true;
            //GameObject.Find("TimerText").SetActive(true);
            Application.LoadLevel(11);
        }
	}
}
