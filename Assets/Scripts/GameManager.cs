using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	static public GameManager instance;

	[Header("----- フェードIO -----")]
	[SerializeField] GameObject fadeIOPanel;
	[SerializeField] CanvasGroup fadeGroup;
	[SerializeField] Image loadingFillImage;
	private float fadeDuration = 1f;
	private bool isTransiting = false;


	//------------------------------------------
	// Unityランタイム
	//------------------------------------------
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	private void Start()
	{
		fadeGroup.alpha = 0;
	}



	//------------------------------------------
	// 外部共有関数
	//------------------------------------------
	public void OnSceneTransition(SceneName name)
	{
		if (!isTransiting)
		{
			isTransiting = true;
			StartCoroutine(SceneTransition(name));
		}
	}
	private IEnumerator SceneTransition(SceneName name)
	{
		fadeIOPanel.SetActive(true);
		fadeGroup.DOFade(1, fadeDuration);
		yield return new WaitForSeconds(fadeDuration);

		var async = SceneManager.LoadSceneAsync(name.ToString());
		if (async != null)
		{
			loadingFillImage.fillAmount = Mathf.Clamp01(async.progress);
			while (async.isDone)
			{
				break;
			}
		}
		yield return new WaitUntil(() => async.isDone);

		fadeGroup.DOFade(0, fadeDuration);
		yield return new WaitForSeconds(fadeDuration);

		fadeIOPanel.SetActive(false);
		isTransiting = false;
	}
}
