using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CVSResult : MonoBehaviour
{
	[SerializeField] Button home01Btn = default;
	[SerializeField] Button home02Btn = default;
	[SerializeField] Button nextBtn = default;
	[SerializeField] Button restartBtn = default;

	[SerializeField] GameObject cvsActorState = default;
	[SerializeField] GameObject winPanel = default;
	[SerializeField] GameObject losePanel = default;

	private BattleManager battleManager;

	private void Start()
	{
		battleManager = FindObjectOfType<BattleManager>();

		home01Btn.onClick.AddListener(OnHomeButton);
		home02Btn.onClick.AddListener(OnHomeButton);
		nextBtn.onClick.AddListener(OnNextButton);
		restartBtn.onClick.AddListener(OnRestartButton);
	}


	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
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


	//------------------------------------------
	// イベントリスナー
	//------------------------------------------
	private void OnHomeButton()
	{
		GameManager.instance.OnSceneTransition(SceneName.SelectScene);
	}
	private void OnNextButton()
	{
		GameManager.instance.OnSceneTransition(battleManager.Stage.NextScene);
	}
	private void OnRestartButton()
	{
		GameManager.instance.OnSceneTransition(battleManager.Stage.CurrentScene);
	}
}
