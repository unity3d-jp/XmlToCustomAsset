using UnityEngine;
using System.Collections;

//@script ExecuteInEditMode()

[ExecuteInEditMode]
public class GUITextInPlace : MonoBehaviour {

	public Transform playerObject;

	// Update is called once per frame
	void Update () {
		Vector3 parentPos = playerObject.transform.position;
		Camera mainCam = Camera.main;	
		transform.position = mainCam.WorldToViewportPoint(parentPos);	
	}
}
