using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtility;

public class BattleManager : MonoBehaviour
{
	[Header("---------- ステージを設定 ----------")]
	[SerializeField] StageData stageData;
	[SerializeField] ActorBase[] enemys;

	private List<ActorBase> actorList = new List<ActorBase>();
	private CVSResult cvsResult;
	private int playerNum;
	private int enemyNum;
	private bool isBattleBegin = false;


	private void Start()
	{
		cvsResult = GetComponentInChildren<CVSResult>();
	}


	//------------------------------------------
	// デリゲート通知
	//------------------------------------------
	public delegate void OnBeginBattleNotifyer(List<ActorBase> actorList);
	public OnBeginBattleNotifyer OnBeginBattleNotifyerHandler;


	//------------------------------------------
	// デリゲート通知受信
	//------------------------------------------
	private void OnDeathReciever(Friendly friendly)
	{
		switch (friendly)
		{
			case Friendly.PlayerFriendly:
				playerNum -= 1;
				ObserveActorNum(playerNum, cvsResult.OnPlayerLoseTheBattle);
				break;
			case Friendly.EnemyFriendly:
				enemyNum -= 1;
				ObserveActorNum(enemyNum, cvsResult.OnPlayerWinThebattle);
				break;
		}
	}


	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
	public StageData StageData { get => stageData; }
	public ActorBase GetTrackableActor(ActorBase actor)
	{
		if (actorList == null && !isBattleBegin) return null;

		ActorBase targetActor = null;
		float distance = float.MaxValue;

		foreach (var target in actorList)
		{
			if(target != null)
			{
				if(target.enabled && target.Friendly != actor.Friendly && Utility.Probability(target.Param.TrackRate))
				{
					float dist = Vector2.Distance(actor.transform.position, target.transform.position);
					if (dist <= distance)
					{
						distance = dist;
						targetActor = target;
					}
				}
			}
		}
		return targetActor;
	}
	public void OnBeginBattle(List<RequestUnit> requestUnitList)
	{
		foreach(var el in enemys)
		{
			el.Friendly = Friendly.EnemyFriendly;
			actorList.Add(el);
			enemyNum += 1;
		}

		foreach (var unit in requestUnitList)
		{
			var actorObj = Instantiate(unit.actorUnit.prefab, unit.requestGrid.transform.position, Quaternion.identity);
			var actor = actorObj.GetComponent<ActorBase>();
			actor.Friendly = Friendly.PlayerFriendly;
			unit.requestGrid.SetActive(false);
			actorList.Add(actor);
			playerNum += 1;
		}

		foreach(var actor in actorList)
		{
			actor.enabled = true;
			actor.OnDeathTheActorNotifyerHandler = OnDeathReciever;
		}

		OnBeginBattleNotifyerHandler?.Invoke(actorList);
		isBattleBegin = true;
	}


	//------------------------------------------
	// 内部共有関数
	//------------------------------------------
	private void ObserveActorNum(int num, Action action)
	{
		if (num <= 0) action();
	}
}
