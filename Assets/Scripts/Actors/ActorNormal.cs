using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtility;
using State = StateMachine<ActorNormal>.State;

public class ActorNormal : ActorBase
{
	private StateMachine<ActorNormal> stateMachine;
	private readonly int IsAttackHash = Animator.StringToHash("IsAttack");
	private readonly int IsBiteHash = Animator.StringToHash("IsBite");
	private readonly int IsDamageHash = Animator.StringToHash("IsDamage");


	//------------------------------------------
	// Unityタイムライン
	//------------------------------------------
	protected override void SetStateMachineOnBegin()
	{
		stateMachine = new StateMachine<ActorNormal>(this);
		stateMachine.AddTransition<StateWonder, StateChase>((int)Event.DoChase);
		stateMachine.AddTransition<StateChase, StateAttack>((int)Event.DoAttack);
		stateMachine.AddTransition<StateAttack, StateWonder>((int)Event.DoWonder);
		stateMachine.AddTransition<StateBite, StateWonder>((int)Event.DoWonder);
		stateMachine.AddTransition<StateDamage, StateWonder>((int)Event.DoWonder);
		stateMachine.AddTransition<StateAttack, StateBite>((int)Event.DoBite);
		stateMachine.AddTransition<StateAttack, StateDamage>((int)Event.DoDamage);
		stateMachine.AddAnyTransition<StateDeath>(((int)Event.DoDeath));
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
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (hp <= 0) return;
		if (collision.gameObject != this.gameObject)
		{
			var target = collision.gameObject.GetComponent<ActorBase>();
			if (target != null && IsBiteingEnable() && target.Friendly != Friendly)
			{
				target.ApplyDamage(this, param.Power);
				stateMachine.Dispatch(((int)Event.DoBite));
			}
		}
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
			VisualizeDamage();
			stateMachine.Dispatch(((int)Event.DoDamage));
		}
		else stateMachine.Dispatch(((int)Event.DoDeath));
	}


	//------------------------------------------
	// ステートマシン
	//------------------------------------------
	protected enum Event
	{
		DoWonder, DoChase, DoAttack, DoBite, DoDamage, DoDeath,
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
		protected override void OnUpdate()
		{
			if (owner.GetTargetDistance() <= owner.param.AttackDistance)
			{
				stateMachine.Dispatch(((int)Event.DoAttack));
			}
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = owner.GetForceVelocity(owner.param.Speed);
		}
	}
	private class StateAttack : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.animator.SetBool(owner.IsAttackHash, true);
		}
		protected override void OnExit(State nextState)
		{
			owner.animator.SetBool(owner.IsAttackHash, false);
		}
	}
	private class StateBite : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.animator.SetBool(owner.IsBiteHash, true);
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.AddForce(owner.param.BiteDestinate);
		}
		protected override void OnExit(State nextState)
		{
			owner.animator.SetBool(owner.IsBiteHash, false);
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
			owner.bitingSource = null;
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
				owner.VisualizeDeath();
			}
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = Vector2.down;
		}
	}
}
