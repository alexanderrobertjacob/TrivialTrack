using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{

    public StateMachine<EnemyScript> stateMachine;

    public const int State_Standing = 0;
    public const int State_Running = 1;
    public const int State_Jumping = 2;
    public const int State_Falling = 3;
    public const int State_Fighting = 4;
    public const int State_Test = 5;

    public enum anim
    {
    }

    protected Transform _transform;
    protected Rigidbody2D _rigidbody;
    protected BoxCollider2D _boxcollider;
    protected MoveScript _movescript;
    protected SwordScript _swordscript;
    //protected Animator _animator;

    public float runVel = 2.5f;
    public float jumpVel = 4f;
    public float swordSwingSpeed = 500f;
    public float attackCooldown = 5f;

    private int currentState;
    private double boxColliderSize;
    private float moveVel;
    private float distanceDown;
    private bool grounded;
    private double jumpWait;

    //TEST VARIABLES
    private bool swording = false;

    public virtual void Awake()
    {
        State<EnemyScript>[] statesList = new State<EnemyScript>[] { new StandingState(),
                                                                                 new RunningState(),
                                                                                 new JumpingState(),
                                                                                 new FallingState(),
                                                                                 new FightingState()
                                                                                 };

        State<EnemyScript>[] facingStatesList = new State<EnemyScript>[] { new RightFaceState(),
                                                                             new LeftFaceState()};

        _transform = transform;
        _rigidbody = rigidbody2D;
        _boxcollider = this.GetComponentInChildren<BoxCollider2D>();
        _movescript = this.GetComponent<MoveScript>();
        _swordscript = this.GetComponentInChildren<SwordScript>();
        //_animator = this.GetComponent<Animator>();
        boxColliderSize = ((_boxcollider.bounds.size.y / 2) + .1);
        grounded = true;
        jumpWait = 0;

        stateMachine = new StateMachine<EnemyScript>(State_Standing, statesList, this);
        facingStateMachine = new StateMachine<EnemyScript>(State_RightFace, facingStatesList, this);
    }

    class StandingState : State<EnemyScript>
    {
        public StandingState()
        {
            actionList.Add("OnMoveSide", new StateAction(MoveSide));
            actionList.Add("OnJump", new StateAction(Jump));
            actionList.Add("OnAttack", new StateAction(Attack));
        }

        public override void OnEnter(EnemyScript owner)
        {
            //Debug.Log("Entering standing state");
            owner.moveVel = 0;
        }

        public override int PerformStateAction(EnemyScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Standing);
            return State_Standing;
        }

        public override void OnExit(EnemyScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.None);
        }

        int MoveSide(EnemyScript owner, params object[] arg)
        {
            return State_Running;
        }

        int Jump(EnemyScript owner, params object[] arg)
        {
            return State_Jumping;
        }

        int Attack(EnemyScript owner, params object[] arg)
        {
            owner._swordscript.Attack();
            return State_Standing;
        }
    }

    class RunningState : State<EnemyScript>
    {
        public RunningState()
        {
            actionList.Add("OnNoMove", new StateAction(NoMove));
            actionList.Add("OnJump", new StateAction(Jump));
            actionList.Add("OnAttack", new StateAction(Attack));
        }

        public override void OnEnter(EnemyScript owner)
        {
            //Debug.Log("Entering running state");
            owner._movescript.UpdateXSpeed(owner.runVel);
        }

        public override int PerformStateAction(EnemyScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Running);
            return State_Standing;
        }

        public override void OnExit(EnemyScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.None);
        }

        int NoMove(EnemyScript owner, params object[] arg)
        {
            return State_Standing;
        }

        int Jump(EnemyScript owner, params object[] arg)
        {
            return State_Jumping;
        }

        int Attack(EnemyScript owner, params object[] arg)
        {
            owner._swordscript.Attack();
            return State_Standing;
        }
    }

    class FallingState : State<EnemyScript>
    {
        public FallingState()
        {
            actionList.Add("NoJump", new StateAction(NoJump));
            actionList.Add("OnMoveSide", new StateAction(MoveSide));
        }

        public override void OnEnter(EnemyScript owner)
        {
            //Debug.Log("Entering falling state");
        }

        public override int PerformStateAction(EnemyScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Jumping);
            return State_Falling;
        }

        public override void OnExit(EnemyScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.None);
        }

        int NoJump(EnemyScript owner, params object[] arg)
        {
            return State_Standing;
        }

        int MoveSide(EnemyScript owner, params object[] arg)
        {
            return State_Running;
        }
    }

    class JumpingState : State<EnemyScript>
    {
        public JumpingState()
        {
            actionList.Add("OnFall", new StateAction(Fall));
            actionList.Add("OnMoveSide", new StateAction(MoveSide));
            actionList.Add("OnAttack", new StateAction(Attack));
        }

        public override void OnEnter(EnemyScript owner)
        {
            //Debug.Log("Entering jumping state");
            owner._rigidbody.velocity = new Vector2(owner.moveVel, owner.jumpVel);
        }

        public override int PerformStateAction(EnemyScript owner)
        {
            //owner._animator.SetInteger("RyuAnimState", (int)anim.Jumping);

            return State_Jumping;
        }
        public override void OnExit(EnemyScript owner)
        {
            base.OnExit(owner);
        }

        int Fall(EnemyScript owner, params object[] arg)
        {
            return State_Falling;
        }

        int MoveSide(EnemyScript owner, params object[] arg)
        {
            return State_Running;
        }

        int Attack(EnemyScript owner, params object[] arg)
        {
            //owner.SwingSword();
            owner.swording = true;
            return State_Standing;
        }
    }

    class FightingState : State<EnemyScript>
    {
        public FightingState()
        {
            actionList.Add("OnNoMove", new StateAction(NoMove));
        }

        public override void OnEnter(EnemyScript owner)
        {
            //Debug.Log("Entering fighting state");
            owner.moveVel = owner.runVel / 2;
        }

        public override int PerformStateAction(EnemyScript owner)
        {

            return State_Fighting;
        }

        public override void OnExit(EnemyScript owner)
        {

        }

        int NoMove(EnemyScript owner, params object[] arg)
        {
            return State_Standing;
        }
    }

    /**
     * The following code defines a state machine for the facing of the player.
     * This is independant of the rest of the states, because every state will
     * have the exact same functionality when facing the other direction,
     * simply changing the direction of facing and actions.
     * */

    public StateMachine<EnemyScript> facingStateMachine;

    public const int State_RightFace = 0;
    public const int State_LeftFace = 1;

    class RightFaceState : State<EnemyScript>
    {
        public RightFaceState()
        {
            actionList.Add("OnFaceLeft", new StateAction(FaceLeft));
        }

        public override void OnEnter(EnemyScript owner)
        {
//            Debug.Log("Entering right facing state");
            owner._transform.localScale = new Vector3(owner._transform.localScale.x, owner._transform.localScale.y, owner._transform.localScale.z);
        }

        public override int PerformStateAction(EnemyScript owner)
        {
            owner._rigidbody.velocity = new Vector2(owner.moveVel, owner._rigidbody.velocity.y);
            //owner._rigidbody.velocity = new Vector2((owner._rigidbody.velocity.x + (owner.moveVel * Time.deltaTime)), owner._rigidbody.velocity.y);
            return State_LeftFace;
        }

        int FaceLeft(EnemyScript owner, params object[] arg)
        {
            return State_LeftFace;
        }
    }

    class LeftFaceState : State<EnemyScript>
    {
        public LeftFaceState()
        {
            actionList.Add("OnFaceRight", new StateAction(FaceRight));
        }

        public override void OnEnter(EnemyScript owner)
        {
//            Debug.Log("Entering left facing state");
            // owner._transform.localScale = new Vector3(0 - owner._transform.localScale.x,
            //                                          owner._transform.localScale.y,
            //                                          owner._transform.localScale.z);

            owner._transform.localScale = new Vector3(0 - owner._transform.localScale.x, owner._transform.localScale.y, owner._transform.localScale.z);
        }

        public override int PerformStateAction(EnemyScript owner)
        {
            owner._rigidbody.velocity = new Vector2(0 - owner.moveVel, owner._rigidbody.velocity.y);
            //owner._rigidbody.velocity = new Vector2((owner._rigidbody.velocity.x + (0 - owner.moveVel * Time.deltaTime)), owner._rigidbody.velocity.y);
            return State_LeftFace;
        }

        public override void OnExit(EnemyScript owner)
        {
            owner._transform.localScale = new Vector3(0 - owner._transform.localScale.x,
                                                      owner._transform.localScale.y,
                                                      owner._transform.localScale.z);
        }

        int FaceRight(EnemyScript owner, params object[] arg)
        {
            return State_RightFace;
        }
    }

    //END FACING STATE MACHINE

    //OTHER METHODS

    void Update()
    {
        stateMachine.performStateAction();
        facingStateMachine.performStateAction();

        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0)
        {
            object[] arg = new object[0];
            stateMachine.sendActionMessage("OnAttack", arg);
            attackCooldown = 1f;
        }

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
        //    Debug.Log("Collider pos: " + boxColliderSize);
        //    distanceDown = Mathf.Abs(hitDown.point.y - _transform.position.y);
        //    if (distanceDown < boxColliderSize)
        //    {
        //        grounded = true;
        //    }
        //    else
        //    {
        //        grounded = false;
        //        jumpWait = 1;
        //    }
        //}

        if (_rigidbody.velocity.y < 0)
        {
            object[] arg = new object[0];

            stateMachine.sendActionMessage("OnFall", arg);
        }

    }

    public void FixedUpdate()
    {

    }

}
