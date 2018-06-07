using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(fileName = "Assets/Prefabs/Costumes/New Costume", menuName = "Player Costume", order = 1)]
#endif

public class Costume : ScriptableObject
{
	public String Name;
	
	public GameObject Head;
	public GameObject LegLeft;
	public GameObject LegRight;
}
