using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CVSUnitGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	[SerializeField] Image unitImg = default;
	[SerializeField] Text costTxt = default;
	[SerializeField] GameObject unitRequestPrefab = default;

	private CVSUnitSelecter unitSelecter;
	private SmoothCamera smoothCamera;
	private RequestUnit requestUnit;
	private ActorUnit actorUnit;


	private void Start()
	{
		smoothCamera = Camera.main.GetComponent<SmoothCamera>();
	}


	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
	public void InitGrid(CVSUnitSelecter unitSelecter, GameObject obj)
	{
		this.unitSelecter = unitSelecter;
		actorUnit = new ActorUnit(obj);
		unitImg.sprite = actorUnit.param.ActorSprite;
		costTxt.text = actorUnit.param.Cost.ToString();
	}


	//------------------------------------------
	// インターフェイス
	//------------------------------------------
	public void OnPointerClick(PointerEventData eventData)
	{
		unitSelecter.OnDetailDisplay(actorUnit.param);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		smoothCamera.enabled = false;
		if (unitSelecter.IsEnableToGenerate(actorUnit.param.Cost))
		{
			var requestGrid = Instantiate(unitRequestPrefab);
			var requet = requestGrid.GetComponent<CVSUnitRequest>();
			requestUnit = new RequestUnit(actorUnit, requet, requestGrid);

			requet.InitRequestGrid(requestUnit);
			unitSelecter.OnRequestGenerate(requestUnit);
		}
	}
	public void OnDrag(PointerEventData eventData)
	{
		if(requestUnit != null)
		{
			var pos = Camera.main.ScreenToWorldPoint(eventData.position);
			pos.z = 0;
			requestUnit.requestGrid.transform.position = pos;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		smoothCamera.enabled = true;
	}
}
