using UnityEngine;
using UnityEditor;
using System.Collections;

/*
	MyCustomAssetEditorをインスペクターの表示をカスタマイズする
	エディター拡張クラス
*/
[CustomEditor(typeof(MyCustomAsset))]
public class MyCustomAssetEditor : Editor {

    private MyCustomAsset _customAsset;

    public MyCustomAsset customAsset {
        get {
            if (_customAsset == null)
            {
                _customAsset = target as MyCustomAsset;
            }
            return _customAsset;
        }
    }

    public override void OnInspectorGUI()
    {
    	serializedObject.Update ();
    	DrawDefaultInspector();

        GUILayout.Space(5f);
		if(GUILayout.Button("Custom!")) {
			Debug.Log("Custom Operation!!");
//				if(XMLToCustomAsset.ReloadMyCustomAssetFromText(customAsset)) {
//					EditorUtility.SetDirty(customAsset);
//				}
		}
    	serializedObject.ApplyModifiedProperties ();
    }
    
}
