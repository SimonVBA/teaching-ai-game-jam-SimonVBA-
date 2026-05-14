using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyController : StateMachine<EnemyController>, IMoveControl
{
    [SerializeField] private float _maxSpeed = 25f;
    public float maxSpeed {
        get => _maxSpeed;
        set => _maxSpeed = value;
    }//Property 
    public Rigidbody2D playerRigidbody{get; set;}
    public SpriteRenderer spriteRenderer{get; set;}
    
    //event Action<Vector2> MoveEvent;
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        var moveState = new MoveState<EnemyController>(this);
        states.Add(moveState.GetType(),moveState);
        var wobblemoveState = new WobbleMoveState<EnemyController>(this);
        states.Add(wobblemoveState.GetType(),wobblemoveState);
        
        System.Random random = new System.Random();
        //currentState = states.ElementAt(random.Next(states.Count)).Value;
        switch (random.Next(states.Count))
        {
            case 0: SwitchState<WobbleMoveState<EnemyController>>();
                break;
            case 1: SwitchState<MoveState<EnemyController>>();
                break;
            default: Debug.LogError("random out of Range");
                break;
        }
        
        
        currentState = currentState;
    }
    public Vector2 getTrackedPosition(){ return AiCenter.Instance.transform.position;}
}
