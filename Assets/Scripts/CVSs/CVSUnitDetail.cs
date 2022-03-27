using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CVSUnitDetail : MonoBehaviour
{
	[SerializeField] Button closeBtn = default;
	[SerializeField] Text nameTxt = default;
	[SerializeField] Text costTxt = default;
	[SerializeField] Text explainTxt = default;
	[SerializeField] Image actorImg = default;

	private BattleCVSSoundManager soundManager;

	private void Start()
	{
		soundManager = GetComponentInParent<BattleCVSSoundManager>();
		closeBtn.onClick.AddListener(OnCloseButton);
	}
	public void OnActivatePanel(ActorParam param)
	{
		gameObject.SetActive(true);
		nameTxt.text = "名称：" + param.ActorName;
		costTxt.text = "コスト：" + param.Cost.ToString() + "G";
		explainTxt.text = param.ActorExplain;
		actorImg.sprite = param.ActorSprite;
	}
	private void OnCloseButton()
	{
		soundManager.OnPointSound();
		gameObject.SetActive(false);
	}
}
