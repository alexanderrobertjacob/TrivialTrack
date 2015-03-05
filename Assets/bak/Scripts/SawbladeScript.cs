using UnityEngine;
using System.Collections;

public class SawbladeScript : MonoBehaviour {

    private MoveScript _movescript;
    public bool friendly = false;
    public int maxFriendlyBounce = 1;
    private int bounceCount;

    public float initialXForce = 200;
    public float initialYForce = 200;
    public float maxVelocity = 5.0f;
    public float slowdownSpeed = 5.0f;
    public bool applyInitialVelocity = false;

    //For debug
    private float curVelocity = 0.0f;

	// Use this for initialization
	void Start () {
        bounceCount = 0;
        if(applyInitialVelocity)
            rigidbody2D.AddForce(new Vector2(initialXForce, initialYForce));
	}
	
	// Update is called once per frame
    void Update()
    {
        curVelocity = magnitude(rigidbody2D.velocity.x, rigidbody2D.velocity.y);
        if (curVelocity > maxVelocity)
        {
            rigidbody2D.AddForce(-rigidbody2D.velocity * Time.deltaTime * slowdownSpeed);
        }
	
	}

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (friendly && collider.gameObject.tag == "Turret")
        {
            Destroy(collider.transform.parent.gameObject);
            audio.Play();
            Destroy(this.gameObject);
        }
        else if (friendly && bounceCount >= maxFriendlyBounce)
        {
            bounceCount = 0;
            friendly = false;
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
        else if (friendly)
        {
            bounceCount++;
        }

    }

    private float magnitude(float x, float y)
    {
        float temp = 0;
        x = x * x;
        y = y * y;

        temp = x + y;
        temp = Mathf.Sqrt(temp);

        return temp;
    }
}
