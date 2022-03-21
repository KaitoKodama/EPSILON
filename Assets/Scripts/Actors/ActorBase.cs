using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CommonUtility;

public abstract class ActorBase : MonoBehaviour
{
	[SerializeField]
	protected ActorParam param = default;
	[SerializeField]
	private ParticleSystem bloodEffect = default;

	protected BattleManager battleManager;
	protected ActorBase bitingSource;
	protected Animator animator;
	protected Rigidbody2D rigid;
	[SerializeField]protected float hp, maxHp;

	private ActorBase trackingTarget;
	private SpriteRenderer render;
	private Friendly friendly;
	private Vector2 force;


	//------------------------------------------
	// 初期化
	//------------------------------------------

	private void Start()
	{
		hp = param.Health;
		maxHp = hp;

		battleManager = GameObject.FindWithTag("BattleManager").GetComponent<BattleManager>();
		render = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody2D>();

		SetStateMachineOnBegin();
	}
	protected abstract void SetStateMachineOnBegin();


	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
	public ActorParam Param { get => param; }
	public Friendly Friendly { get => friendly; set => friendly = value; }

	public abstract void OnAnimationExit();
	public virtual void ApplyDamage(ActorBase biter, float damage)
	{
		if (hp > 0)
		{
			bloodEffect.Play();
			bitingSource = biter;
			hp -= damage;
		}
	}

	//------------------------------------------
	// デリゲート通知
	//------------------------------------------
	public delegate void OnDeathTheActorNotifyer(Friendly friendly);
	public OnDeathTheActorNotifyer OnDeathTheActorNotifyerHandler;


	//------------------------------------------
	// 継承先共有関数
	//------------------------------------------
	protected Vector2 GetForceVelocity(float multiply)
	{
		var targetform = trackingTarget.transform.position;
		var selfform = transform.position;
		var force = (targetform - selfform).normalized;
		return force * multiply * Time.deltaTime;
	}
	protected float GetTargetDistance()
	{
		float distance = Vector2.Distance(trackingTarget.transform.position, transform.position);
		return distance;
	}
	protected bool IsBiteingEnable()
	{
		if (Utility.Probability(param.BiteRate))
		{
			if (bitingSource == null) return true;
			else bitingSource = null;
		}
		return false;
	}
	protected void OnRefleshTrackActor(ActorBase target)
	{
		trackingTarget = target;

		force = (target.transform.position - transform.position).normalized;
		int direction = System.Math.Sign(force.x);
		if (direction == -1) transform.rotation = Quaternion.Euler(0, 0, 0);
		else if (direction == 1) transform.rotation = Quaternion.Euler(0, 180, 0);

		rigid.velocity = Vector2.zero;
	}
	protected void FetchCurrentDirection()
	{
		int direction = System.Math.Sign(force.x);
		if (direction == -1) transform.rotation = Quaternion.Euler(0, 0, 0);
		else if (direction == 1) transform.rotation = Quaternion.Euler(0, 180, 0);
		rigid.velocity = Vector2.zero;
	}
	protected void VisualizeDamage()
	{
		float one = hp / maxHp;
		render.DOColor(new Color(1, one, one, 1), 0.3f);
	}
	protected void VisualizeDeath()
	{
		enabled = false;
		OnDeathTheActorNotifyerHandler?.Invoke(friendly);
		render.DOFade(0f, 3f).OnComplete(() =>
		{
			gameObject.SetActive(false);
		});
	}
}
