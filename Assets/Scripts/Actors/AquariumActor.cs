using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State = StateMachine<AquariumActor>.State;

public class AquariumActor : MonoBehaviour
{
	[SerializeField] Vector3 rangeFrom;
	[SerializeField] Vector3 rangeTo;
	[SerializeField] float speed = 60f;

	private StateMachine<AquariumActor> stateMachine;
	private Rigidbody2D rigid;
	private Vector3 destination;


	private void Start()
	{
		rigid = GetComponent<Rigidbody2D>();

		stateMachine = new StateMachine<AquariumActor>(this);
		stateMachine.AddTransition<StatePatroll, StateWonder>((int)Event.DoWonder);
		stateMachine.AddTransition<StateWonder, StatePatroll>((int)Event.DoPatroll);
		stateMachine.Start<StateWonder>();
	}
	private void Update()
	{
		stateMachine.Update();
	}
	private void FixedUpdate()
	{
		stateMachine.FixedUpdate();
	}

	private Vector3 RandomInRange()
	{
		float x = Random.Range(rangeFrom.x, rangeTo.x);
		float y = Random.Range(rangeFrom.y, rangeTo.y);
		return new Vector3(x, y, 0);
	}
	private void SetDirectionRotate()
	{
		int direction = System.Math.Sign(destination.x);
		if (direction == -1) transform.rotation = Quaternion.Euler(0, 0, 0);
		else if (direction == 1) transform.rotation = Quaternion.Euler(0, 180, 0);
	}



	//------------------------------------------
	// ステートマシン
	//------------------------------------------
	protected enum Event
	{
		DoWonder, DoPatroll
	}
	private class StateWonder : State
	{
		float time = 0;
		float elapseTime = 0f;
		protected override void OnEnter(State prevState)
		{
			owner.destination = owner.RandomInRange();
			elapseTime = Random.Range(1, 5);
		}
		protected override void OnUpdate()
		{
			time += Time.deltaTime;
			if (time >= elapseTime)
			{
				time = 0;
				stateMachine.Dispatch(((int)Event.DoPatroll));
			}
		}
	}
	private class StatePatroll : State
	{
		float breakDistance = 1f;
		protected override void OnUpdate()
		{
			owner.SetDirectionRotate();
			float distance = Vector2.Distance(owner.transform.position, owner.destination);
			if (distance <= breakDistance)
			{
				stateMachine.Dispatch(((int)Event.DoWonder));
			}
		}
		protected override void OnFixedUpdate()
		{
			var targetform = owner.destination;
			var selfform = owner.transform.position;
			var force = (targetform - selfform).normalized;
			owner.rigid.velocity = force * owner.speed * Time.deltaTime;
		}
		protected override void OnExit(State nextState)
		{
			owner.rigid.velocity = Vector2.zero;
		}
	}
}
