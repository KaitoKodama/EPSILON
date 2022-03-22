using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "ScriptableObject/Param")]
public class ActorParam : ScriptableObject
{
	[Header("プレハブ")]
	[SerializeField] GameObject actorPrefab = default;

	[Header("アニメーションコントローラ")]
	[SerializeField] RuntimeAnimatorController controller = default;

	[Header("スプライト")]
	[SerializeField] Sprite actorSprite = default;

	[Header("使用コスト")]
	[SerializeField] float cost = 10;

	[Header("名前")]
	[SerializeField] string actorName = default;

	[Header("説明")]
	[SerializeField][TextArea(5,10)]
	private string actorExplain = default;

	[Header("噛みつき移動先")]
	[SerializeField] Vector2 biteDestinate = Vector2.up;

	[Header("噛みつき成功率(％)")]
	[SerializeField] float biteRate = 50;

	[Header("体力")]
	[SerializeField] float health = 100;

	[Header("攻撃力")]
	[SerializeField] float power = 10;

	[Header("狙われ率(％)")]
	[SerializeField] float trackRate = 100;

	private float bitingForce = 10;
	private float speed = 50;
	private float attackDistance = 3;


	public Vector2 BiteDestinate { get => biteDestinate; }
	public float BiteRate { get => biteRate; }
	public float Speed { get => speed; }
	public float Power { get => power; }
	public float Health { get => health;  }
	public float BitingForce { get => bitingForce; }
	public float AttackDistance { get => attackDistance;  }
	public float TrackRate { get => trackRate; }

	public GameObject ActorPrefab { get => actorPrefab; }
	public Sprite ActorSprite { get => actorSprite; }
	public RuntimeAnimatorController Controller { get => controller; }
	public string ActorName { get => actorName; }
	public string ActorExplain { get => actorExplain; }
	public float Cost { get => cost;  }
}
