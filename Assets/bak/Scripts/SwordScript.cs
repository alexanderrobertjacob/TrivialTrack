using UnityEngine;
using System.Collections;

public class SwordScript : WeaponScript {

    public float swordSwingSpeed = 500f;
    public float powerAttackMultiplyer = 2.0f;
    public float powerAttackThreshhold = 2.0f;
    public float defaultRValue = 0;
    public float defaultGValue = 0;
    public float defaultBValue = 0;
    public float attackDuration = 1.0f;

    public const float DELAY_FOR_NORMAL_ATTACK = 0.5f;

    private bool swording = false;
    private bool powerSwording = false;
    private bool counting = false;
    private float count = 0.0f;
    private SpriteRenderer _sprite;
    private BoxCollider2D _collider;
    private PlayerScript _player;
    private Vector3 playerFacing;

    private const int smooth = 5;
	private const float swordRotationOffset = -90.0f;

    public void Attack()
    {
		playerFacing = _player.currentFacing;
		transform.rotation = Quaternion.Euler(0, 0, playerFacing.z + 120 + swordRotationOffset);
        swording = true;
        counting = true;
        count = 0.0f;
//        Debug.Log("Player facing = " + playerFacing.z);
    }

    public void StartAttack()
    {
        count = 0;
        counting = true;
    }

    public void StartPowerAttack()
    {
        _sprite.enabled = true;
    }

    public void MaxPowerAttack()
    {
        _sprite.color = new Color(0, 0, 1);
    }

    public void ReleaseAttack()
    {
        if (count < powerAttackThreshhold)
        {
            Attack();
        }
        else if (count >= powerAttackThreshhold)
        {
            powerSwording = true;
            damage = damage * powerAttackMultiplyer;
            Attack();
        }

    }

    public void Start()
    {
        _sprite = this.GetComponent<SpriteRenderer>();
        _collider = this.GetComponent<BoxCollider2D>();
        //defaultRValue = _sprite.color.r;
        //defaultRValue = _sprite.color.g;
        //defaultRValue = _sprite.color.b;
        _collider.enabled = false;
        _sprite.enabled = false;
        transform.eulerAngles = Vector3.zero;
        _player = GameObject.Find("Player").GetComponent<PlayerScript>();
        playerFacing = GameObject.Find("Player").GetComponent<PlayerScript>().currentFacing;
    }

    public void Update()
    {
        if (counting)
        {
            count += Time.deltaTime;
        }
        if (swording)
        {
            //Increase damage by power attack multiplyer if power attacking
            //float originalDamage = damage;

            _collider.enabled = true;
            _sprite.enabled = true;
            BoxCollider2D swordColl = gameObject.GetComponent<BoxCollider2D>();

			var targetAngle = Quaternion.Euler(0, 0, playerFacing.z - 20 + swordRotationOffset);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, count/attackDuration);

			if (count >= attackDuration)
            {
                _collider.enabled = false;
                _sprite.enabled = false;
                swording = false;
                if (powerSwording)
                {
//                    Debug.Log("Resetting power attack damage");
                    damage = damage / powerAttackMultiplyer;
                    //Always stop power attacking after each attack
                    powerSwording = false;
                }

                object[] arg = new object[0];
                _player.stateMachine.sendActionMessage("StopPowerAttack", arg);
            }

        }

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "SawBlade")
        {
            float xForce = (collider.transform.position.x - transform.position.x);
            float yForce = (collider.transform.position.y - transform.position.y);

            collider.rigidbody2D.AddForce(new Vector2(xForce * 200, yForce * 200));
            collider.GetComponent<SawbladeScript>().friendly = true;
            collider.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
        }
    }
}
