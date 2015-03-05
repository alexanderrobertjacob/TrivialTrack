using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
	public float bulletSpeed, flashCooldown;
	public Sprite bulletBlack, bulletRed;
	protected float flashTimer;
	protected bool isBlack;
	protected SpriteRenderer _renderer;

	void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
		flashTimer = flashCooldown;
		isBlack = true;

		rigidbody2D.velocity = bulletSpeed * transform.right;
	}
	
	void Update()
	{
		flashTimer -= Time.deltaTime;
		if (flashTimer <= 0)
		{
			flashTimer = flashCooldown;
			isBlack = !isBlack;
			_renderer.sprite = isBlack? bulletBlack : bulletRed;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
        Destroy(this.gameObject);
        
	}
}
