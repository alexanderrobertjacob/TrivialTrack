using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerScript : MonoBehaviour {

    public float initialTime = 20.0f;
    public float timeLeft;
    public int failScene = 8;

    public Text timeUI;
    public bool counting = false;

	// Use this for initialization
	void Awake () {
        timeUI = GameObject.Find("TimerText").GetComponent<Text>();
	}

    void Start()
    {
        timeLeft = initialTime;

    }
	
	// Update is called once per frame
    void Update()
    {
        timeUI = GameObject.Find("TimerText").GetComponent<Text>();
        if(counting)
            timeLeft -= Time.deltaTime;
        var truncated = (float)((int)(timeLeft * 10)) / 10.0f;
        timeUI.text = string.Format("Time Left: {0}", truncated);

        if (timeLeft <= 0)
        {
            counting = false;
            timeLeft = initialTime;
            Application.LoadLevel(failScene);
        }
	}
}
