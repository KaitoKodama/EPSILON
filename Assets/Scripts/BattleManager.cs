using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtility;

public class BattleManager : MonoBehaviour
{
	[Header("利用可能キャラデータ")]
	[Header("------ 戦闘ユニット情報 ------")]
	[SerializeField] ActorParam[] enableActorParams = default;
	[Header("敵キャラデータ")]
	[SerializeField] RequestEnemyUnit[] requestEnemyUnits = default;
	[Header("利用可能コスト")]
	[SerializeField] float enableCost = default;

	[Header("現在のシーン名")]
	[Header("------ 保存と遷移用情報 ------")]
	[Space(20)]
	[SerializeField] SceneName currentScene = default;
	[Header("遷移後のシーン名")]
	[SerializeField] SceneName nextScene = default;
	[Header("獲得パッチ")]
	[SerializeField] StagePatch gainPatch = default;

	[Header("非表示オブジェクト")]
	[Header("------ 非アクティブ化したいオブジェクト ------")]
	[Space(20)]
	[SerializeField] GameObject[] disableObjects = default;

	private List<ActorBase> actorList = new List<ActorBase>();
	private CVSResult cvsResult;
	private int playerNum;
	private int enemyNum;
	private bool isBattleBegin = false;


	private void Start()
	{
		cvsResult = GetComponentInChildren<CVSResult>();

		foreach(var el in requestEnemyUnits)
		{
			var enemy = Instantiate(el.param.ActorPrefab, el.location.position, Quaternion.identity);
			var actor = enemy.GetComponent<ActorBase>();
			actor.InitActor(el.param, Friendly.EnemyFriendly);
			actorList.Add(actor);
			enemyNum += 1;
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
	public ActorParam[] EnableActorParams { get => enableActorParams; }
	public SceneName CurrentScene { get => currentScene; }
	public SceneName NextScene { get => nextScene; }
	public StagePatch GainPatch { get => gainPatch; }
	public float EnableCost { get => enableCost; }

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


	public void OnBeginBattle(List<RequestUnit> requestUnitList)
	{
		foreach(var obj in disableObjects)
		{
			obj.SetActive(false);
		}

		foreach (var unit in requestUnitList)
		{
			var actorObj = Instantiate(unit.param.ActorPrefab, unit.requestGrid.transform.position, Quaternion.identity);
			var actor = actorObj.GetComponent<ActorBase>();
			actor.InitActor(unit.param, Friendly.PlayerFriendly);
			unit.requestGrid.SetActive(false);
			actorList.Add(actor);
			playerNum += 1;
		}

		foreach(var actor in actorList)
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
}
