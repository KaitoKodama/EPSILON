using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(StageData))]
public class StageSOEditor : Editor
{
	GUIContent enableActorParamsLabel;
	SerializedProperty enableActorParamsProp;

	GUIContent simulateModeLabel;
	SerializedProperty simulateModeProp;

	GUIContent currentSceneLabel;
	SerializedProperty currentSceneProp;

	GUIContent nextSceneLabel;
	SerializedProperty nextSceneProp;

	GUIContent gainPatchLabel;
	SerializedProperty gainPatchProp;

	GUIContent enableCostLabel;
	SerializedProperty enableCostProp;


	private void OnEnable()
	{
		enableActorParamsLabel = new GUIContent("使用可能キャラデータ");
		enableActorParamsProp = serializedObject.FindProperty("enableActorParams");

		simulateModeLabel = new GUIContent("ゲームモード選択");
		simulateModeProp = serializedObject.FindProperty("simulateMode");

		currentSceneLabel = new GUIContent("現在のシーン名");
		currentSceneProp = serializedObject.FindProperty("currentScene");

		nextSceneLabel = new GUIContent("遷移先シーン名");
		nextSceneProp = serializedObject.FindProperty("nextScene");

		gainPatchLabel = new GUIContent("取得可能パッチ");
		gainPatchProp = serializedObject.FindProperty("gainPatch");

		enableCostLabel = new GUIContent("使用可能コスト");
		enableCostProp = serializedObject.FindProperty("enableCost");
	}
	public override void OnInspectorGUI()
	{
		// 最新データを取得
		serializedObject.Update();

		EditorGUILayout.HelpBox("基本設定", MessageType.None);
		EditorGUILayout.PropertyField(simulateModeProp, simulateModeLabel);
		EditorGUILayout.PropertyField(enableActorParamsProp, enableActorParamsLabel);
		EditorGUILayout.Space(20);

		if (simulateModeProp.enumValueIndex == 0)
		{
			EditorGUILayout.HelpBox("詳細設定", MessageType.None);
			EditorGUILayout.PropertyField(enableCostProp, enableCostLabel);
			EditorGUILayout.PropertyField(gainPatchProp, gainPatchLabel);
			EditorGUILayout.Space(20);
		}

		EditorGUILayout.HelpBox("遷移設定", MessageType.None);
		EditorGUILayout.PropertyField(currentSceneProp, currentSceneLabel);
		EditorGUILayout.PropertyField(nextSceneProp, nextSceneLabel);

		serializedObject.ApplyModifiedProperties();
	}
}


[CustomEditor(typeof(BattleManager))]
public class BattleManagerEditor : Editor
{
	GUIContent stageLabel;
	SerializedProperty stageProp;

	GUIContent requestEnemyUnitsLabel;
	SerializedProperty requestEnemyUnitsProp;

	GUIContent disableObjectsLabel;
	SerializedProperty disableObjectsProp;

	GUIContent editorUnitEditLabel;
	SerializedProperty editorUnitEditProp; 


	private void OnEnable()
	{
		stageLabel = new GUIContent("ステージ情報格納");
		stageProp = serializedObject.FindProperty("stage");

		requestEnemyUnitsLabel = new GUIContent("敵キャラ登録");
		requestEnemyUnitsProp = serializedObject.FindProperty("requestEnemyUnits");

		editorUnitEditLabel = new GUIContent("登録の有効化");
		editorUnitEditProp = serializedObject.FindProperty("editorUnitEdit");

		disableObjectsLabel = new GUIContent("非表示オブジェクト");
		disableObjectsProp = serializedObject.FindProperty("disableObjects");
	}
	public override void OnInspectorGUI()
	{
		// 最新データを取得
		serializedObject.Update();

		EditorGUILayout.HelpBox("ゲーム設定", MessageType.None);
		EditorGUILayout.PropertyField(stageProp, stageLabel);
		EditorGUILayout.PropertyField(editorUnitEditProp, editorUnitEditLabel);
		if (editorUnitEditProp.boolValue)
		{
			EditorGUILayout.PropertyField(requestEnemyUnitsProp, requestEnemyUnitsLabel);
		}
		EditorGUILayout.Space(20);

		EditorGUILayout.HelpBox("オブジェクト設定", MessageType.None);
		EditorGUILayout.PropertyField(disableObjectsProp, disableObjectsLabel);

		serializedObject.ApplyModifiedProperties();
	}
}
#endif