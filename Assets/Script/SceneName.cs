using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SceneName : MonoBehaviour {
	
	public MyCustomAsset myData;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(null != myData) {
			guiText.text = myData.messages;
			if( Application.isPlaying ) {
				guiText.material.color =  myData.color;	
			}
		}
	}
}
