using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CVSUnitRequest : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] Button requestCancelBtn;
    [SerializeField] Image gridImg = default;

    private RequestUnit requestUnit;
    private SmoothCamera smoothCamera;
    private Transform _transform;

	void Start()
    {
        requestCancelBtn.onClick.AddListener(OnRequestCancelBtn);
        smoothCamera = Camera.main.GetComponent<SmoothCamera>();
        _transform = transform;
    }


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public void InitRequestGrid(RequestUnit unit)
	{
        requestUnit = unit;
        gridImg.sprite = unit.actorUnit.param.ActorSprite;

        var cvs = GetComponent<Canvas>();
        cvs.worldCamera = Camera.main;
        cvs.sortingOrder = 30;
    }

    //------------------------------------------
    // イベントハンドラー
    //------------------------------------------
    private void OnRequestCancelBtn()
	{
        FindObjectOfType<CVSUnitSelecter>().OnRequestCancel(requestUnit);
        gameObject.SetActive(false);
	}


    //------------------------------------------
    // インターフェイス
    //------------------------------------------
    public void OnBeginDrag(PointerEventData eventData)
    {
        smoothCamera.enabled = false;
    }
    public void OnDrag(PointerEventData eventData)
	{
        var pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = 0;
        _transform.position = pos;
	}
	public void OnEndDrag(PointerEventData eventData)
	{
        smoothCamera.enabled = true;
	}
}
