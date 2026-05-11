using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour, IMoveControl, IStateControl<EnemyController>
{
    //[SerializeField]protected List<State<EnemyController>> states = new List<State<EnemyController>>();
    //protected Dictionary<Type, State<EnemyController>> states = new Dictionary<Type, State<EnemyController>>();
    public Dictionary<Type, State<EnemyController>> states{get;} =new Dictionary<Type, State<EnemyController>>();
    public State<EnemyController> currentState{get;set;} 
    private AiCenter _aiCore;
    public float maxSpeed {get; set;} // in m/s
    public Rigidbody2D playerRigidbody{get; set;}
    
    //event Action<Vector2> MoveEvent;
    private void Awake()
    {
        var moveState = new MoveState<EnemyController>(this);
        states.Add(moveState.GetType(),moveState);
        var wobblemoveState = new WobbleMoveState<EnemyController>(this);
        states.Add(moveState.GetType(),wobblemoveState);
        
        System.Random random = new System.Random();
        currentState = states.ElementAt(random.Next(states.Count)).Value;
    }
    public Vector2 _getTrackedPosition(){ return _aiCore.transform.position;}
    public void SwitchState<TState>() where TState: State<EnemyController>, new()
    {
        Type stateType = typeof(TState);
        if (!states.ContainsKey(stateType)) states.Add(stateType,new TState());
        currentState = states[stateType];
    }
}
