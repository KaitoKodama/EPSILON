using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CVSUnitGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler
{
	[SerializeField] Image unitImg = default;
	[SerializeField] Text costTxt = default;
	[SerializeField] GameObject unitRequestPrefab = default;

	private CVSUnitSelecter unitSelecter;
	private ActorParam reruestActorParam;
	private RequestUnit requestUnit;


	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
	public void InitGrid(CVSUnitSelecter unitSelecter, ActorParam reruestActorParam)
	{
		this.unitSelecter = unitSelecter;
		this.reruestActorParam = reruestActorParam;

		unitImg.sprite = reruestActorParam.ActorSprite;
		costTxt.text = reruestActorParam.Cost.ToString();
	}


	//------------------------------------------
	// インターフェイス
	//------------------------------------------
	public void OnPointerClick(PointerEventData eventData)
	{
		unitSelecter.OnDetailDisplay(reruestActorParam);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (unitSelecter.IsEnableToGenerate(reruestActorParam.Cost))
		{
			var requestGrid = Instantiate(unitRequestPrefab);
			var requet = requestGrid.GetComponent<CVSUnitRequest>();
			requestUnit = new RequestUnit(reruestActorParam, requestGrid);
			requet.InitRequestGrid(unitSelecter, requestUnit);
			unitSelecter.OnAddRequestUnit(requestUnit);
		}
	}
	public void OnDrag(PointerEventData eventData)
	{
		if(requestUnit != null)
		{
			var pos = Camera.main.ScreenToWorldPoint(eventData.position);
			requestUnit.requestGrid.transform.position = unitSelecter.GetDragAreaInRange(pos);
		}
	}
}
