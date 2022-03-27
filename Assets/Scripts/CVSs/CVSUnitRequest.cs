using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CVSUnitRequest : MonoBehaviour, IDragHandler
{
    [SerializeField] Button requestCancelBtn = default;
    [SerializeField] Image gridImg = default;
    [SerializeField] GameObject gridImageObj = default;
    private CVSUnitSelecter unitSelecter;
    private RequestUnit requestUnit;


	void Start()
    {
        requestCancelBtn.onClick.AddListener(OnRequestCancelBtn);
    }


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public void InitRequestGrid(CVSUnitSelecter unitSelecter, RequestUnit request)
	{
        gridImageObj.transform.rotation = unitSelecter.GetTargetRotate();
        this.unitSelecter = unitSelecter;
        requestUnit = request;
        gridImg.sprite = request.param.ActorSprite;
        var cvs = GetComponent<Canvas>();
        cvs.worldCamera = Camera.main;
        cvs.sortingOrder = 30;
    }

    //------------------------------------------
    // イベントハンドラー
    //------------------------------------------
    private void OnRequestCancelBtn()
	{
        unitSelecter.OnRemoveRequestUnit(requestUnit);
        gameObject.SetActive(false);
	}


    //------------------------------------------
    // インターフェイス
    //------------------------------------------
    public void OnDrag(PointerEventData eventData)
	{
        var pos = Camera.main.ScreenToWorldPoint(eventData.position);
        requestUnit.requestGrid.transform.position = unitSelecter.GetDragAreaInRange(pos);
    }
}
