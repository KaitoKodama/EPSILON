using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//------------------------------------------
// クラス
//------------------------------------------
[System.Serializable]
public class AnyDictionary<Tkey, Tvalue>
{
	public Tkey key;
	public Tvalue value;

	public AnyDictionary(Tkey key, Tvalue value)
	{
		this.key = key;
		this.value = value;
	}
	public AnyDictionary(KeyValuePair<Tkey, Tvalue> pair)
	{
		this.key = pair.Key;
		this.value = pair.Value;
	}
}

// ユニット配置
public class RequestUnit
{
	public RequestUnit(ActorParam param, GameObject requestGrid)
	{
		this.param = param;
		this.requestGrid = requestGrid;
	}
	public ActorParam param;
	public GameObject requestGrid;
}

[System.Serializable]
public class RequestEnemyUnit
{
	public RequestEnemyUnit(ActorParam param, Transform location)
	{
		this.param = param;
		this.location = location;
	}
	public ActorParam param;
	public Transform location;
}

// ステート表示
public class StatuePool
{
	public StatuePool(ActorBase actor, CVSCondition condition, Color requestColor)
	{
		this.actor = actor;
		this.condition = condition;
		condition.InitColor(requestColor);
	}
	public ActorBase actor;
	public CVSCondition condition;
}


//------------------------------------------
// インターフェイス
//------------------------------------------


//------------------------------------------
// 列挙
//------------------------------------------
public enum Friendly
{
	PlayerFriendly,
	EnemyFriendly,
}
public enum RequestTarget
{
	PlayerUnit,
	EnemyUnit,
}
public enum SimulateMode
{
	UnitBattle,
	UnitFreedom,
}
public enum StagePatch
{
	None,
	Stage01Completed,
	Stage02Completed,
	Stage03Completed,
	Stage04Completed,
	Stage05Completed,
	Stage06Completed,
	Stage07Completed,
	Stage08Completed,
}
public enum SceneName
{
	TitleScene,
	SelectScene,
	FreedomScene,
	Stage01,
	Stage02,
	Stage03,
	Stage04,
	Stage05,
	Stage06,
	Stage07,
	Stage08,
}



//------------------------------------------
// ユーティリティ
//------------------------------------------
namespace CommonUtility
{
	public static class Utility
	{
		public static TValue GetDICVal<TValue, TKey>(TKey component, List<AnyDictionary<TKey, TValue>> dics)
		{
			foreach (var dic in dics)
			{
				if (dic.key.Equals(component))
				{
					return dic.value;
				}
			}
			return default;
		}
		public static T GetNextEnum<T>(int currentEnum)
		{
			int nextIndex = currentEnum + 1;
			T nextEnum = (T)Enum.ToObject(typeof(T), nextIndex);
			int length = Enum.GetValues(typeof(T)).Length;
			if (nextIndex >= length)
			{
				nextEnum = (T)Enum.ToObject(typeof(T), 0);
			}
			return nextEnum;
		}
		public static T GetIntToEnum<T>(int targetInt)
		{
			T targetEnum = (T)Enum.ToObject(typeof(T), targetInt);
			return targetEnum;
		}
		public static bool FilpFlop(bool value)
		{
			return !value;
		}
		public static bool Probability(float fPercent)
		{
			float fProbabilityRate = UnityEngine.Random.value * 100.0f;

			if (fPercent == 100.0f && fProbabilityRate == fPercent) return true;
			else if (fProbabilityRate < fPercent) return true;
			else return false;
		}
	}
}