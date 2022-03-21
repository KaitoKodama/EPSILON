using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CVSResult : MonoBehaviour
{
	[SerializeField] GameObject cvsActorState;
	[SerializeField] GameObject winPanel;
	[SerializeField] GameObject losePanel;

	public void OnPlayerWinThebattle()
	{
		cvsActorState.SetActive(false);
		winPanel.SetActive(true);
	}
	public void OnPlayerLoseTheBattle()
	{
		cvsActorState.SetActive(false);
		losePanel.SetActive(true);
	}
}
