using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Stage", fileName ="Stage")]
public class StageData : ScriptableObject
{
	[Header("-- 利用可能キャラプレハブ --")]
	[Space(20)][SerializeField] GameObject[] enablePrefabs;

	[Header("-- 利用可能コスト --")]
	[Space(20)][SerializeField] float enableCost;

	public GameObject[] EnablePrefabs { get => enablePrefabs; }
	public float EnableCost { get => enableCost; }
}
