using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtility;
using State = StateMachine<ActorDecoy>.State;

public class ActorDecoy : ActorBase
{
	private StateMachine<ActorDecoy> stateMachine;



	//------------------------------------------
	// Unityタイムライン
	//------------------------------------------
	protected override void InitStateMachine()
	{
		stateMachine = new StateMachine<ActorDecoy>(this);
		stateMachine.AddTransition<StateDecoy, StateWonder>((int)Event.DoWonder);
		stateMachine.AddTransition<StateWonder, StateChase>((int)Event.DoChase);
		stateMachine.AddAnyTransition<StateDecoy>(((int)Event.DoDecoy));
		stateMachine.AddAnyTransition<StateDeath>(((int)Event.DoDeath));
		stateMachine.Start<StateWonder>();
	}



	//------------------------------------------
	// 継承関数
	//------------------------------------------
	public override void OnAnimationExit()
	{
		stateMachine.Dispatch(((int)Event.DoWonder));
	}
	public override void ApplyDamage(ActorBase biter, float damage)
	{
		base.ApplyDamage(biter, damage);
		if (hp > 0)
		{
			stateMachine.Dispatch(((int)Event.DoDecoy));
		}
		else stateMachine.Dispatch(((int)Event.DoDeath));
	}

	protected override void OnVirtualUpdate()
	{
		stateMachine.Update();
	}

	protected override void OnVirtualFixedUpdate()
	{
		stateMachine.FixedUpdate();
	}


	//------------------------------------------
	// ステートマシン
	//------------------------------------------
	protected enum Event
	{
		DoWonder, DoChase, DoDecoy, DoDeath,
	}
	private class StateWonder : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.rigid.velocity = Vector2.zero;
		}
		protected override void OnUpdate()
		{
			var target = owner.battleManager?.GetTrackableActor(owner);
			if (target != null)
			{
				owner.OnRefleshTrackActor(target);
				stateMachine.Dispatch(((int)Event.DoChase));
			}
			else owner.FetchCurrentDirection();
		}
	}
	private class StateChase : State
	{
		float additiveSpeed = 2f;
		protected override void OnUpdate()
		{
			if (owner.GetTargetDistance() <= owner.param.ActionDistance)
			{
				stateMachine.Dispatch(((int)Event.DoDecoy));
			}
			else owner.FetchCurrentDirection();
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = owner.GetForceVelocity(owner.param.Speed * additiveSpeed);
		}
	}
	private class StateDecoy : State
	{
		float time = 0f;
		float decoyTime = 0.3f;
		float additiveSpeed = 5f;
		protected override void OnUpdate()
		{
			time += Time.deltaTime;
			if (time >= decoyTime)
			{
				time = 0;
				stateMachine.Dispatch(((int)Event.DoWonder));
			}
			owner.FetchCurrentDirection(true);
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = owner.GetForceVelocity(owner.param.Speed * additiveSpeed) * -1;
		}
	}
	private class StateDeath : State
	{
		bool isOrdered = false;
		protected override void OnEnter(State prevState)
		{
			if (!isOrdered)
			{
				isOrdered = true;
				owner.OnDeathEffectBegin();
			}
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = Vector2.down;
		}
	}
}
