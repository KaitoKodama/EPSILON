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
		var async = SceneManager.LoadSceneAsync(((int)name));
		yield return new WaitUntil(() => async.isDone);

		isTransiting = false;
	}
}
