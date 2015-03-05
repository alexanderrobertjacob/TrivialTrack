using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    public StateMachine<PlayerScript> stateMachine;

    public const int State_None = -1;
    public const int State_Standing = 0;
    public const int State_Running = 1;
    public const int State_Jumping = 2;
    public const int State_Falling = 3;
    public const int State_PowerAttack = 4;
    public const int State_PowerAttackMoving = 5;
    public const int State_RunningVert = 6;

    public enum anim
    {
    }

    protected Transform _transform, _body;
    protected Rigidbody2D _rigidbody;
    protected CircleCollider2D _circlecollider;
    protected MoveScript _movescript;
    protected SwordScript _swordscript;
    //protected Animator _animator;

    public float runVel = 2.5f;
    public float jumpVel = 4f;
    public float swordSwingSpeed = 500f;
    public float powerAttackSpeedMod = 0.5f;
    public Vector3 currentFacing;
    public bool hasKey = false;
    public bool hasPlank = false;
    public AudioClip grunt;

    private int currentState;
    private double circleColliderSize;
    private float moveVel;
    private float distanceDown;
    private bool grounded;
    private double jumpWait;

    //TEST VARIABLES
    private bool swording = false;

    public virtual void Awake()
    {
        State<PlayerScript>[] statesList = new State<PlayerScript>[] { new StandingState(),
                                                                                 new RunningState(),
                                                                                 new JumpingState(),
                                                                                 new FallingState(),
                                                                                 new PowerAttackState(),
                                                                                 new PowerAttackMovingState(),
                                                                                 new RunningVertState()
                                                                                 };

        State<PlayerScript>[] facingStatesList = new State<PlayerScript>[] {    new RightFaceState(),
                                                                                new LeftFaceState(),
                                                                                new UpFaceState(),
                                                                                new DownFaceState()};


        _transform = transform;
        _rigidbody = this.GetComponentInParent<Rigidbody2D>();
        _circlecollider = this.GetComponentInChildren<CircleCollider2D>();
        _movescript = this.GetComponentInParent<MoveScript>();
        _swordscript = this.GetComponentInChildren<SwordScript>();
		_body = GetComponentInChildren<PlayerBodyScript>().transform;
        //_animator = this.GetComponent<Animator>();
        circleColliderSize = ((_circlecollider.bounds.size.y / 2) + .1);
        grounded = true;
        jumpWait = 0;

        stateMachine = new StateMachine<PlayerScript>(State_Standing, statesList, this);
        facingStateMachine = new StateMachine<PlayerScript>(State_RightFace, facingStatesList, this);
    }

    class StandingState : State<PlayerScript>
    {
        public StandingState()
        {
            actionList.Add("OnMoveSide", new StateAction(MoveSide));
            actionList.Add("OnJump", new StateAction(Jump));
            actionList.Add("OnAttack", new StateAction(Attack));
            actionList.Add("StartAttack", new StateAction(StartAttack));
            actionList.Add("ReleaseAttack", new StateAction(ReleaseAttack));
            actionList.Add("StartPowerAttack", new StateAction(StartPowerAttack));
            actionList.Add("OnMoveVert", new StateAction(MoveVert));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering standing state");
            owner.currentState = State_Standing;
            owner._movescript.UpdateXSpeed(0);
        }

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Standing);
            return State_Standing;
        }

        public override void OnExit(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.None);
            owner.currentState = State_None;
        }

        int MoveSide(PlayerScript owner, params object[] arg)
        {
            return State_Running;
        }

        int Jump(PlayerScript owner, params object[] arg)
        {
            return State_Jumping;
        }

        int Attack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.Attack();
            return State_Standing;
        }

        int StartAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.StartAttack();
            return State_Standing;
        }

        int ReleaseAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.ReleaseAttack();
            return State_Standing;
        }

        int StartPowerAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.StartPowerAttack();
            return State_PowerAttack;
        }

        int MoveVert(PlayerScript owner, params object[] arg)
        {
            return State_RunningVert;
        }
    }

    class RunningState : State<PlayerScript>
    {
        public RunningState()
        {
            actionList.Add("OnNoMove", new StateAction(NoMove));
            actionList.Add("OnJump", new StateAction(Jump));
            actionList.Add("OnAttack", new StateAction(Attack));
            actionList.Add("StartAttack", new StateAction(StartAttack));
            actionList.Add("ReleaseAttack", new StateAction(ReleaseAttack));
            actionList.Add("StartPowerAttack", new StateAction(StartPowerAttack));
            actionList.Add("OnMoveVert", new StateAction(MoveVert));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering running state");
            owner.currentState = State_Running;
            owner._movescript.UpdateXSpeed(owner.runVel);
        }

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Running);
            return State_Standing;
        }

        public override void OnExit(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.None);
            owner._movescript.UpdateXSpeed(0);
            owner.currentState = State_None;
        }

        int Attack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.Attack();
            return State_Standing;
        }

        int NoMove(PlayerScript owner, params object[] arg)
        {
            return State_Standing;
        }

        int Jump(PlayerScript owner, params object[] arg)
        {
            return State_Jumping;
        }

        int StartAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.StartAttack();
            return State_Running;
        }

        int ReleaseAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.ReleaseAttack();
            return State_Running;
        }

        int StartPowerAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.StartPowerAttack();
            return State_PowerAttackMoving;
        }

        int MoveVert(PlayerScript owner, params object[] arg)
        {
            return State_RunningVert;
        }
    }

    class RunningVertState : State<PlayerScript>
    {
        public RunningVertState()
        {
            actionList.Add("OnNoMove", new StateAction(NoMove));
            actionList.Add("OnJump", new StateAction(Jump));
            actionList.Add("OnAttack", new StateAction(Attack));
            actionList.Add("StartAttack", new StateAction(StartAttack));
            actionList.Add("ReleaseAttack", new StateAction(ReleaseAttack));
            actionList.Add("StartPowerAttack", new StateAction(StartPowerAttack));
            actionList.Add("OnMoveSide", new StateAction(MoveSide));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering running state");
            owner.currentState = State_Running;
            owner._movescript.UpdateYSpeed(owner.runVel);
        }

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Running);
            return State_Standing;
        }

        public override void OnExit(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.None);
            owner._movescript.UpdateYSpeed(0);
            owner.currentState = State_None;
        }

        int Attack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.Attack();
            return State_Standing;
        }

        int NoMove(PlayerScript owner, params object[] arg)
        {
            return State_Standing;
        }

        int Jump(PlayerScript owner, params object[] arg)
        {
            return State_Jumping;
        }

        int StartAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.StartAttack();
            return State_Running;
        }

        int ReleaseAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.ReleaseAttack();
            return State_Running;
        }

        int StartPowerAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.StartPowerAttack();
            return State_PowerAttackMoving;
        }

        int MoveSide(PlayerScript owner, params object[] arg)
        {
            return State_Running;
        }
    }

    class FallingState : State<PlayerScript>
    {
        public FallingState()
        {
            actionList.Add("NoJump", new StateAction(NoJump));
            actionList.Add("OnMoveSide", new StateAction(MoveSide));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering falling state");
            owner.currentState = State_Falling;
        }

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Jumping);
            return State_Falling;
        }

        public override void OnExit(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.None);
            owner.currentState = State_None;
        }

        int NoJump(PlayerScript owner, params object[] arg)
        {
            return State_Standing;
        }

        int MoveSide(PlayerScript owner, params object[] arg)
        {
            return State_Running;
        }
    }

    class JumpingState : State<PlayerScript>
    {
        public JumpingState()
        {
            actionList.Add("OnFall", new StateAction(Fall));
            actionList.Add("OnMoveSide", new StateAction(MoveSide));
            actionList.Add("OnAttack", new StateAction(Attack));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering jumping state");
            owner.currentState = State_Jumping;
            owner._movescript.ChangeYDirection(1);
            owner._movescript.AddSpeed(new Vector2(0, owner.jumpVel));
        }

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Jumping);

            return State_Jumping;
        }
        public override void OnExit(PlayerScript owner)
        {
            owner.currentState = State_None;
            base.OnExit(owner);
        }

        int Fall(PlayerScript owner, params object[] arg)
        {
            return State_Falling;
        }

        int MoveSide(PlayerScript owner, params object[] arg)
        {
            return State_Running;
        }

        int Attack(PlayerScript owner, params object[] arg)
        {
            //owner.SwingSword();
            owner.swording = true;
            return State_Standing;
        }
    }

    class PowerAttackState : State<PlayerScript>
    {
        public PowerAttackState()
        {
            actionList.Add("OnMoveSide", new StateAction(MoveSide));
            actionList.Add("ReleaseAttack", new StateAction(ReleaseAttack));
            actionList.Add("MaxPowerAttack", new StateAction(MaxPowerAttack));
            actionList.Add("StopPowerAttack", new StateAction(StopPowerAttack));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering power attack state");
            owner._movescript.UpdateXSpeed(0);
        }

        public override int PerformStateAction(PlayerScript owner)
        {

            return State_PowerAttack;
        }

        public override void OnExit(PlayerScript owner)
        {
            owner.currentState = State_None;

        }

        int MoveSide(PlayerScript owner, params object[] arg)
        {
            return State_PowerAttackMoving;
        }

        int ReleaseAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.ReleaseAttack();
            return State_Standing;
        }

        int StopPowerAttack(PlayerScript owner, params object[] arg)
        {
            return State_Standing;
        }

        int MaxPowerAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.MaxPowerAttack();
            return State_PowerAttack;
        }
    }

    class PowerAttackMovingState : State<PlayerScript>
    {
        public PowerAttackMovingState()
        {
            actionList.Add("OnNoMove", new StateAction(NoMove));
            actionList.Add("ReleaseAttack", new StateAction(ReleaseAttack));
            actionList.Add("MaxPowerAttack", new StateAction(MaxPowerAttack));
            actionList.Add("StopPowerAttack", new StateAction(StopPowerAttack));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering power attack state");
            owner._movescript.UpdateXSpeed(owner.runVel * owner.powerAttackSpeedMod);
        }

        public override int PerformStateAction(PlayerScript owner)
        {

            return State_PowerAttackMoving;
        }

        public override void OnExit(PlayerScript owner)
        {
            owner.currentState = State_None;

        }

        int NoMove(PlayerScript owner, params object[] arg)
        {
            return State_PowerAttack;
        }

        int ReleaseAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.ReleaseAttack();
            return State_PowerAttackMoving;
        }

        int StopPowerAttack(PlayerScript owner, params object[] arg)
        {
            return State_Standing;
        }

        int MaxPowerAttack(PlayerScript owner, params object[] arg)
        {
            owner._swordscript.MaxPowerAttack();
            return State_PowerAttackMoving;
        }
    }

    /**
     * The following code defines a state machine for the facing of the player.
     * This is independant of the rest of the states, because every state will
     * have the exact same functionality when facing the other direction,
     * simply changing the direction of facing and actions.
     * */

    public StateMachine<PlayerScript> facingStateMachine;

    public const int State_RightFace = 0;
    public const int State_LeftFace = 1;
    public const int State_UpFace = 2;
    public const int State_DownFace = 3;

    class RightFaceState : State<PlayerScript>
    {
        public RightFaceState()
        {
            actionList.Add("OnFaceLeft", new StateAction(FaceLeft));
            actionList.Add("OnFaceDown", new StateAction(FaceDown));
            actionList.Add("OnFaceUp", new StateAction(FaceUp));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering right facing state");
            owner._movescript.ChangeXDirection(1);
			owner.currentFacing = new Vector3(0, 0, 0);
			owner._body.localScale = new Vector3(-Mathf.Abs(owner._transform.localScale.x),
			                                          owner._transform.localScale.y,
			                                          owner._transform.localScale.z);
//			Debug.Log ("Facing right");
        }

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._rigidbody.velocity = new Vector2(owner.moveVel, owner._rigidbody.velocity.y);
            //owner._rigidbody.velocity = new Vector2((owner._rigidbody.velocity.x + (owner.moveVel * Time.deltaTime)), owner._rigidbody.velocity.y);
            return State_RightFace;
        }

        int FaceLeft(PlayerScript owner, params object[] arg)
        {
            return State_LeftFace;
        }

        int FaceUp(PlayerScript owner, params object[] arg)
        {
            return State_UpFace;
        }

        int FaceDown(PlayerScript owner, params object[] arg)
        {
            return State_DownFace;
        }
    }

    class LeftFaceState : State<PlayerScript>
    {
        public LeftFaceState()
        {
            actionList.Add("OnFaceRight", new StateAction(FaceRight));
            actionList.Add("OnFaceDown", new StateAction(FaceDown));
            actionList.Add("OnFaceUp", new StateAction(FaceUp));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering left facing state");

            owner._movescript.ChangeXDirection(-1);
			owner.currentFacing = new Vector3(0, 0, 180);
			owner._body.localScale = new Vector3(Mathf.Abs(owner._transform.localScale.x),
			                                          owner._transform.localScale.y,
			                                          owner._transform.localScale.z);
//			Debug.Log ("Facing left");

		}

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._rigidbody.velocity = new Vector2(0 - owner.moveVel, owner._rigidbody.velocity.y);
            //owner._rigidbody.velocity = new Vector2((owner._rigidbody.velocity.x + (0 - owner.moveVel * Time.deltaTime)), owner._rigidbody.velocity.y);
            return State_LeftFace;
        }

        public override void OnExit(PlayerScript owner)
        {
            //owner._transform.localScale = new Vector3(0 - owner._transform.localScale.x,
            //                                          owner._transform.localScale.y,
            //                                          owner._transform.localScale.z);
        }

        int FaceRight(PlayerScript owner, params object[] arg)
        {
            return State_RightFace;
        }

        int FaceUp(PlayerScript owner, params object[] arg)
        {
            return State_UpFace;
        }

        int FaceDown(PlayerScript owner, params object[] arg)
        {
            return State_DownFace;
        }
    }

    class UpFaceState : State<PlayerScript>
    {
        public UpFaceState()
        {
            actionList.Add("OnFaceRight", new StateAction(FaceRight));
            actionList.Add("OnFaceLeft", new StateAction(FaceLeft));
            actionList.Add("OnFaceDown", new StateAction(FaceDown));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering up facing state");
            // owner._transform.localScale = new Vector3(0 - owner._transform.localScale.x,
            //                                          owner._transform.localScale.y,
            //                                          owner._transform.localScale.z);

            owner._movescript.ChangeYDirection(1);
            //owner._transform.localScale = new Vector3(1, 1, owner._transform.localScale.z);
			owner.currentFacing = new Vector3(0, 0, 90);
        }

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._rigidbody.velocity = new Vector2(0 - owner.moveVel, owner._rigidbody.velocity.y);
            //owner._rigidbody.velocity = new Vector2((owner._rigidbody.velocity.x + (0 - owner.moveVel * Time.deltaTime)), owner._rigidbody.velocity.y);
            return State_UpFace;
        }

        public override void OnExit(PlayerScript owner)
        {

        }

        int FaceRight(PlayerScript owner, params object[] arg)
        {
            return State_RightFace;
        }

        int FaceLeft(PlayerScript owner, params object[] arg)
        {
            return State_LeftFace;
        }

        int FaceDown(PlayerScript owner, params object[] arg)
        {
            return State_DownFace;
        }
    }

    class DownFaceState : State<PlayerScript>
    {
        public DownFaceState()
        {
            actionList.Add("OnFaceRight", new StateAction(FaceRight));
            actionList.Add("OnFaceLeft", new StateAction(FaceLeft));
            actionList.Add("OnFaceUp", new StateAction(FaceUp));
        }

        public override void OnEnter(PlayerScript owner)
        {
//            Debug.Log("Entering down facing state");
            // owner._transform.localScale = new Vector3(0 - owner._transform.localScale.x,
            //                                          owner._transform.localScale.y,
            //                                          owner._transform.localScale.z);

            owner._movescript.ChangeYDirection(-1);
            //owner._transform.localScale = new Vector3(1, -1, owner._transform.localScale.z);
			owner.currentFacing = new Vector3(0, 0, 270);
        }

        public override int PerformStateAction(PlayerScript owner)
        {
            //owner._rigidbody.velocity = new Vector2(0 - owner.moveVel, owner._rigidbody.velocity.y);
            //owner._rigidbody.velocity = new Vector2((owner._rigidbody.velocity.x + (0 - owner.moveVel * Time.deltaTime)), owner._rigidbody.velocity.y);
            return State_DownFace;
        }

        public override void OnExit(PlayerScript owner)
        {
            //owner._transform.localScale = new Vector3(owner._transform.localScale.x,
            //                                          0 - owner._transform.localScale.y,
            //                                          owner._transform.localScale.z);

        }

        int FaceRight(PlayerScript owner, params object[] arg)
        {
            return State_RightFace;
        }

        int FaceLeft(PlayerScript owner, params object[] arg)
        {
            return State_LeftFace;
        }

        int FaceUp(PlayerScript owner, params object[] arg)
        {
            return State_UpFace;
        }
    }

    //END FACING STATE MACHINE

    //OTHER METHODS

    bool countPowerAttack = false;
    float powerAttackTimer = 0;

    void Update()
    {
        stateMachine.performStateAction();
        facingStateMachine.performStateAction();

        if (/*Input.GetKey(KeyCode.D) ||*/ Input.GetAxis("Horizontal") > 0.1)
        {
            object[] arg = new object[0];
            stateMachine.sendActionMessage("OnMoveSide", arg);
            if (!swording)
                facingStateMachine.sendActionMessage("OnFaceRight", arg);
        }

        if (Input.GetKeyUp(KeyCode.D))// || (Input.GetAxis("Horizontal") < 0.1 && Input.GetAxis("Horizontal") > -0.1))
        {
            object[] arg = new object[0];

            if (!Input.GetKey(KeyCode.A))
                stateMachine.sendActionMessage("OnNoMove", arg);
        }

        if (/*Input.GetKey(KeyCode.A) ||*/ Input.GetAxis("Horizontal") < -0.1)
        {
            object[] arg = new object[0];
            stateMachine.sendActionMessage("OnMoveSide", arg);
            if(!swording)
                facingStateMachine.sendActionMessage("OnFaceLeft", arg);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            object[] arg = new object[0];

            if (!Input.GetKey(KeyCode.D))
                stateMachine.sendActionMessage("OnNoMove", arg);
        }

        if (/*Input.GetKey(KeyCode.W) ||*/ Input.GetAxis("Vertical") > 0.1)
        {
            object[] arg = new object[0];
            stateMachine.sendActionMessage("OnMoveVert", arg);
            if (!swording)
                facingStateMachine.sendActionMessage("OnFaceUp", arg);
        }

        if (Input.GetKeyUp(KeyCode.W))// || (Input.GetAxis("Vertical") < 0.1 && Input.GetAxis("Vertical") > -0.1))
        {
            object[] arg = new object[0];

            if (!Input.GetKey(KeyCode.S))
                stateMachine.sendActionMessage("OnNoMove", arg);
        }

        if (/*Input.GetKey(KeyCode.S) ||*/ Input.GetAxis("Vertical") < -0.1)
        {
            object[] arg = new object[0];
            stateMachine.sendActionMessage("OnMoveVert", arg);
            if (!swording)
                facingStateMachine.sendActionMessage("OnFaceDown", arg);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            object[] arg = new object[0];

            if (!Input.GetKey(KeyCode.W))
                stateMachine.sendActionMessage("OnNoMove", arg);
        }

        if (/*Input.GetKeyDown(KeyCode.F) ||*/ Input.GetButtonDown("SwingSword"))
        {
            object[] arg = new object[0];
            stateMachine.sendActionMessage("OnAttack", arg);
        }

        //if (countPowerAttack)
        //{
        //    powerAttackTimer += Time.deltaTime;
        //    if (powerAttackTimer > 0.5)
        //    {
        //        object[] arg = new object[0];
        //        stateMachine.sendActionMessage("StartPowerAttack", arg);
        //    }
        //}

        //if (powerAttackTimer > SwordScript.DELAY_FOR_NORMAL_ATTACK && powerAttackTimer < _swordscript.powerAttackThreshhold)
        //{
        //    object[] arg = new object[0];
        //    stateMachine.sendActionMessage("StartPowerAttack", arg);
        //}
        //else if (powerAttackTimer >= _swordscript.powerAttackThreshhold)
        //{
        //    object[] arg = new object[0];
        //    stateMachine.sendActionMessage("MaxPowerAttack", arg);
        //}

        //if (Input.GetKeyUp(KeyCode.F))
        //{
        //    powerAttackTimer = 0;
        //    countPowerAttack = false;
        //    object[] arg = new object[0];
        //    stateMachine.sendActionMessage("ReleaseAttack", arg);
        //}



        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    object[] arg = new object[0];

        //    stateMachine.sendActionMessage("OnJump", arg);
        //}

        //if (jumpWait > 0)
        //{
        //    jumpWait -= 0.1;
        //}
        //else
        //{
        //    RaycastHit2D hitDown = Physics2D.Linecast(_transform.position, new Vector2(_transform.position.x, _transform.position.y - 25), 1 << 8);
        //    Debug.Log("Distance down: " + distanceDown);
        //    Debug.Log("Collider pos: " + circleColliderSize);
        //    distanceDown = Mathf.Abs(hitDown.point.y - _transform.position.y);
        //    if (distanceDown < circleColliderSize)
        //    {
        //        grounded = true;
        //    }
        //    else
        //    {
        //        grounded = false;
        //        jumpWait = 1;
        //    }
        //}

        //if (grounded == true && Input.GetKeyDown(KeyCode.Space))
        //{
        //    object[] arg = new object[0];

        //    stateMachine.sendActionMessage("OnJump", arg);
        //}
        //else if (grounded == true)
        //{
        //    object[] arg = new object[0];

        //    stateMachine.sendActionMessage("NoJump", arg);

        //}

        //if (_rigidbody.velocity.y < 0)
        //{
        //    object[] arg = new object[0];

        //    stateMachine.sendActionMessage("OnFall", arg);
        //}

    }

    public void FixedUpdate()
    {

    }

}
