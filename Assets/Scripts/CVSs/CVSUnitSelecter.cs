using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CommonUtility;

public class CVSUnitSelecter : MonoBehaviour
{
	[Header("----- ボタンイベント -----")]
	[SerializeField] Button switchTargetUnitBtn = default;
	[SerializeField] Button unitButton = default;
	[SerializeField] Button battleBeginButton = default;

	[Header("----- ログテキスト -----")]
	[SerializeField] Text costTxt = default;

	[Header("----- 詳細表示 -----")]
	[SerializeField] CVSUnitDetail unitDetail = default;

	[Header("----- メインUIのアニメーション -----")]
	[SerializeField] Image unitBtnImg = default;
	[SerializeField] Transform unitParent = default;
	[SerializeField] GameObject unitMainArrow = default;

	[Header("----- 選択可能ユニット -----")]
	[SerializeField] GameObject unitGroupPanel = default;
	[SerializeField] GameObject unitGridPrefab = default;

	[Header("----- モード別非表示オブジェクト -----")]
	[SerializeField] GameObject switchTargetUnitButtonObject = default;


	private List<RequestUnit> requestPlayerUnitList = new List<RequestUnit>();
	private List<RequestUnit> requestEnemyUnitList = new List<RequestUnit>();
	private BattleCVSSoundManager soundManager;
	private BattleManager battleManager;
	private RectTransform unitGroupRect;
	
	private float enableCost = 0;
	private bool isUnitGroupOpen = false;


	private void Start()
	{
		InitEventSystems();
		soundManager = GetComponentInParent<BattleCVSSoundManager>();
		unitBtnImg.DOFade(0.8f, 1f).SetLoops(-1, LoopType.Yoyo);
		unitGroupRect = unitGroupPanel.GetComponent<RectTransform>();
		unitGroupRect.anchoredPosition = new Vector2(0, -47.1f);

		battleManager = FindObjectOfType<BattleManager>();
		if (battleManager.Stage.SimulateMode == SimulateMode.UnitBattle)
		{
			enableCost = battleManager.Stage.EnableCost;
			switchTargetUnitButtonObject.SetActive(false);
			SetUnitCostText();
		}
		else if (battleManager.Stage.SimulateMode == SimulateMode.UnitFreedom)
		{
			switchTargetUnitButtonObject.SetActive(true);
			costTxt.text = GetTargetUnitString();
		}
		foreach (var actorParam in battleManager.Stage.EnableActorParams)
		{
			var grid = Instantiate(unitGridPrefab, unitParent).GetComponent<CVSUnitGrid>();
			grid.InitGrid(this, actorParam);
		}
	}



	//------------------------------------------
	// 内部共有関数
	//------------------------------------------
	private void InitEventSystems()
	{
		battleBeginButton.onClick.AddListener(OnBeginBattleButton);
		unitButton.onClick.AddListener(OnUnitButton);
		switchTargetUnitBtn.onClick.AddListener(OnSwitchUnitTargetButton);
	}
	private void SetUnitCostText()
	{
		costTxt.text = "利用可能コスト：" + enableCost.ToString();
	}
	private List<RequestUnit> GetTargetUnitList()
	{
		if (battleManager.RequestTarget == RequestTarget.PlayerUnit) return requestPlayerUnitList;
		if (battleManager.RequestTarget == RequestTarget.EnemyUnit) return requestEnemyUnitList;
		return null;
	}
	private string GetTargetUnitString()
	{
		if (battleManager.RequestTarget == RequestTarget.PlayerUnit) return "Unitプレイヤー";
		if (battleManager.RequestTarget == RequestTarget.EnemyUnit) return "Unitエネミー";
		return null;
	}
	private bool IsSimulatable()
	{
		bool playerCondition = requestPlayerUnitList != null && requestPlayerUnitList.Count > 0;
		bool enemyCondition = requestEnemyUnitList != null && requestEnemyUnitList.Count > 0;
		if (battleManager.Stage.SimulateMode == SimulateMode.UnitBattle)
		{
			if (playerCondition) return true;
		}
		if (battleManager.Stage.SimulateMode == SimulateMode.UnitFreedom)
		{
			if (playerCondition && enemyCondition) return true;
		}
		return false;
	}


	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
	public void OnRemoveRequestUnit(RequestUnit cancelUnit)
	{
		soundManager.OnPointSound();
		if (battleManager.Stage.SimulateMode == SimulateMode.UnitBattle)
		{
			enableCost += cancelUnit.param.Cost;
			requestPlayerUnitList.Remove(cancelUnit);
			SetUnitCostText();
		}
		else if (battleManager.Stage.SimulateMode == SimulateMode.UnitFreedom)
		{
			GetTargetUnitList()?.Remove(cancelUnit);
		}
	}
	public void OnAddRequestUnit(RequestUnit requestUnit)
	{
		soundManager.OnPointSound();
		if (battleManager.Stage.SimulateMode == SimulateMode.UnitBattle)
		{
			enableCost -= requestUnit.param.Cost;
			requestPlayerUnitList.Add(requestUnit);
			SetUnitCostText();
		}
		else if (battleManager.Stage.SimulateMode == SimulateMode.UnitFreedom)
		{
			GetTargetUnitList()?.Add(requestUnit);
		}
	}
	public void OnDetailDisplay(ActorParam param)
	{
		soundManager.OnPointSound();
		unitDetail.OnActivatePanel(param);
	}
	public Vector3 GetDragAreaInRange(Vector3 position)
	{
		position.z = 0;
		if (battleManager.RequestTarget == RequestTarget.PlayerUnit)
		{
			if (position.x > -1) position.x = -1;
		}
		if (battleManager.RequestTarget == RequestTarget.EnemyUnit)
		{
			if (position.x < 1) position.x = 1;
		}
		return position;
	}
	public Quaternion GetTargetRotate()
	{
		if (battleManager.RequestTarget == RequestTarget.PlayerUnit) return Quaternion.Euler(0, 180, 0);
		if (battleManager.RequestTarget == RequestTarget.EnemyUnit) return Quaternion.Euler(0, 0, 0);
		return default;
	}
	public bool IsEnableToGenerate(float theCost)
	{
		if (battleManager.Stage.SimulateMode == SimulateMode.UnitFreedom) return true;
		float virtualCost = enableCost - theCost;
		if (virtualCost >= 0) return true;
		else return false;
	}


	//------------------------------------------
	// イベントシステム
	//------------------------------------------
	private void OnUnitButton()
	{
		soundManager.OnPointSound();
		if (!isUnitGroupOpen)
		{
			isUnitGroupOpen = true;
			unitMainArrow.transform.rotation = Quaternion.Euler(0, 0, 0);
			unitGroupRect.DOAnchorPosY(71.315f, 0.5f);
		}
		else
		{
			isUnitGroupOpen = false;
			unitMainArrow.transform.rotation = Quaternion.Euler(180, 0, 0);
			unitGroupRect.DOAnchorPosY(-47.1f, 0.5f);
		}
	}
	private void OnSwitchUnitTargetButton()
	{
		soundManager.OnPointSound();
		var target = Utility.GetNextEnum<RequestTarget>(((int)battleManager.RequestTarget));
		battleManager.RequestTarget = target;
		costTxt.text = GetTargetUnitString();
	}
	private void OnBeginBattleButton()
	{
		if (IsSimulatable())
		{
			soundManager.OnPointSound();
			battleManager.OnBeginBattle(requestPlayerUnitList, requestEnemyUnitList);
		}
		else costTxt.text = "ユニットを選択ください";
	}
}
