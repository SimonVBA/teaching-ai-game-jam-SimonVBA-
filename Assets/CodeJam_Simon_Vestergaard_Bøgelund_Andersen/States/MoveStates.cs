using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class MoveState<T>: State<T> where T: MonoBehaviour, IMoveControl//, IStateControl<T>
{
    protected Coroutine moveRoutine;
    public MoveState(T currentContext): base(currentContext){}
    public override void EnterState()
    {
        moveRoutine = context.StartCoroutine(Move());
    }

    protected virtual IEnumerator Move()
    {
        while (true)
        {
            Vector2 direction = TargetDistanceVector().normalized;
            context.playerRigidbody.AddForce(direction* context.maxSpeed);
            context.spriteRenderer.flipX = direction.x > 0;
            yield return new WaitForFixedUpdate();
        }
    }
    
    public override void ExitState()
    {
        context.StopCoroutine(moveRoutine);
    }

    protected Vector2 TargetDistanceVector()
    {
        Vector2 difference = context.getTrackedPosition() - new Vector2(context.transform.position.x,context.transform.position.y);
        return difference;
    }
}
[Serializable]
public class WobbleMoveState<T> : MoveState<T> where T: MonoBehaviour, IMoveControl//, IStateControl<T>
{
    [SerializeField] [Serialize]float frequency;
    [SerializeField] [Serialize]float amplitude;
    public WobbleMoveState(T currentContext): base(currentContext)
    {
        frequency = UnityEngine.Random.Range(0.1f, 10f);
        amplitude = UnityEngine.Random.Range(0f, 5f);
    }

    protected override IEnumerator Move()
    {
        while (true)
        {
            float wobble = Mathf.Sin(Time.time * frequency);
            Vector2 direction = TargetDistanceVector().normalized;
            Vector2 offset = new Vector2(-direction.y,direction.x) * wobble * amplitude;
            Vector2 finalDirection = (direction + offset ).normalized;
            context.playerRigidbody.AddForce(finalDirection * context.maxSpeed);
            Debug.DrawLine(context.transform.position,context.transform.position + new Vector3(direction.x,direction.y,0));
            Debug.DrawLine(context.transform.position,context.transform.position + new Vector3(offset.x,offset.y,0),Color.blue);
            context.spriteRenderer.flipX = finalDirection.x > 0;
            yield return new WaitForFixedUpdate();
        }
    }

}
