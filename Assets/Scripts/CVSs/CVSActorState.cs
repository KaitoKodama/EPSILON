using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CommonUtility;

public class CVSActorState : MonoBehaviour
{
	[SerializeField] Image enemyFill;
	[SerializeField] Image playerFill;
	[SerializeField] Button stateButton;
	[SerializeField] GameObject conditionParent;
	[SerializeField] GameObject unitWindow;
	[SerializeField] GameObject conditionPrefab;

	private List<StatuePool> statuePoolList;
	private BattleManager battleManager;
	private float playerHPTotal, playerHPMaxTotal;
	private float enemyHPTotal, enemyHPMaxTotal;
	private float circleOffset = 50f;
	private bool isVisualize = false;

	private void Start()
	{
		battleManager = FindObjectOfType<BattleManager>();
		battleManager.OnBattleBeginNotifyerHandler = OnBattleBeginReciever;
		stateButton.onClick.AddListener(OnStateButton);

		unitWindow.SetActive(false);
	}
	private void Update()
	{
		if (statuePoolList != null && isVisualize)
		{
			foreach (var pool in statuePoolList)
			{
				pool.condition.transform.position = pool.actor.transform.position;
				if (pool.actor.enabled)
				{
					var actorScreenpos = Camera.main.WorldToScreenPoint(pool.actor.transform.position);
					actorScreenpos.x += circleOffset;
					actorScreenpos.y += circleOffset;
					actorScreenpos.z = -5f;
					pool.condition.transform.position = actorScreenpos;
					pool.condition.SetCircleFill(pool.actor.ClampHP);
				}
				else
				{
					if (pool.condition.gameObject.activeSelf)
					{
						pool.condition.gameObject.SetActive(false);
					}
				}
			}
		}
	}


	//------------------------------------------
	// デリゲート通知
	//------------------------------------------
	private void OnBattleBeginReciever(List<ActorBase> actorList)
	{
		statuePoolList = new List<StatuePool>();
		foreach (var actor in actorList)
		{
			actor.OnDamageNotifyerHandler = OnActorDamageReciever;
			SetActorTotalHP(actor.Friendly, actor.MaxHP);

			var circle = Instantiate(conditionPrefab, conditionParent.transform);
			var condition = circle.GetComponent<CVSCondition>();
			circle.SetActive(false);
			statuePoolList.Add(new StatuePool(actor, condition, GetColorDueFriendly(actor.Friendly)));
		}
	}
	private void OnActorDamageReciever(Friendly friendly, float damage)
	{
		if (friendly == Friendly.PlayerFriendly)
		{
			playerHPTotal -= damage;
			playerFill.DOFillAmount(playerHPTotal / playerHPMaxTotal, 0.3f);
		}
		if (friendly == Friendly.EnemyFriendly)
		{
			enemyHPTotal -= damage;
			enemyFill.DOFillAmount(enemyHPTotal / enemyHPMaxTotal, 0.3f);
		}
	}


	//------------------------------------------
	// イベントリスナー
	//------------------------------------------
	private void OnStateButton()
	{
		if(statuePoolList != null)
		{
			isVisualize = Utility.FilpFlop(isVisualize);
			unitWindow.SetActive(Utility.FilpFlop(unitWindow.activeSelf));
			foreach (var pool in statuePoolList)
			{
				if (pool.actor.enabled)
				{
					var cvs = pool.condition.gameObject;
					cvs.SetActive(Utility.FilpFlop(cvs.activeSelf));
				}
			}
		}
	}


	//------------------------------------------
	// 内部共有関数
	//------------------------------------------
	private Color GetColorDueFriendly(Friendly friendly)
	{
		if (friendly == Friendly.PlayerFriendly) return Color.green;
		if (friendly == Friendly.EnemyFriendly) return Color.red;
		return default;
	}
	private void SetActorTotalHP(Friendly friendly, float hp)
	{
		if(friendly == Friendly.PlayerFriendly)
		{
			playerHPTotal += hp;
			playerHPMaxTotal = playerHPTotal;
		}
		else if(friendly == Friendly.EnemyFriendly)
		{
			enemyHPTotal += hp;
			enemyHPMaxTotal = enemyHPTotal;
		}
	}
}
