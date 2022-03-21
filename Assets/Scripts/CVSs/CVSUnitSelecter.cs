using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CVSUnitSelecter : MonoBehaviour
{
	[SerializeField] Text costTxt = default;
	[SerializeField] Image unitBtnImg = default;
	[SerializeField] Button unitButton = default;
	[SerializeField] Button battleBeginButton = default;

	[SerializeField] CVSUnitDetail unitDetail = default;
	[SerializeField] Transform unitParent = default;
	[SerializeField] GameObject unitMainArrow = default;
	[SerializeField] GameObject unitGroupPanel = default;
	[SerializeField] GameObject unitGridPrefab = default;

	private BattleManager battleManager;
	private RectTransform unitGroupRect;
	private List<RequestUnit> requestUnitList = new List<RequestUnit>();
	private float enableCost = 0;
	private bool isUnitGroupOpen = false;


	private void Start()
	{
		InitEventSystems();
		unitBtnImg.DOFade(0.8f, 1f).SetLoops(-1, LoopType.Yoyo);
		unitGroupRect = unitGroupPanel.GetComponent<RectTransform>();
		unitGroupRect.anchoredPosition = new Vector2(0, -37f);

		battleManager = GameObject.FindWithTag("BattleManager").GetComponent<BattleManager>();
		enableCost = battleManager.StageData.EnableCost;
		foreach (var actorPrefab in battleManager.StageData.EnablePrefabs)
		{
			var grid = Instantiate(unitGridPrefab, unitParent).GetComponent<CVSUnitGrid>();
			grid.InitGrid(this, actorPrefab);
		}
	}

	//------------------------------------------
	// 内部共有関数
	//------------------------------------------
	private void InitEventSystems()
	{
		battleBeginButton.onClick.AddListener(OnBeginBattleButton);
		unitButton.onClick.AddListener(OnUnitButton);
		SetUnitCostText();
	}
	private void SetUnitCostText()
	{
		costTxt.text = "利用可能コスト：" + enableCost.ToString();
	}


	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
	public void OnRequestCancel(RequestUnit cancelUnit)
	{
		enableCost += cancelUnit.actorUnit.param.Cost;
		requestUnitList.Remove(cancelUnit);
		SetUnitCostText();
	}
	public void OnRequestGenerate(RequestUnit requestUnit)
	{
		enableCost -= requestUnit.actorUnit.param.Cost;
		requestUnitList.Add(requestUnit);
		SetUnitCostText();
	}
	public void OnDetailDisplay(ActorParam param)
	{
		unitDetail.OnActivatePanel(param);
	}

	public bool IsEnableToGenerate(float theCost)
	{
		float virtualCost = enableCost - theCost;
		if (virtualCost >= 0) return true;
		else return false;
	}


	//------------------------------------------
	// イベントシステム
	//------------------------------------------
	private void OnUnitButton()
	{
		if (!isUnitGroupOpen)
		{
			isUnitGroupOpen = true;
			unitMainArrow.transform.rotation = Quaternion.Euler(0, 0, 0);
			unitGroupRect.DOAnchorPosY(71f, 0.5f);
		}
		else
		{
			isUnitGroupOpen = false;
			unitMainArrow.transform.rotation = Quaternion.Euler(180, 0, 0);
			unitGroupRect.DOAnchorPosY(-37f, 0.5f);
		}
	}
	private void OnBeginBattleButton()
	{
		if(requestUnitList != null && requestUnitList.Count > 0)
		{
			unitGroupPanel.SetActive(false);
			battleManager.OnBeginBattle(requestUnitList);
		}
		else
		{
			costTxt.text = "ユニットを選択ください";
		}
	}
}
