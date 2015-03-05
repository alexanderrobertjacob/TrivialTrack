using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour
{

    public Vector2 velocity = new Vector2();
    public Vector2 direction = new Vector2();
    public float gravityScale = 1;
    private float gravity = 9.8f;

    //private Vector2 movement = new Vector2();
    private float xMovement = 0;
    private float yMovement = 0;

    public void UpdateXSpeed(float x)
    {
        velocity.x = x;
    }

    public void UpdateYSpeed(float y)
    {
        velocity.y = y;
    }

    public void AddSpeed(Vector2 velToAdd)
    {
        rigidbody2D.velocity += velToAdd;
    }

    public void ChangeXDirection(float x)
    {
        direction.x = x;
    }

    public void ChangeYDirection(float y)
    {
        direction.y = y;
    }

    // Update is called once per frame
    void Update()
    {
        xMovement = (velocity.x * direction.x);
        yMovement = (velocity.y * direction.y);
        //velocity.y -= (gravity * gravityScale * Time.deltaTime);
        //xMovement = new Vector2(xVel, rigidbody2D.velocity.y);
    }

    void FixedUpdate()
    {
        rigidbody2D.velocity = new Vector2(xMovement, yMovement);
    }
}
