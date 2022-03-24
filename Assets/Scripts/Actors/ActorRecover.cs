using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtility;
using State = StateMachine<ActorRecover>.State;

public class ActorRecover : ActorBase
{
	private StateMachine<ActorRecover> stateMachine;
	private readonly int IsDamageHash = Animator.StringToHash("IsDamage");

	//------------------------------------------
	// Unityタイムライン
	//------------------------------------------
	protected override void InitStateMachine()
	{
		stateMachine = new StateMachine<ActorRecover>(this);
		stateMachine.AddTransition<StateWonder, StateChase>(((int)Event.DoChase));
		stateMachine.AddTransition<StateRecover, StateWonder>(((int)Event.DoWonder));
		stateMachine.AddTransition<StateDamage, StateWonder>(((int)Event.DoWonder));
		stateMachine.AddTransition<StateChase, StateRecover>(((int)Event.DoRecover));
		stateMachine.AddTransition<StateRecover, StateDamage>(((int)Event.DoDamage));
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
			stateMachine.Dispatch(((int)Event.DoDamage));
		}
		else stateMachine.Dispatch(((int)Event.DoDeath));
	}

	protected override void OnVirtualUpdate()
	{
		stateMachine.Update();
		if (hp > 0) FetchCurrentDirection();
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
		DoWonder, DoChase, DoRecover, DoDamage, DoDeath,
	}
	private class StateWonder : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.rigid.velocity = Vector2.zero;
		}
		protected override void OnUpdate()
		{
			var target = GetNearestFriendlyActor();
			if (target != null)
			{
				owner.OnRefleshTrackActor(target);
				stateMachine.Dispatch(((int)Event.DoChase));
			}
		}
		private ActorBase GetNearestFriendlyActor()
		{
			ActorBase targetActor = null;
			float distance = float.MaxValue;
			var actorList = owner.battleManager?.GetActorList();
			foreach(var actor in actorList)
			{
				var recover = actor.GetComponent<ActorRecover>();
				if (actor != null && actor.enabled && actor.Friendly == owner.Friendly && recover == null) 
				{
					float dist = Vector2.Distance(owner.transform.position, actor.transform.position);
					if (dist <= distance)
					{
						distance = dist;
						targetActor = actor;
					}
				}
			}
			return targetActor;
		}
	}
	private class StateChase : State
	{
		float additiveSpeed = 2f;
		protected override void OnUpdate()
		{
			if (owner.GetTargetDistance() <= owner.param.ActionDistance)
			{
				stateMachine.Dispatch(((int)Event.DoRecover));
			}
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = owner.GetForceVelocity(owner.param.Speed * additiveSpeed);
		}
	}
	private class StateRecover : State
	{
		float time = 0;
		float coolTime = 0.3f;
		float recoverValue = 5f;

		protected override void OnEnter(State prevState)
		{
			owner.TrackingActorInfo.HP += recoverValue;
		}
		protected override void OnUpdate()
		{
			time += Time.deltaTime;
			if (time >= coolTime)
			{
				time = 0;
				stateMachine.Dispatch(((int)Event.DoWonder));
			}
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = owner.GetForceVelocity(owner.param.Speed);
		}
	}
	private class StateDamage : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.animator.SetBool(owner.IsDamageHash, true);
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = owner.GetForceVelocity(owner.param.BitingForce);
		}
		protected override void OnExit(State nextState)
		{
			owner.animator.SetBool(owner.IsDamageHash, false);
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
