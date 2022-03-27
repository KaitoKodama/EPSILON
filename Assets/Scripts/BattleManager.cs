using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtility;

public class BattleManager : MonoBehaviour
{
	[SerializeField] StageData stage;
	[SerializeField] RequestEnemyUnit[] requestEnemyUnits = default;
	[SerializeField] GameObject[] disableObjects = default;
	[SerializeField] bool editorUnitEdit = true;


	private List<ActorBase> actorList = new List<ActorBase>();
	private CVSResult cvsResult;
	private RequestTarget requestTarget = RequestTarget.PlayerUnit;
	private int playerNum;
	private int enemyNum;
	private bool isBattleBegin = false;




	private void Start()
	{
		cvsResult = GetComponentInChildren<CVSResult>();
		if (stage.SimulateMode == SimulateMode.UnitBattle)
		{
			foreach (var el in requestEnemyUnits)
			{
				var enemy = Instantiate(el.param.ActorPrefab, el.location.position, Quaternion.identity);
				var actor = enemy.GetComponent<ActorBase>();
				actor.InitActor(el.param, Friendly.EnemyFriendly);
				actor.enabled = false;
				actorList.Add(actor);
				enemyNum += 1;
			}
		}
	}

	//------------------------------------------
	// デリゲート通知
	//------------------------------------------
	public delegate void OnBattleBeginNotifyer(List<ActorBase> actorList);
	public OnBattleBeginNotifyer OnBattleBeginNotifyerHandler;



	//------------------------------------------
	// デリゲート通知受信
	//------------------------------------------
	protected void OnDeathReciever(Friendly friendly)
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
	public StageData Stage { get => stage; }
	public RequestTarget RequestTarget { get => requestTarget; set => requestTarget = value; }

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
	public List<ActorBase> GetActorList()
	{
		if (actorList == null && !isBattleBegin) return null;
		return actorList;
	}


	public void OnBeginBattle(List<RequestUnit> requestPlayerUnitList, List<RequestUnit> requestEnemyUnitList)
	{
		foreach(var obj in disableObjects)
		{
			obj.SetActive(false);
		}
		if (stage.SimulateMode == SimulateMode.UnitFreedom)
		{
			OnSetupRequestUnit(requestEnemyUnitList, Friendly.EnemyFriendly, () => { enemyNum += 1; });
		}
		OnSetupRequestUnit(requestPlayerUnitList, Friendly.PlayerFriendly, () => { playerNum += 1; });

		foreach (var actor in actorList)
		{
			actor.enabled = true;
			actor.OnDeathTheActorNotifyerHandler = OnDeathReciever;
		}
		OnBattleBeginNotifyerHandler?.Invoke(actorList);
		isBattleBegin = true;
	}


	//------------------------------------------
	// 内部共有関数
	//------------------------------------------
	private void ObserveActorNum(int num, Action action)
	{
		if (num <= 0) action();
	}
	private void OnSetupRequestUnit(List<RequestUnit> requests, Friendly friendly, Action addition)
	{
		foreach (var unit in requests)
		{
			var actorObj = Instantiate(unit.param.ActorPrefab, unit.requestGrid.transform.position, Quaternion.identity);
			var actor = actorObj.GetComponent<ActorBase>();
			actor.InitActor(unit.param, friendly);
			unit.requestGrid.SetActive(false);
			actorList.Add(actor);
			addition();
		}
	}
}
