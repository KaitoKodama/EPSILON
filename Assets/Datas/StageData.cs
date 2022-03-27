using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Stage", fileName ="Stage")]
public class StageData : ScriptableObject
{
	[SerializeField] ActorParam[] enableActorParams = default;
	[SerializeField] SimulateMode simulateMode = SimulateMode.UnitBattle;
	[SerializeField] SceneName currentScene = default;
	[SerializeField] SceneName nextScene = default;
	[SerializeField] StagePatch gainPatch = default;
	[SerializeField] float enableCost = default;

	public ActorParam[] EnableActorParams { get => enableActorParams; }
	public SimulateMode SimulateMode { get => simulateMode; }
	public SceneName CurrentScene { get => currentScene; }
	public SceneName NextScene { get => nextScene; }
	public StagePatch GainPatch { get => gainPatch; }
	public float EnableCost { get => enableCost; }
}
