using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonUtility;
using InstanceManager;

public class CVSActorState : MonoBehaviour
{
	[SerializeField] Button displayBtn = default;
	[SerializeField] GameObject dispalyBtnObj = default;
    [SerializeField] GameObject circlePrefab = default;

	private List<StatuePool> statuePoolList = new List<StatuePool>();
    private BattleManager battleManager;
	private float circleOffset = 0.7f;
	private float circleSpeed = 7f;
    private bool isVisualizeState = false;

	void Start()
    {
        battleManager = GameObject.FindWithTag("BattleManager").GetComponent<BattleManager>();
        battleManager.OnBeginBattleNotifyerHandler = OnBeginBattleReciever;

		displayBtn.onClick.AddListener(OnDisplayButton);
		dispalyBtnObj.SetActive(false);
    }
	private void Update()
	{
		if(statuePoolList != null && isVisualizeState)
		{
			foreach(var pool in statuePoolList)
			{
				if (pool.actor.enabled)
				{
					var actorPos = pool.actor.transform.position;
					var poolPos = pool.obj.transform.position;
					float deltaY = Mathf.Lerp(poolPos.y, actorPos.y - circleOffset, Time.deltaTime * circleSpeed);
					pool.obj.transform.position = new Vector3(actorPos.x, deltaY, 0);
				}
				else
				{
					if (pool.obj.activeSelf) pool.obj.SetActive(false);
				}
			}
		}
	}

	private void OnDisplayButton()
	{
		isVisualizeState = Utility.FilpFlop(isVisualizeState);
		foreach(var pool in statuePoolList)
		{
			if (pool.actor.enabled) pool.obj.SetActive(Utility.FilpFlop(pool.obj.activeSelf));
		}
	}
	private void OnBeginBattleReciever(List<ActorBase> actorList)
	{
		dispalyBtnObj.SetActive(true);
		foreach (var actor in actorList)
		{
			var circle = Instantiate(circlePrefab, transform);
			circle.SetActive(false);
			statuePoolList.Add(new StatuePool(actor, circle, GetColorDueFriendly(actor.Friendly)));
		}
	}
	private Color GetColorDueFriendly(Friendly friendly)
	{
		if (friendly == Friendly.PlayerFriendly) return Color.green;
		if (friendly == Friendly.EnemyFriendly) return Color.red;
		return default;
	}
}
