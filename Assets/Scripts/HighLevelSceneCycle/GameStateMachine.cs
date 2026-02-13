using System;
using System.Collections.Generic;
using Zenject;

public class GameStateMachine
{
    private readonly DiContainer _container;
    private readonly Dictionary<Type, IGameState> _states = new();
    private IGameState _activeState;

    public GameStateMachine(DiContainer container)
    {
        _container = container;
    }

    // add returning of curr state;
    public T Enter<T>() where T : IGameState
    {
        _activeState?.Exit();

        var stateType = typeof(T);

        if (!_states.TryGetValue(stateType, out var newState))
        {
            newState = _container.Instantiate<T>();    
            _states.Add(stateType, newState);
        }

        _activeState = newState;
        _activeState.Enter();

        return (T) _activeState;
    }
}