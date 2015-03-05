using UnityEngine;
using System.Collections;

public class AndroidGUIScript : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
	    if(GUI.Button(new Rect(0,0,0,0), "test")){
            Debug.Log("Test clicked");
        } 
	}
}
