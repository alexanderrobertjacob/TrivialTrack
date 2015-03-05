using UnityEngine;
using System.Collections;

public class PlayerBodyScript : MonoBehaviour {

    PlayerScript player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Key")
        {
            GameObject.Find("Door").SetActive(false);
            collider.gameObject.audio.Play();
            collider.gameObject.GetComponent<SpriteRenderer>().enabled = false; // SetActive(false);
            player.hasKey = true;
        }
        if (collider.gameObject.tag == "Plank" && !player.hasPlank)
        {
            player.hasPlank = true;
            collider.gameObject.SetActive(false);
        }
        if (collider.gameObject.tag == "Hole" && player.hasPlank)
        {
            collider.gameObject.SetActive(false);
            player.hasPlank = false;
        }
        else if (collider.gameObject.tag == "Hole" && !player.hasPlank)
        {
            this.GetComponent<HealthScript>().Damage(1);
        }
    }
}
