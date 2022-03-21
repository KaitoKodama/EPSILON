using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "ScriptableObject/Param")]
public class ActorParam : ScriptableObject
{
	[Header("スプライト")]
	[Header("--------- ユニット展開情報 --------")]
	[SerializeField]
	private Sprite actorSprite = default;

	[Header("使用コスト")]
	[SerializeField]
	private float cost = 10;

	[Header("名前")]
	[SerializeField]
	private string actorName = default;

	[Header("説明")]
	[SerializeField][TextArea(5,10)]
	private string actorExplain = default;


	[Header("噛みつき時の移動先")]
	[Header("----- 噛みつき -----")]
	[Header("--------- ゲームパラメータ --------")]
	[SerializeField]
	private Vector2 biteDestinate = Vector2.up;

	[Header("噛みつき成功率(％)")]
	[SerializeField, Range(1f, 99f)]
	private float biteRate = 50;

	[Header("噛みつき力")]
	[SerializeField]
	private float bitingForce = 10;

	[Header("移動速度")]
	[Header("----- 基本情報 -----")]
	[SerializeField]
	private float speed = 50;

	[Header("体力")]
	[SerializeField]
	private float health = 100;

	[Header("攻撃力")]
	[SerializeField]
	private float power = 10;

	[Header("攻撃距離")]
	[SerializeField]
	private float attackDistance = 2;

	[Header("狙われ率(％)")]
	[SerializeField, Range(1f, 100f)]
	private float trackRate = 100;


	//ゲームパラメータ
	public Vector2 BiteDestinate { get => biteDestinate; }
	public float BiteRate { get => biteRate; }
	public float Speed { get => speed; }
	public float Power { get => power; }
	public float Health { get => health;  }
	public float BitingForce { get => bitingForce; }
	public float AttackDistance { get => attackDistance;  }
	public float TrackRate { get => trackRate; }

	//ユニット展開
	public Sprite ActorSprite { get => actorSprite; }
	public string ActorName { get => actorName; }
	public string ActorExplain { get => actorExplain; }
	public float Cost { get => cost;  }
}
