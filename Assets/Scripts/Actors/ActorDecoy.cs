using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtility;
using State = StateMachine<ActorDecoy>.State;

public class ActorDecoy : ActorBase
{
	[SerializeField] AudioClip attackSound;

	private StateMachine<ActorDecoy> stateMachine;
	private readonly int IsAttackHash = Animator.StringToHash("IsAttack");
	private readonly int IsBiteHash = Animator.StringToHash("IsBite");
	private readonly int IsDamageHash = Animator.StringToHash("IsDamage");


	//------------------------------------------
	// Unityタイムライン
	//------------------------------------------
	protected override void InitStateMachine()
	{
		stateMachine = new StateMachine<ActorDecoy>(this);
		stateMachine.AddTransition<StateDecoy, StateWonder>((int)Event.DoWonder);
		stateMachine.AddTransition<StateAttack, StateWonder>((int)Event.DoWonder);
		stateMachine.AddTransition<StateBite, StateWonder>((int)Event.DoWonder);
		stateMachine.AddTransition<StateWonder, StateChase>((int)Event.DoChase);
		stateMachine.AddTransition<StateChase, StateDecoy>((int)Event.DoDecoy);
		stateMachine.AddTransition<StateChase, StateAttack>((int)Event.DoAttack);
		stateMachine.AddTransition<StateAttack, StateBite>((int)Event.DoBite);
		stateMachine.AddTransition<StateAttack, StateDamage>((int)Event.DoDamage);
		stateMachine.AddTransition<StateDecoy, StateDamage>((int)Event.DoDamage);
		stateMachine.AddAnyTransition<StateDeath>(((int)Event.DoDeath));
		stateMachine.Start<StateWonder>();
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (hp <= 0) return;
		if (collision.gameObject != this.gameObject)
		{
			var target = collision.gameObject.GetComponent<ActorBase>();
			bool con01 = target != null && target.Friendly != Friendly;
			bool con02 = IsBiteingEnable();

			if (con01 && con02)
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
		DoWonder, DoChase, DoDecoy, DoAttack, DoBite, DoDamage, DoDeath,
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
				if (Utility.Probability(owner.ClampHP * 100)) stateMachine.Dispatch(((int)Event.DoDecoy));
				else stateMachine.Dispatch(((int)Event.DoAttack));
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
	private class StateAttack : State
	{
		float soundGap = 5f;
		protected override void OnEnter(State prevState)
		{
			owner.OnPlaySoundGaply(owner.attackSound, soundGap);
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
				owner.OnDeathEffectBegin();
			}
		}
		protected override void OnFixedUpdate()
		{
			owner.rigid.velocity = Vector2.down;
		}
	}
}
