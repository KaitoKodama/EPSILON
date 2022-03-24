using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CVSCondition : MonoBehaviour
{
	[SerializeField] Image circleImage;
	private float previousClampHP;
	public void InitColor(Color color)
	{
		circleImage.color = color;
	}
	public void SetCircleFill(float clampHP)
	{
		if (previousClampHP != clampHP)
		{
			previousClampHP = clampHP;
			circleImage.DOFillAmount(clampHP, 0.5f);
		}
	}
}
