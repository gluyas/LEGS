using System.Collections.Generic;
using UnityEngine;

public static class Util
{
	public static T RandomElement<T>(this IList<T> list)
	{
		return list[Random.Range(0, list.Count)];
	}
}
