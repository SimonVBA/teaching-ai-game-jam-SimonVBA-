using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IStateControl<T>
{
    //protected List<State<T>> states{get; set;}
    Dictionary<Type, State<T>> states{get;}
    State<T> currentState{get;}
    //void SwitchState(State<T> state);
    public void SwitchState<TState>() where TState: State<T>;
}

public interface IMoveControl
{
    float maxSpeed {get; set;} // in m/s
    Rigidbody2D playerRigidbody{get; set;}
    Vector2 getTrackedPosition();
    SpriteRenderer spriteRenderer{get; set;}
}

[Serializable]
public abstract partial class State<T>
{
    [Serialize] protected T context;
    public State(T currentContext)
    {
        context = currentContext;
    }
    public abstract void EnterState();
    public abstract void ExitState();
}

public abstract class StateMachine<T> : MonoBehaviour, IStateControl<T>
{
    public Dictionary<Type, State<T>> states{get;} =new Dictionary<Type, State<T>>();
    public State<T> currentState{get; protected set;} 
    public void SwitchState<TState>() where TState: State<T>
    {
        currentState?.ExitState();
        Type stateType = typeof(TState);
        if (!states.ContainsKey(stateType)) states.Add(stateType,(TState)Activator.CreateInstance(stateType, new object[]{this}));
        currentState = states[stateType];
        currentState.EnterState();
    }
}