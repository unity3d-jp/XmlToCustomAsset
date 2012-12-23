using UnityEngine;
using System.Collections;

/*
	ゲームで利用するカスタムデータ型2
	Kyleのアクションで利用
*/
public class MyCustomAssetInnerClass : ScriptableObject {

	public string action;
	
	[Range (0.1f, 10f)]
	public float speed;
}
