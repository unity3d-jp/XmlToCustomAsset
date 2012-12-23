using UnityEngine;
using System.Collections;

/*
	ゲームで利用するカスタムデータ型１
	
	オブジェクトがUnityの編集作業にてシリアライズ＆保存されるには
	ScriptableObjectを継承すること
*/
public class MyCustomAsset : ScriptableObject {

	public Color color;
	public string messages;
	public MyCustomAssetInnerClass[] inner;
	public TextAsset source;
}
