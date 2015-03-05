using UnityEngine;
using System.Collections;

public class ExitScript : MonoBehaviour {

    public int nextLevel = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Application.LoadLevel(nextLevel);
        }
    }
}
