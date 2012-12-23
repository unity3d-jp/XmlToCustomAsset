using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class KyleBehavior : MonoBehaviour {
	
	public GUIText nameLabel;
	public MyCustomAssetInnerClass kyleData;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( null != kyleData ) {
			if( nameLabel != null ) {
				nameLabel.text = kyleData.name;
			}
			
			Animator animator = GetComponent<Animator>();			
			animator.SetBool("dying", kyleData.action == "Dead");
			animator.speed = kyleData.speed;
		}		
	}
}
