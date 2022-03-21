using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CVSUnitDetail : MonoBehaviour
{
	[SerializeField] Button closeBtn;
	[SerializeField] Text nameTxt;
	[SerializeField] Text costTxt;
	[SerializeField] Text explainTxt;
	[SerializeField] Image actorImg;


	private void Start()
	{
		closeBtn.onClick.AddListener(()=> { gameObject.SetActive(false); });
		gameObject.SetActive(false);
	}
	public void OnActivatePanel(ActorParam param)
	{
		nameTxt.text = "名称：" + param.ActorName;
		costTxt.text = "コスト：" + param.Cost.ToString() + "G";
		explainTxt.text = "説明：" + param.ActorExplain;
		actorImg.sprite = param.ActorSprite;
		gameObject.SetActive(true);
	}
}
