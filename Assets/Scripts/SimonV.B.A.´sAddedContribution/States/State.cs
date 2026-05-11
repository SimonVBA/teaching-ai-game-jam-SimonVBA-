using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IMonoBehaviourComponent
{
    Transform transform {get;}
    GameObject gameObject {get;}

    public Coroutine StartCoroutine(string methodName);
    public void StopCoroutine(Coroutine routine);
}

public interface IStateControl<T>
{
    //protected List<State<T>> states{get; set;}
    Dictionary<Type, State<T>> states{get;}
    State<T> currentState{get;}
    //void SwitchState(State<T> state);
    public void SwitchState<TState>() where TState: State<T>, new();
}
public interface IMoveControl
{
    float maxSpeed {get; set;} // in m/s
    Rigidbody2D playerRigidbody{get; set;}
    Vector2 _getTrackedPosition();
    //event Action<Vector2> MoveEvent;
}

[Serializable]
public abstract partial class State<T>
{
    protected T context;

    public State(T currentContext)
    {
        context = currentContext;
    }

    public abstract void EnterState();
    //public abstract void UpdateState(); //The state can control this itself with a coroutine
    public abstract  void ExitState();
}

public class MoveState<T>: State<T> where T: MonoBehaviour, IMoveControl, IStateControl<T>
{
    protected Coroutine moveRoutine;
    public MoveState(T currentContext): base(currentContext){}
    public override void EnterState()
    {
        moveRoutine = context.StartCoroutine("Move");
    }

    protected virtual IEnumerator Move()
    {
        while (true)
        {
            //context.MoveEvent?.Invoke(_getForce(_getTrackedPosition()).normalized);

            context.playerRigidbody.AddForce(TargetDistanceVector().normalized * context.maxSpeed);

            yield return new WaitForFixedUpdate();
        }
    }
    protected Vector2 TargetDistanceVector()
    {
        Vector2 difference = context._getTrackedPosition() - new Vector2(context.transform.position.x,context.transform.position.y);
        return difference;
    }
    
    public override void ExitState()
    {
        context.StopCoroutine(moveRoutine);
        //context.MoveEvent?.Invoke(Vector2.zero);
    }
}
public class WobbleMoveState<T> : MoveState<T> where T: MonoBehaviour, IMoveControl, IStateControl<T>
{
    [SerializeField]float frequency;
    [SerializeField]float amplitude;
    public WobbleMoveState(T currentContext): base(currentContext){}

    protected override IEnumerator Move()
    {
        while (true)
        {
            //context.MoveEvent?.Invoke(_getForce(_getTrackedPosition()).normalized);
            float wobble = Mathf.Sin(Time.time * frequency + amplitude);
            Vector2 Direction = TargetDistanceVector().normalized;
            Vector2 finalDirection = (Direction + new Vector2(Direction.y,Direction.x) * wobble).normalized;
            context.playerRigidbody.AddForce(finalDirection * context.maxSpeed);

            yield return new WaitForFixedUpdate();
        }
    }

}