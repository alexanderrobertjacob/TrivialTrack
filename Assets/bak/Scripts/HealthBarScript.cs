using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

	public RectTransform healthTransform;
	private float cachedY;
	private float minX;
	private float maxX;
	private int times;
    private HealthScript playerHealth;
	public float currentHealth;
	public float maxHealth;
	public Text healthText;
	public Image visualHealth;
    

	// Use this for initialization
	void Start ()
	{
		cachedY = healthTransform.position.y;
		maxX = healthTransform.position.x;
		minX = healthTransform.position.x - healthTransform.rect.width;
        playerHealth = GameObject.Find("Player").GetComponentInChildren<HealthScript>();

        if(playerHealth != null)
        {
            maxHealth = playerHealth.health;
        }
        currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update ()
	{
		HandleHealth ();
	}

	private void HandleHealth()
	{
        if (playerHealth == null)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth = playerHealth.health;
        }

		healthText.text = "Health : " + currentHealth;

		float currentXValue = MapValues (currentHealth, 0, maxHealth, minX, maxX);

		healthTransform.position = new Vector3 (currentXValue, cachedY);

		/*if (currentHealth > maxHealth) {
			visualHealth.color = new Color32 ((byte)MapValues (currentHealth, maxHealth / 2, maxHealth, 255, 0), 255, 0, 255);
		} else
			visualHealth.color = new Color32 (255, (byte)MapValues (currentHealth, 0, maxHealth / 2, 0, 255), 0, 255);*/
	}

	private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}


}
