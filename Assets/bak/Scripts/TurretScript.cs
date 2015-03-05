using UnityEngine;
using System.Collections;

public class TurretScript : MonoBehaviour
{
	public StateMachine<TurretScript> stateMachine;
	
	public const int State_Scanning = 0;
	public const int State_Attacking = 1;
	public const int State_Reseting = 2;

	public GameObject bulletPrefab, muzzleFlashPrefab;
	public Transform endOfBarrel;
	public LineRenderer laser;
	public float scanningSpeed, trackingSpeed, resetSpeed, shotCooldown, attackingCooldown, range, startScanningAngle, endScanningAngle;
	public bool trackBeyondAngleLimits;

	protected Transform _transform, player;
	protected float attackingTimer, shotTimer, barrelLength, scanDegreesCount, scanDegreesTotal;
	protected bool scanningTowardsEnd;

	void Awake()
	{
		var value = 1.2345f;
		var result2 = (float)((int)(value * 10.0f)) / 10.0f;
		var result = string.Format("Test {0}", result2);

		_transform = transform;
		player = GameObject.Find("Player").transform;

		barrelLength = endOfBarrel.position.x - _transform.position.x;
		scanningTowardsEnd = true;
		_transform.rotation = Quaternion.Euler(0, 0, startScanningAngle);

		if (scanningSpeed > 0)
		{
			scanDegreesTotal = endScanningAngle - startScanningAngle;
		}
		else
		{
			scanDegreesTotal = startScanningAngle - endScanningAngle;
		}
		if (scanDegreesTotal < 0) scanDegreesTotal += 360.0f;

		scanDegreesTotal = Mathf.Abs(scanDegreesTotal);
		scanDegreesCount = scanDegreesTotal;
		attackingTimer = attackingCooldown;
		shotTimer = 0;

		State<TurretScript>[] statesList = new State<TurretScript>[] {
			new ScanningState(),
			new AttackingState(),
			new ResetingState()
		};

		stateMachine = new StateMachine<TurretScript>(State_Scanning, statesList, this);
	}
	
	class ScanningState : State<TurretScript>
	{
		public ScanningState()
		{
		}
		
		public override void OnEnter(TurretScript owner)
		{
			owner.laser.enabled = true;
		}
		
		public override int PerformStateAction(TurretScript owner)
		{
			var startAngle = owner.transform.rotation.eulerAngles.z;
			var scanningSpeed = owner.scanningTowardsEnd? owner.scanningSpeed : -owner.scanningSpeed;
			owner._transform.Rotate(new Vector3(0, 0, scanningSpeed * Time.deltaTime));

			if (owner.startScanningAngle != owner.endScanningAngle)
			{
				float endAngle = owner.transform.rotation.eulerAngles.z;
				float deltaAngle = endAngle - startAngle;
				if (scanningSpeed < 0)
					deltaAngle = startAngle - endAngle;

				if (deltaAngle < 0) deltaAngle += 360.0f;

				owner.scanDegreesCount -= deltaAngle;

				if (owner.scanDegreesCount <= 0)
				{
					owner.scanDegreesCount = owner.scanDegreesTotal;
					owner.scanningTowardsEnd = !owner.scanningTowardsEnd;
				}
			}

			var endOfLaser = owner.endOfBarrel.position + owner.range * owner._transform.right;
			owner.laser.SetPosition(0, owner.endOfBarrel.position);
			owner.laser.SetPosition(1, endOfLaser);

			var hit = Physics2D.Raycast(owner.endOfBarrel.position, owner._transform.right, owner.range);
			if (hit.transform != null && hit.transform.gameObject.tag == "Player")
				return State_Attacking;

			else if (hit.transform != null)
				owner.laser.SetPosition(1, hit.point);

			return State_Scanning;
		}
		
		public override void OnExit(TurretScript owner)
		{
			owner.laser.enabled = false;
		}
	}
	
	class AttackingState : State<TurretScript>
	{
		public AttackingState()
		{
		}
		
