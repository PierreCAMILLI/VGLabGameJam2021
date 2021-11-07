using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeShip : ArcadeActor
{
    private Animator _animator;

    [SerializeField]
    private ShipBullet _bulletPrefab;

    [SerializeField]
    private Transform _shipHead;

    [Space]
    [SerializeField]
    private float _maxSpeed;
    public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }

    [SerializeField]
    private int _maxLife;
    public int MaxLife { get { return _maxLife; } set { _maxLife = value; } }

    [SerializeField]
    [Range(1,10)]
    private int _maxBulletCount;
    public int MaxBulletCount { get { return _maxBulletCount; } set { _maxBulletCount = value; } }

    [Space]
    [SerializeField]
    private float _damageInvincibleDuration;
    public float DamageInvincibleDuration { get { return _damageInvincibleDuration; } set { _damageInvincibleDuration = value; } }

    [SerializeField]
    private float _respawnInvincibleDuration;
    public float RespawnInvincibleDuration { get { return _respawnInvincibleDuration; } set { _respawnInvincibleDuration = value; } }

    public Collider2D Collider { get; set; }
    public Vector2 Direction { get; private set; }
    public bool ShootEnabled { get; set; }
    public bool RespawnEnabled { get; set; }
    public int Life { get; set; }
    public bool ForceInvincible { get; set; }
    public float InvincibleDuration { get; set; }
    public int BulletCount { get; set; }

    private PoolObject<ShipBullet> _bulletPool;
    private Vector3 _startPosition;

    public bool IsInvincible { get { return ForceInvincible || InvincibleDuration >= 0f; } }

    public enum State
    {
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
        Direction = Vector2.zero;
        ShootEnabled = false;
        RespawnEnabled = true;
        ForceInvincible = false;
        _startPosition = transform.position;
        _bulletPool = new PoolObject<ShipBullet>(_bulletPrefab);
        _bulletPool.SetOnPop(sb => {
            sb.transform.parent = transform.parent;
            sb.transform.localScale = Vector3.one;
            sb.transform.position = _shipHead.position;
            sb.Pool = _bulletPool;

            BulletCount += 1;
            });
        _bulletPool.SetOnPush(sb => {
            BulletCount -= 1;
            });

        Respawn();

        _stateMachine = new StateMachine<State>();
        _stateMachine
            .AddState(new MovingState(State.Moving, this).AddTransition(State.Destroyed, () => { return Life <= 0; }), true)
            .AddState(new DestroyedState(State.Destroyed, this).AddTransition(State.Moving, () => { return Life > 0; }));
    }

    // Update is called once per frame
    void Update()
    {
        InvincibleDuration -= Time.deltaTime;

        _stateMachine.Update();

        _animator.SetInteger("Life", Life);
        _animator.SetBool("Invincible", IsInvincible);
    }

    public void Move(Vector2 direction)
    {
        Direction = direction;
    }

    public void Shoot()
    {
        if (ShootEnabled && BulletCount < MaxBulletCount)
        {
            _bulletPool.Pop();
        }
    }

    public void Respawn()
    {
        if (RespawnEnabled)
        {
            transform.position = _startPosition;
            Heal();
            InvincibleDuration = _respawnInvincibleDuration;
        }
    }

    public void Heal()
    {
        Life = _maxLife;
    }

    public bool Damage(int power = 1, bool invincibleTime = true, bool ignoreInvincible = false)
    {
        if (!IsInvincible || ignoreInvincible)
        {
            Life -= power;
            if (invincibleTime)
            {
                InvincibleDuration = _damageInvincibleDuration;
            }
            return true;
        }
        return false;
    }

    public void Destroy(bool ignoreInvincible = false)
    {
        if (!IsInvincible || ignoreInvincible)
        {
            Life = 0;
        }
    }

    #region Inputs
    public void OnDirection(InputValue value)
    {
        Move(value.Get<Vector2>());
    }

    public void OnShoot(InputValue value)
    {
        Shoot();
    }
    #endregion

#if UNITY_EDITOR
    #region Editor
    private void OnGUI()
    {
        //if (GUI.Button(new Rect(10, 10, 150, 50), "Respawn"))
        //{
        //    Respawn();
        //}

        //if (GUI.Button(new Rect(10, 70, 150, 50), "Damage"))
        //{
        //    Damage();
        //}

        //if (GUI.Button(new Rect(10, 130, 150, 50), "Destroy"))
        //{
        //    Destroy();
        //}
    }
    #endregion
#endif

    #region States
    public abstract class ShipState : AState<State>
    {
        public ArcadeShip Ship { get; private set; }

        public ShipState(State stateName, ArcadeShip ship) : base(stateName)
        {
            Ship = ship;
        }
    }

    public class MovingState : ShipState
    {
        public MovingState(State stateName, ArcadeShip ship) : base(stateName, ship) { }

        protected override void OnStateEnter()
        {
            Ship.Collider.enabled = true;
            Ship.ShootEnabled = true;
        }

        protected override void OnStateUpdate()
        {
            Ship.transform.position += (Vector3)Ship.Direction * Time.deltaTime * Ship.MaxSpeed;
        }

        protected override void OnStateExit()
        {
            Ship.ShootEnabled = false;
        }
    }

    public class DestroyedState : ShipState
    {
        public DestroyedState(State stateName, ArcadeShip ship) : base(stateName, ship) { }

        protected override void OnStateEnter()
        {
            Ship.Collider.enabled = false;
            Ship.Life = 0;
            Ship.RespawnEnabled = false;
        }

        protected override void OnStateUpdate()
        {
        }
    }
    #endregion
}
