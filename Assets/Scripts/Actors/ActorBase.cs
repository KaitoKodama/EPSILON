using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CommonUtility;

public abstract class ActorBase : MonoBehaviour
{
	[SerializeField] GameObject bloodEffect = default;

	protected ActorParam param;
	protected BattleManager battleManager;
	protected AudioSource audioSource;
	protected ActorBase bitingSource;
	protected Animator animator;
	protected Rigidbody2D rigid;
	protected float hp, maxHp;

	private ActorBase trackingTarget;
	private Coroutine anyCoroutine;
	private Coroutine soundCoroutine;
	private Coroutine particleCoroutine;
	private SpriteRenderer render;
	private Friendly friendly;
	private Vector2 force;
	private bool isInitActor = false;


	//------------------------------------------
	// 初期化
	//------------------------------------------
	public void InitActor(ActorParam param, Friendly friendly)
	{
		battleManager = FindObjectOfType<BattleManager>();
		audioSource = GetComponent<AudioSource>();
		render = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody2D>();

		render.sprite = param.ActorSprite;
		this.animator.runtimeAnimatorController = param.Controller;
		this.friendly = friendly;
		this.param = param;
		this.hp = param.Health;
		this.maxHp = hp;

		InitStateMachine();
		isInitActor = true;
	}
	private void Update()
	{
		if (isInitActor)
		{
			OnVirtualUpdate();
		}
	}
	private void FixedUpdate()
	{
		if (isInitActor)
		{
			OnVirtualFixedUpdate();
		}
	}


	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
	public ActorParam Param { get => param; }
	public Friendly Friendly { get => friendly; }
	public float ClampHP { get { return hp / maxHp; } }
	public float HP { get => hp; set => hp = value; }
	public float MaxHP { get => hp; }

	public abstract void OnAnimationExit();
	public virtual void ApplyDamage(ActorBase biter, float damage)
	{
		if (hp > 0)
		{
			render.DOColor(new Color(1, ClampHP, ClampHP, 1), 1f);
			bitingSource = biter;
			hp -= damage;
			OnEmitDamageParticle();
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
	protected ActorBase TrackingActorInfo { get => trackingTarget; }
	protected abstract void InitStateMachine();
	protected abstract void OnVirtualUpdate();
	protected abstract void OnVirtualFixedUpdate();
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
	protected void FetchCurrentDirection(bool isInverse = false)
	{
		int direction = System.Math.Sign(force.x);
		if (!isInverse)
		{
			if (direction == -1) transform.rotation = Quaternion.Euler(0, 0, 0);
			else if (direction == 1) transform.rotation = Quaternion.Euler(0, 180, 0);
		}
		else
		{
			if (direction == -1) transform.rotation = Quaternion.Euler(0, 180, 0);
			else if (direction == 1) transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}
	protected void OnEmitAnyParticle(GameObject obj, float gap)
	{
		if (anyCoroutine == null)
		{
			anyCoroutine = StartCoroutine(EmitAnyParticle(obj, gap));
		}
	}
	protected void OnPlaySoundGaply(AudioClip clip, float gap)
	{
		if(soundCoroutine == null)
		{
			soundCoroutine = StartCoroutine(PlaySoundGaply(clip, gap));
		}
	}
	protected void OnDeathEffectBegin()
	{
		enabled = false;
		OnDeathTheActorNotifyerHandler?.Invoke(friendly);
		render.DOFade(0f, 3f).OnComplete(() =>
		{
			gameObject.SetActive(false);
		});
	}

	//------------------------------------------
	// 内部共有関数
	//------------------------------------------
	private void OnEmitDamageParticle()
	{
		if (particleCoroutine == null)
		{
			particleCoroutine = StartCoroutine(EmitDamageParticle());
		}
	}
	private IEnumerator EmitDamageParticle()
	{
		bloodEffect.SetActive(true);
		yield return new WaitForSeconds(1.5f);

		bloodEffect.SetActive(false);
		particleCoroutine = null;
	}
	private IEnumerator PlaySoundGaply(AudioClip clip, float gap)
	{
		audioSource.PlayOneShot(clip);
		yield return new WaitForSeconds(gap);
		soundCoroutine = null;
	}
	private IEnumerator EmitAnyParticle(GameObject obj, float gap)
	{
		obj.SetActive(true);
		yield return new WaitForSeconds(gap);

		obj.SetActive(false);
		anyCoroutine = null;
	}
}