		public override void OnEnter(TurretScript owner)
		{
			owner.scanDegreesCount = owner.scanDegreesTotal;
		}
		
		public override int PerformStateAction(TurretScript owner)
		{
			// Can we still see the player?
			var direction = owner.player.position - owner._transform.position;
			direction.Normalize();

			var startOfRay = owner._transform.position + owner.barrelLength * direction;
			var hit = Physics2D.Raycast(startOfRay, direction, owner.range);
			if (hit.transform == null || hit.transform.gameObject.tag != "Player")
			{
				owner.attackingTimer -= Time.deltaTime;
			}
			else
			{
				var targetAngle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + 90.0f;
				Quaternion newRotation = Quaternion.Lerp(owner._transform.rotation,
				                                  Quaternion.Euler(0, 0, targetAngle),
				                                  owner.trackingSpeed * Time.deltaTime);
				targetAngle = newRotation.eulerAngles.z;
				var deltaAngle = targetAngle - owner._transform.eulerAngles.z;

				// what is the boundary angle?
				// if I'm going in the same direction as the owner.scanningSpeed, it's endScanningAngle
				// otherwise it's startScanningAngle
				owner.scanningTowardsEnd = (Mathf.Sign(deltaAngle) == Mathf.Sign(owner.scanningSpeed));
				var angleLimit = owner.scanningTowardsEnd? owner.endScanningAngle : owner.startScanningAngle;

				if ( owner.trackBeyondAngleLimits
				    || (owner.startScanningAngle == owner.endScanningAngle)
				    || (deltaAngle > 0 && targetAngle < angleLimit)
				    || (deltaAngle < 0 && targetAngle > angleLimit) )
				{
					owner.attackingTimer = owner.attackingCooldown;
					owner._transform.rotation = newRotation;
				}
				else
				{
					//Debug.Log ("Out of bounds: targetAngle = " + targetAngle + ", angleLimit = " + angleLimit);
					owner.attackingTimer -= Time.deltaTime;
				}
			}
			
			owner.shotTimer -= Time.deltaTime;
			if (owner.shotTimer <= 0)
			{
				var flash = GameObject.Instantiate(owner.muzzleFlashPrefab, owner.endOfBarrel.position, owner.transform.rotation);
				GameObject.Destroy(flash, 0.1f);

				var bullet = (GameObject)GameObject.Instantiate(owner.bulletPrefab, owner.endOfBarrel.position, owner.transform.rotation);
				GameObject.Destroy(bullet, 5.0f);

				owner.shotTimer = owner.shotCooldown;

                owner.GetComponent<AudioSource>().Play();
			}

			if (owner.attackingTimer <= 0)
			{
				Debug.Log ("Cooldown timer elapsed");
				owner.attackingTimer = owner.attackingCooldown;
				owner.shotTimer = owner.shotCooldown;

				if (owner.startScanningAngle != owner.endScanningAngle)
				{
					return State_Reseting;
				}

				return State_Scanning;
			}

			return State_Attacking;
		}
		
		public override void OnExit(TurretScript owner)
		{
		}
	}

	class ResetingState : State<TurretScript>
	{
		protected float endAngle, resetTimer;

		public ResetingState()
		{
		}
		
		public override void OnEnter(TurretScript owner)
		{
			endAngle = owner.scanningTowardsEnd? owner.endScanningAngle : owner.startScanningAngle;
		}
		
		public override int PerformStateAction(TurretScript owner)
		{
			resetTimer += Time.deltaTime;
			owner._transform.rotation = Quaternion.Slerp(owner._transform.rotation,
			                                             Quaternion.Euler(0, 0, endAngle),
			                                             resetTimer/owner.resetSpeed);

			if (resetTimer >= owner.resetSpeed)
			{
				owner.scanningTowardsEnd = !owner.scanningTowardsEnd;
				return State_Scanning;
			}

			return State_Reseting;
		}
		
		public override void OnExit(TurretScript owner)
		{
			resetTimer = 0;
		}
	}

	void Update()
	{
		stateMachine.performStateAction();
	}
}
