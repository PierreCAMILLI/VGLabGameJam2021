using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeAlien : ArcadeActor, IPopPool, IPushPool
{
    private Animator _animator;

    [SerializeField]
    private ArcadeData _arcadeData;

    [Space]
    [SerializeField]
    private float _maxSpeed;
    public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }

    [SerializeField]
    private float _moveRadius;
    public float MoveRadius { get { return _moveRadius; } set { _moveRadius = value; } }

    [SerializeField]
    private float _idleMaxDuration;
    public float IdleMaxDuration { get { return _idleMaxDuration; } set { _idleMaxDuration = value; } }

    [SerializeField]
    private int _pointsEarned;

#if UNITY_EDITOR
    [Space]
    [SerializeField]
    private bool _activateTest;
    [SerializeField]
    private AlienBullet _alienBullet;
    [SerializeField]
    private Transform _testTarget;
#endif

    public Collider2D Collider { get; set; }
    public bool Destroyed { get; set; }
    public Transform Target { get; set; }
    public PoolObject<ArcadeAlien> AlienPool { get; set; }
    public PoolObject<AlienBullet> BulletPool { get; set; }

    public enum State
    {
        Idle,
        Moving,
        Destroyed
    }
    private StateMachine<State> _stateMachine;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        Collider = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        if (_activateTest)
        {
            Destroyed = false;
            BulletPool = new PoolObject<AlienBullet>(_alienBullet);
            BulletPool.SetOnPop(ab => ab.Pool = BulletPool);
            Target = _testTarget;
        }
#endif

        _stateMachine = new StateMachine<State>();

        _stateMachine
            .AddState(new IdleState(State.Idle, this).AddTransition(State.Destroyed, () => Destroyed))
            .AddState(new MovingState(State.Moving, this).AddTransition(State.Destroyed, () => Destroyed), true)
            .AddState(new DestroyedState(State.Destroyed, this).AddTransition(State.Moving, () => !Destroyed));
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();

        _animator.SetBool("Destroyed", Destroyed);
    }

    public void Shoot()
    {
        AlienBullet bullet = BulletPool.Pop();
        bullet.transform.parent = transform.parent;
        bullet.transform.localScale = Vector3.one;
        bullet.transform.position = transform.position;
        bullet.Forward = (Target.position - transform.position).normalized;
        bullet.Pool = BulletPool;
    }

    public void Spawn(Vector2 position)
    {
        transform.position = (Vector3)position + (Vector3.forward * transform.position.z);
        Destroyed = false;
        Collider.enabled = true;
    }

    public void Destroy(bool addPoint = true)
    {
        Destroyed = true;
        Collider.enabled = false;
        if (addPoint)
        {
            _arcadeData.Score += _pointsEarned;
        }
    }

    public void PushInPool()
    {
        AlienPool.Push(this);
    }

    #region Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _moveRadius);
    }

    public void OnPush()
    {
        gameObject.SetActive(false);
    }

    public void OnPop()
    {
        gameObject.SetActive(true);
    }
    #endregion

    #region States
    public abstract class AlienState : AState<State>
    {
        public ArcadeAlien Alien { get; private set; }

        public AlienState(State stateName, ArcadeAlien alien) : base(stateName) 
        {
            Alien = alien;
        }
    }

    public class IdleState : AlienState
    {
        private float _idleDuration;

        public IdleState(State stateName, ArcadeAlien alien) : base(stateName, alien) { }

        protected override void OnStateEnter()
        {
            _idleDuration = Alien.IdleMaxDuration;
            Alien.Shoot();
        }

        protected override void OnStateUpdate()
        {
            _idleDuration -= Time.deltaTime;
            if (_idleDuration <= 0f)
            {
                NextState = State.Moving;
            }
        }
    }

    public class MovingState : AlienState
    {
        private Vector3 _targetPosition;

        public MovingState(State stateName, ArcadeAlien alien) : base(stateName, alien) { }

        protected override void OnStateEnter()
        {
            Vector3 direction = Alien.Target.position - Alien.transform.position;
            _targetPosition = Alien.Target.position - (direction.normalized * Alien.MoveRadius);

            direction = _targetPosition - Alien.transform.position;
            if (direction.sqrMagnitude > (Alien.MoveRadius * Alien.MoveRadius))
            {
                _targetPosition = Alien.transform.position + (direction.normalized * Alien.MoveRadius);
            }
        }

        protected override void OnStateUpdate()
        {
            Alien.transform.position = Vector3.MoveTowards(Alien.transform.position, _targetPosition, Alien.MaxSpeed * Time.deltaTime);
            if (Alien.transform.position == _targetPosition)
            {
                NextState = State.Idle;
            }
        }
    }

    public class DestroyedState : AlienState
    {
        public DestroyedState(State stateName, ArcadeAlien alien) : base(stateName, alien) { }

        protected override void OnStateEnter()
        {
            Alien.Collider.enabled = false;
        }

        protected override void OnStateExit()
        {
            Alien.Collider.enabled = true;
        }
    }
    #endregion
}
