using UnityEngine;
using System.Collections;

public class VillianScript : MonoBehaviour {

    public float sawCooldown = 5.0f;
    public int maxSaws = 4;
    public GameObject sawBlade;
    public float throwSpeed = 75;
    public bool debugDestroy = false;
    public int health = 3;

    private float cooldown = 5.0f;
    private PlayerScript player;
    private int currentSaws = 0;
    private float redDuration = 0.5f;
    private float redCount = 0;
    private bool counting = false;

	// Use this for initialization
	void Start () {
        cooldown = 2.0f;
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        GameObject.Find("Key").GetComponent<CircleCollider2D>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            if (currentSaws < maxSaws)
            {
                SpawnSawblade();
                cooldown = sawCooldown;
                currentSaws++;
            }
        }

        if (counting)
        {
            redCount += Time.deltaTime;

            if (redCount >= redDuration)
            {
                counting = false;
                redCount = 0;
                this.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        if (debugDestroy)
        {
            BeatBoss();
        }
	}

    void SpawnSawblade()
    {
        float distanceToSpawn = gameObject.GetComponent<CircleCollider2D>().radius +
                                sawBlade.GetComponent<CircleCollider2D>().radius;
        float initialX;
        float initialY;

        if (player.transform.position.x - this.transform.position.x >= 0)
            initialX = distanceToSpawn;
        else
            initialX = -distanceToSpawn;
        if (player.transform.position.y - this.transform.position.y >= 0)
            initialY = distanceToSpawn;
        else
            initialY = -distanceToSpawn;

        var blade = (GameObject)GameObject.Instantiate(sawBlade, 
                                                        new Vector3(this.transform.position.x + initialX,
                                                                    this.transform.position.y + initialY, 
                                                                    this.transform.position.z),
                                                        this.transform.rotation);

        SawbladeScript saw = blade.GetComponent<SawbladeScript>();

        float xForce = (player.transform.position.x - transform.position.x);
        float yForce = (player.transform.position.y - transform.position.y);
        Vector2 sawBladeVelocity = new Vector2(xForce, yForce);
        sawBladeVelocity.Normalize();
        sawBladeVelocity.x = sawBladeVelocity.x * throwSpeed;
        sawBladeVelocity.y = sawBladeVelocity.y * throwSpeed;

        //saw.gameObject.rigidbody2D.velocity = Vector2.zero;

        saw.gameObject.rigidbody2D.AddForce(sawBladeVelocity, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "SawBlade")
        {
            if (collider.gameObject.GetComponent<SawbladeScript>().friendly)
            {
                this.GetComponent<SpriteRenderer>().color = Color.red;
                counting = true;

                health--;
                Destroy(collider.gameObject);
                currentSaws--;

                this.GetComponent<AudioSource>().Play();

                if (health <= 0)
                {
                    BeatBoss();
                }
            }
        }
    }

    void BeatBoss()
    {
        GameObject[] remainingSaws = GameObject.FindGameObjectsWithTag("SawBlade");
        foreach (GameObject saw in remainingSaws)
        {
            Destroy(saw);
        }
        GameObject.Find("Key").GetComponent<CircleCollider2D>().enabled = true;
        GameObject.Find("Timer").GetComponent<TimerScript>().counting = false;

        Destroy(this.gameObject);
    }
}
