using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeGame : MonoBehaviour
{
    [SerializeField]
    private ArcadeUI _arcadeUI;
    public ArcadeUI UI { get { return _arcadeUI; } }

    [SerializeField]
    private ArcadeActors _arcadeActors;
    public ArcadeActors Actors { get { return _arcadeActors; } }

    [SerializeField]
    private ArcadeData _arcadeData;
    public ArcadeData Data { get { return _arcadeData; } }

    [SerializeField]
    private int _continueDuration = 15;
    public int ContinueDuration { get { return _continueDuration; } }

    public enum State
    {
        MainGame,
        Continue,
        GameOver
    }
    private StateMachine<State> _stateMachine;

    public State CurrentState { get { return _stateMachine.CurrentState; } }

    // Start is called before the first frame update
    void Start()
    {
        _arcadeData.ContinueTimer = -1f;
        _arcadeData.Score = 0;

        _stateMachine = new StateMachine<State>();
        _stateMachine
            .AddState(new MainGameState(State.MainGame, this).AddTransition(State.Continue, () => Actors.Ship.Life <= 0 && _arcadeData.Credits <= 0))
            .AddState(new ContinueState(State.Continue, this)
                .AddTransition(State.GameOver, () => _arcadeData.ContinueTimer <= 0f)
                .AddTransition(State.MainGame, () => _arcadeData.Credits > 0))
            .AddState(new GameOverState(State.GameOver, this).AddTransition(State.MainGame, () => Actors.Ship.RespawnEnabled && _arcadeData.Credits > 0, () => _arcadeData.Score = 0), true);
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();

        _arcadeUI.Score = _arcadeData.Score;
        _arcadeUI.Credits = _arcadeData.Credits;
        _arcadeUI.ContinueTimer = _arcadeData.ContinueTimer;
    }

    public void AddCredit(int count = 1)
    {
        _arcadeData.Credits += count;
    }

    #region Inputs
    public void OnCredit(UnityEngine.InputSystem.InputValue v)
    {
        AddCredit();
    }

    public void ToggleControls(bool toggle)
    {
        // Disabling the InputActionMap from InputActions doesn't work strangely, so I'm doing it the dirty way
        UnityEngine.InputSystem.PlayerInput input = Actors.Ship.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (input)
        {
            input.enabled = toggle;
        }
    }
    #endregion

    #region States
    public abstract class ArcadeState : AState<State>
    {
        public ArcadeGame ArcadeGame { get; private set; }
        public ArcadeState(State stateName, ArcadeGame arcadeGame) : base(stateName)
        {
            ArcadeGame = arcadeGame;
        }
    }

    public class MainGameState : ArcadeState
    {
        public MainGameState(State stateName, ArcadeGame arcadeGame) : base(stateName, arcadeGame) { }

        protected override void OnStateEnter()
        {
            ArcadeGame.Data.ContinueTimer = -1f;
            ArcadeGame.Actors.AlienGenerator.gameObject.SetActive(true);
        }

        protected override void OnStateUpdate()
        {
            if (ArcadeGame.Actors.Ship.Life <= 0 && ArcadeGame.Data.Credits > 0 && ArcadeGame.Actors.Ship.RespawnEnabled)
            {
                ArcadeGame.AddCredit(-1);
                ArcadeGame.Actors.Ship.Respawn();
            }
        }
    }

    public class ContinueState : ArcadeState
    {
        public ContinueState(State stateName, ArcadeGame arcadeGame) : base(stateName, arcadeGame) { }

        protected override void OnStateEnter()
        {
            ArcadeGame.Data.ContinueTimer = ArcadeGame.ContinueDuration;
            ArcadeGame.Actors.AlienGenerator.DestroyAllAliens();
            ArcadeGame.Actors.AlienGenerator.gameObject.SetActive(false);
        }

        protected override void OnStateUpdate()
        {
            ArcadeGame.Data.ContinueTimer -= Time.deltaTime;
        }
    }

    public class GameOverState : ArcadeState
    {
        public GameOverState(State stateName, ArcadeGame arcadeGame) : base(stateName, arcadeGame) { }

        protected override void OnStateEnter()
        {
            ArcadeGame.Actors.Ship.Destroy(true);
            ArcadeGame.Actors.AlienGenerator.gameObject.SetActive(false);
            ArcadeGame.UI.InsertCoinUIEnabled = true;
        }

        protected override void OnStateExit()
        {
            ArcadeGame.UI.InsertCoinUIEnabled = false;
        }
    }
    #endregion
}
