using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    private RectTransform _pauseTransform;
    public RectTransform PauseTransform { get { return _pauseTransform; } }

    [SerializeField]
    private RectTransform _loadingTransform;
    public RectTransform LoadingTransform { get { return _loadingTransform; } }

    private enum State
    {
        MainMenu,
        Loading,
        MainGame,
        Pause
    }
    private StateMachine<State> _stateMachine;

    public AsyncOperation AsyncSceneLoading { get; set; }

    public bool PauseEnabled { get; set; }

    public BooleanTrigger RequestLaunchGame { get; set; }
    public BooleanTrigger RequestPause { get; set; }
    public BooleanTrigger RequestResumeGame { get; set; }
    public BooleanTrigger RequestLeaveGame { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        RequestLaunchGame = new BooleanTrigger();
        RequestPause = new BooleanTrigger();
        RequestResumeGame = new BooleanTrigger();
        RequestLeaveGame = new BooleanTrigger();

        _stateMachine = new StateMachine<State>();
        _stateMachine
            .AddState(new MainMenuState(State.MainMenu, this).AddTransition(State.Loading, () => RequestLaunchGame.Get(), LoadGame), true)
            .AddState(new LoadingState(State.Loading, this).AddTransition(State.MainGame, () => AsyncSceneLoading != null && AsyncSceneLoading.isDone))
            .AddState(new MainGameState(State.MainGame, this).AddTransition(State.Pause, () => RequestPause.Get()))
            .AddState(new PauseState(State.Pause, this)
                .AddTransition(State.MainGame, () => RequestResumeGame.Get())
                .AddTransition(State.MainMenu, () => RequestLeaveGame.Get()));
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
    }

    private void LoadGame()
    {
        AsyncSceneLoading = SceneManager.LoadSceneAsync("MainGame");
    }

    public void LaunchGame()
    {
        RequestLaunchGame.Set();
    }

    public void BackToMenu()
    {
        RequestLeaveGame.Set();
    }

    public void Pause()
    {
        if (PauseEnabled)
        {
            RequestPause.Set();
        }
    }

    public void Resume()
    {
        RequestResumeGame.Set();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #region States
    private abstract class GameState : AState<State>
    {
        public GameManager Game { get; private set; }

        public GameState(State stateName, GameManager gameManager) : base(stateName)
        {
            Game = gameManager;
        }
    }

    private class MainMenuState : GameState
    {
        public MainMenuState(State stateName, GameManager gameManager) : base(stateName, gameManager) { }

        protected override void OnStateEnter()
        {
            SceneManager.LoadScene("MainMenu");

            Time.timeScale = 0f;
            Game.PauseEnabled = false;
        }
    }

    private class LoadingState : GameState
    {
        public LoadingState(State stateName, GameManager gameManager) : base(stateName, gameManager) { }

        protected override void OnStateEnter()
        {
            Time.timeScale = 0f;
            Game.PauseEnabled = false;

            Game.LoadingTransform.gameObject.SetActive(true);
        }

        protected override void OnStateExit()
        {
            Game.LoadingTransform.gameObject.SetActive(false);
        }
    }

    private class MainGameState : GameState
    {
        public MainGameState(State stateName, GameManager gameManager) : base(stateName, gameManager) { }

        protected override void OnStateEnter()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
            Game.PauseEnabled = true;
        }

    }

    private class PauseState : GameState
    {
        public PauseState(State stateName, GameManager gameManager) : base(stateName, gameManager) { }

        protected override void OnStateEnter()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
            Game.PauseEnabled = false;
            Game.PauseTransform.gameObject.SetActive(true);
        }

        protected override void OnStateExit()
        {
            Game.PauseTransform.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Inputs
    private void OnPause(UnityEngine.InputSystem.InputValue value)
    {
        if (_stateMachine.CurrentState == State.MainGame)
        {
            Pause();
        }
        else if (_stateMachine.CurrentState == State.Pause)
        {
            Resume();
        }
    }
    #endregion
}
