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
public class ActorUnit
{
	public ActorUnit(GameObject prefab)
	{
		this.prefab = prefab;
		param = prefab.GetComponent<ActorBase>().Param;
	}
	public ActorParam param;
	public GameObject prefab;
}
public class RequestUnit
{
	public RequestUnit(ActorUnit actorUnit, CVSUnitRequest request, GameObject requestGrid)
	{
		this.actorUnit = actorUnit;
		this.request = request;
		this.requestGrid = requestGrid;
	}
	public ActorUnit actorUnit;
	public CVSUnitRequest request;
	public GameObject requestGrid;
}

namespace InstanceManager
{ 
	public class StatuePool
	{
		public StatuePool(ActorBase actor, GameObject obj, Color requestColor)
		{
			this.actor = actor;
			this.obj = obj;
			obj.GetComponent<Image>().color = requestColor;
		}
		public ActorBase actor;
		public GameObject obj;
	}
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