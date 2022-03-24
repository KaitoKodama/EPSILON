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
	private float fullPlayerPreviousHP, fullPlayerCurrentHP, fullPlayerMaxHP;
	private float fullEnemyPreviousHP, fullEnemyCurrentHP, fullEnemyMaxHP;
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
			ResetFullActorHP();
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
					SetCurrentActorHP(pool.actor.Friendly, pool.actor.HP);
				}
				else
				{
					if (pool.condition.gameObject.activeSelf)
					{
						pool.condition.gameObject.SetActive(false);
					}
				}
			}
			RefleshFillAmountDuetoGap();
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
			SetFullActorMaxHP(actor.Friendly, actor.MaxHP);
			var circle = Instantiate(conditionPrefab, conditionParent.transform);
			var condition = circle.GetComponent<CVSCondition>();
			circle.SetActive(false);
			statuePoolList.Add(new StatuePool(actor, condition, GetColorDueFriendly(actor.Friendly)));
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
		if (friendly == Friendly.PlayerFriendly) return Color.white;
		if (friendly == Friendly.EnemyFriendly) return Color.black;
		return default;
	}

	private void ResetFullActorHP()
	{
		fullPlayerCurrentHP = 0;
		fullEnemyCurrentHP = 0;
	}
	private void SetFullActorMaxHP(Friendly friendly, float maxHP)
	{
		if (friendly == Friendly.PlayerFriendly) fullPlayerMaxHP += maxHP;
		else if (friendly == Friendly.EnemyFriendly) fullEnemyMaxHP += maxHP;
	}
	private void SetCurrentActorHP(Friendly friendly, float hp)
	{
		if (friendly == Friendly.PlayerFriendly) fullPlayerCurrentHP += hp;
		else if(friendly == Friendly.EnemyFriendly) fullEnemyCurrentHP += hp;
	}
	private void RefleshFillAmountDuetoGap()
	{
		if(fullPlayerCurrentHP != fullPlayerPreviousHP)
		{
			playerFill.DOFillAmount(fullPlayerCurrentHP / fullPlayerMaxHP, 0.3f);
			fullPlayerPreviousHP = fullPlayerCurrentHP;
		}
		if (fullEnemyCurrentHP != fullEnemyPreviousHP)
		{
			enemyFill.DOFillAmount(fullEnemyCurrentHP / fullEnemyMaxHP, 0.3f);
			fullEnemyPreviousHP = fullEnemyCurrentHP;
		}
	}
}
