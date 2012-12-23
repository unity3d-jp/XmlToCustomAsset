using UnityEngine;
using UnityEditor;
using System.Collections;

/*
	アセットのインポートや削除を監視して
	自動的に処理をするポストプロセッサー
*/
public class MyXMLPostprocessor : AssetPostprocessor {

    static void OnPostprocessAllAssets (
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths) 
    {
        foreach (string str in importedAssets) {
        	if(str.LastIndexOf(".xml" ) != -1) {
        		Debug.Log("Reimporting:"+str);
        		XMLToCustomAsset.CreateCustomAssetFromXml(str);
        	}
        }

        foreach (string str in deletedAssets) {
        	if(str.LastIndexOf(".xml" ) != -1) {
        		Debug.Log("Deleting:"+str);
        		string customAssetPath = XMLToCustomAsset.GetCustomAssetPathFromXml(str);
        		AssetDatabase.DeleteAsset(customAssetPath);
        	}
        }
    }
}
