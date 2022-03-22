using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonUtility;

public class CVSActorState : MonoBehaviour
{
	[SerializeField] Button stateButton;
	[SerializeField] GameObject conditionPrefab;

	[SerializeField]private List<StatuePool> statuePoolList;
	private BattleManager battleManager;
	private float circleOffset = 0.6f;
	private bool isVisualize = false;

	private void Start()
	{
		battleManager = FindObjectOfType<BattleManager>();
		battleManager.OnBattleBeginNotifyerHandler = OnBattleBeginReciever;
		stateButton.onClick.AddListener(OnStateButton);
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
					var to = pool.actor.transform.position;
					to.x += circleOffset;
					to.y += circleOffset;
					pool.condition.transform.position = to;
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

	private void OnBattleBeginReciever(List<ActorBase> actorList)
	{
		statuePoolList = new List<StatuePool>();
		foreach (var actor in actorList)
		{
			var circle = Instantiate(conditionPrefab, transform);
			var condition = circle.GetComponent<CVSCondition>();
			circle.SetActive(false);
			statuePoolList.Add(new StatuePool(actor, condition, GetColorDueFriendly(actor.Friendly)));
		}
	}
	private void OnStateButton()
	{
		if(statuePoolList != null)
		{
			isVisualize = Utility.FilpFlop(isVisualize);
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
	private Color GetColorDueFriendly(Friendly friendly)
	{
		if (friendly == Friendly.PlayerFriendly) return Color.green;
		if (friendly == Friendly.EnemyFriendly) return Color.red;
		return default;
	}
}
