using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HideAndSeek : MonoBehaviour
{
    [SerializeField]
    private ArcadeGame _arcadeGame;
    public ArcadeGame ArcadeGame { get { return _arcadeGame; } }

    [SerializeField]
    private HeroController _heroController;
    public HeroController HeroController { get { return _heroController; } }

    [SerializeField]
    private CoinSpawner _coinSpawner;
    public CoinSpawner CoinSpawner { get { return _coinSpawner; } }

    [Header("Music")]
    [SerializeField]
    private AudioSource _bgmSource;
    public AudioSource BGMSource { get { return _bgmSource; } }

    [SerializeField]
    private AudioClip _searchingMusic;
    public AudioClip SearchingMusic { get { return _searchingMusic; } }

    [SerializeField]
    private AudioClip _playingMusic;
    public AudioClip PlayingMusic { get { return _playingMusic; } }

    private enum State
    {
        Searching,
        Playing
    }
    private StateMachine<State> _stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        _stateMachine = new StateMachine<State>();
        _stateMachine
            .AddState(new SearchingState(State.Searching, this).AddTransition(State.Playing, () => _arcadeGame.CurrentState == ArcadeGame.State.MainGame), true)
            .AddState(new PlayingState(State.Playing, this).AddTransition(State.Searching, () => _arcadeGame.CurrentState != ArcadeGame.State.MainGame));

    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
    }

    #region States
    private  abstract class HaSState : AState<State>
    {
        public HideAndSeek Game { get; private set; }
        public HaSState(State stateName, HideAndSeek hideAndSeek) : base(stateName)
        {
            Game = hideAndSeek;
        }
    }

    private class SearchingState : HaSState
    {
        public SearchingState(State stateName, HideAndSeek hideAndSeek) : base(stateName, hideAndSeek) { }

        protected override void OnStateEnter()
        {
            Game.HeroController.ToggleControls(true);
            Game.ArcadeGame.ToggleControls(false);
            Game.CoinSpawner.SpawnCoinAtRandomLocation();

            Game.BGMSource.clip = Game.SearchingMusic;
            Game.BGMSource.Play();
        }

        protected override void OnStateExit()
        {
            Game.HeroController.ToggleControls(false);
        }
    }

    private class PlayingState : HaSState
    {
        public PlayingState(State stateName, HideAndSeek hideAndSeek) : base(stateName, hideAndSeek) { }

        protected override void OnStateEnter()
        {
            Game.HeroController.ToggleControls(false);
            Game.ArcadeGame.ToggleControls(true);

            Game.BGMSource.clip = Game.PlayingMusic;
            Game.BGMSource.Play();
        }

        protected override void OnStateExit()
        {
            Game.ArcadeGame.ToggleControls(false);
        }
    }
    #endregion
}
