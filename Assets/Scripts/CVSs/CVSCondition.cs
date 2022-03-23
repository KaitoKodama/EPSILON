using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CVSCondition : MonoBehaviour
{
	[SerializeField] Image circleImage;

	public void InitColor(Color color)
	{
		circleImage.color = color;
	}
	public void SetCircleFill(float clampHP)
	{
		circleImage.DOFillAmount(clampHP, 0.5f);
	}
}
