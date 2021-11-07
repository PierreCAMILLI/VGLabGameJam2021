using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : struct
{
    public Dictionary<T, AState<T>> States { get; private set; }
    private AState<T> _currentState;
    public T CurrentState { get { return _currentState.StateName; } }
    private bool initiated;

    public StateMachine()
    {
        States = new Dictionary<T, AState<T>>();
        _currentState = null;
        initiated = false;
    }

    public void Update()
    {
        if (!initiated)
        {
            if (_currentState != null)
            {
                foreach (KeyValuePair<T, AState<T>> state in States)
                {
                    state.Value.Init();
                }
                _currentState.Enter();
                initiated = true;
            }
            else
            {
                Debug.LogError("State Machine doesn't have any state set as start state");
                return;
            }
        }

        bool hasNextState = false;
        do
        {
            _currentState.Update();
            hasNextState = _currentState.HasNextState();
            if (hasNextState)
            {
                _currentState.Exit();
                if (!States.ContainsKey(_currentState.NextState))
                {
                    Debug.LogError("State Machine doesn't contains state named " + _currentState.NextState.ToString());
                    return;
                }
                _currentState = States[_currentState.NextState];
                _currentState.Enter();
            }
        } while (hasNextState);
    }

    public StateMachine<T> AddState(AState<T> state, bool isStartState = false)
    {
        if (States.ContainsKey(state.StateName))
        {
            Debug.LogError("StateMachine already has state of name: " + state.StateName.ToString());
        }
        else
        {
            States.Add(state.StateName, state);
            if (isStartState)
            {
                _currentState = state;
            }
        }
        return this;
    }
}

public abstract class AState<T> where T : struct
{
    private struct TransitionState
    {
        public System.Func<bool> condition;
        public T state;
        public System.Action onTransition;
    }

    public T StateName { get; private set; }
    public T NextState { get; protected set; }
    private List<TransitionState> Transitions { get; set; }

    public AState(T stateName)
    {
        StateName = stateName;
        NextState = stateName;
        Transitions = new List<TransitionState>();
    }

    public void Init()
    {
        OnStateInit();
    }

    public void Enter()
    {
        NextState = StateName;
        OnStateEnter();
    }

    public void Update()
    {
        OnStateUpdate();
        if (!HasNextState())
        {
            T stateName;
            if (CheckTransitions(out stateName))
            {
                NextState = stateName;
            }
        }
    }

    public void Exit()
    {
        OnStateExit();
    }

    public AState<T> AddTransition(T nextState, System.Func<bool> condition, System.Action onTransition = null)
    {
        Transitions.Add(new TransitionState { condition = condition, state = nextState, onTransition = onTransition });
        return this;
    }

    private bool CheckTransitions(out T nextState)
    {
        foreach (TransitionState transitionState in Transitions)
        {
            if (transitionState.condition.Invoke())
            {
                if (transitionState.onTransition != null)
                {
                    transitionState.onTransition();
                }
                nextState = transitionState.state;
                return true;
            }
        }
        nextState = StateName;
        return false;
    }

    public bool HasNextState()
    {
        return !NextState.Equals(StateName);
    }

    protected virtual void OnStateInit() { }

    protected virtual void OnStateEnter() { }

    protected virtual void OnStateUpdate() { }

    protected virtual void OnStateExit() { }
}
