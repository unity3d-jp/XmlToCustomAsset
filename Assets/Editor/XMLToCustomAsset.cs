/****************************************************************/
/*!
 @discussion ￼
 	XMLを読んでカスタムアセットを生成/更新するサンプル
 */
/****************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

/*
	XMLファイルからカスタムアセットを作成する
	エディターユーティリティ
	
	MonoBehaviourを継承しているのは DestroyImmediate() などを
	利用するため
*/
public class XMLToCustomAsset : MonoBehaviour {

	/*
		指定のアセットパスからカスタムアセットを作成する
		自動（再）インポート用
	
		シーン上のゲームオブジェクトが持っている参照が
		再インポートで切れないように、カスタムアセットがすでにある場合
		既存のオブジェクトをアップデートする
	*/
	public static void CreateCustomAssetFromXml(string path)
	{
		// パスがxmlファイルでなければ何もしない
		if( path.LastIndexOf(".xml" ) < 0) {
			Debug.Log("Asset is not XML (by name):"+path);
			return;
		}

		// パスのxmlファイルをTextAssetとして読み込む
		Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));

		// TextAssetにキャスト
		TextAsset ta = obj as TextAsset;
		if( ta == null ) {
			Debug.Log("Loading XML file failed:"+path);
			return;
		}
		
		// カスタム用のアセットのパスを作成
		// 今回カスタムアセットはXMLファイルと同じディレクトリに、拡張子を変えて保存するので
		// そのためのパスを作成
		string save_path = GetCustomAssetPathFromXml(path);

		MyCustomAsset a = null;
		
		// そのパスで一回読んでみる
		Object objCustom = AssetDatabase.LoadAssetAtPath(save_path, typeof(MyCustomAsset));
		a = objCustom as MyCustomAsset;
		
		// 読み込みに成功したら、すでに過去に保存したカスタムアセットがあるということなので
		// XMLの内容でカスタムアセットを更新する
		if(null != a) {
			a.source = ta;
			ReloadMyCustomAssetFromText(a);
			Debug.Log("objCustom reloaded.");
		} 
		
		// そうでなければ、新しくカスタムアセットを作成する
		else {
			a = CreateMyCustomAssetFromText(ta);
			if(null != a) {
				// カスタムアセットを保存する新しいアセットファイルをパスに作成し、a を保存
				AssetDatabase.CreateAsset( a, save_path );
			}
		}
		
		// MyCustomAsset のインスタンス以外にもオブジェクトが存在する場合
		// 全てのオブジェクトを保存する必要があるので、AddObjectToAssetする
		if(null != a.inner) {
			foreach(MyCustomAssetInnerClass ic in a.inner) {
				if( !AssetDatabase.Contains(ic) ) {
					AssetDatabase.AddObjectToAsset(ic, save_path);
				}
			}
		}			
		
		// すべてのアセットファイルへの変更をディスクに保存する
		AssetDatabase.SaveAssets();
	}
	
	/*
		XMLファイルのパスから対応するMyCustomAssetのアセットパスを返す
		
		NOTE: hoge.xml を渡すと hoge.custom.asset を返す
	*/
	public static string GetCustomAssetPathFromXml(string xmlPath) {
		if(xmlPath.LastIndexOf(".xml" )<0) {
			return null;
		}
		return xmlPath.Substring( 0, xmlPath.LastIndexOf(".xml" )) + ".custom.asset";
	}


	/*
		XMLのテキストアセットからMyCustomAssetを作成する
	*/
    public static MyCustomAsset CreateMyCustomAssetFromText(TextAsset ta) {
    	MyCustomAsset newAsset = ScriptableObject.CreateInstance<MyCustomAsset>();
    	newAsset.source = ta;
    	
    	if(ReloadMyCustomAssetFromText(newAsset)) {
    		return newAsset;
    	}
    	
    	return null;
    }

	/*
		<hoge>text</hoge> のようなテキストのみを返すノードかどうかを返す
	*/
	private static bool _IsTextNode(XmlNode node) {
		return (node.HasChildNodes == true && node.ChildNodes.Count == 1 && node.ChildNodes[0].NodeType.ToString() == "Text");
	}

	/*
		配列(Array)の中の要素をnameで検索するPredicateを返す関数
	*/
	private static System.Predicate<MyCustomAssetInnerClass> _NameEqual(string nameToMatch) 
	{
		return delegate(MyCustomAssetInnerClass obj)
		{
			return obj.name == nameToMatch;
		};
	}

	/*
		配列(Array)の中の要素をオブジェクトで検索するPredicateを返す関数
	*/
	private static System.Predicate<MyCustomAssetInnerClass> _ObjectEqual(MyCustomAssetInnerClass rhs) 
	{
		return delegate(MyCustomAssetInnerClass obj)
		{
			return obj == rhs;
		};
	}


	/*
		文字列をColorに変換

		reference:
		http://answers.unity3d.com/questions/154867/parsing-a-string-to-a-color.html
	*/
	private static Color _ParseColor (string col) {
	   //Takes strings formatted with numbers and no spaces before or after the commas:
		// "1.0,1.0,.35,1.0"
	   string[] strings = col.Split(","[0] );

	   Color output = new Color();
	   for (var i = 0; i < 4; i++) {
		  output[i] = System.Single.Parse(strings[i]);
	   }
	   return output;
	}

	/*
		XMLをパースしてカスタムアセットを更新する
		
		a.source に設定されているxmlデータを読んで、渡されたMyCustomAssetの
		内容を更新します。
	*/
    public static bool ReloadMyCustomAssetFromText(MyCustomAsset a) {
		
		if(null == a) {
			return false;
		}
		if(null == a.source) {
			return false;
		}

		string nameStr = null;
		Color newColor = Color.gray;
		string messagesStr = null;
		List<MyCustomAssetInnerClass> innerObjArray = new List<MyCustomAssetInnerClass>();
		
		try {
			//
			// System.Xml.XmlDocument を使用してXMLをパース
			//
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(new StringReader(a.source.text));
			XmlElement elem = xmlDocument.DocumentElement;
			
			if (elem.HasChildNodes) {
				XmlNode childNode = elem.FirstChild;
			
				while (childNode != null) {
					if(childNode.Name == "name" && _IsTextNode(childNode)) {
						nameStr = childNode.ChildNodes[0].Value;
					}
					if(childNode.Name == "color" && _IsTextNode(childNode)) {
						newColor = _ParseColor(childNode.ChildNodes[0].Value);
					}
					
					else if(childNode.Name == "message" && _IsTextNode(childNode)) {
						messagesStr = childNode.ChildNodes[0].Value;
					}

					else if(childNode.Name == "inner" && childNode.HasChildNodes == true) {
						foreach (XmlNode n in childNode.ChildNodes) {
							// entryタグ以外は無視
							if(n.Name != "entry") {
								continue;
							}
							// 子ノードを持っていない場合も無視
							if(!n.HasChildNodes) {
								continue;
							}
							
							float speed = 1.0f;
							string text = "";
							string name = "";
							
							foreach (XmlNode cn in n.ChildNodes) {
								if(cn.Name == "speed" && _IsTextNode(cn)) {
									try {
										speed = float.Parse(cn.ChildNodes[0].Value);
									} catch (System.Exception exc) {
										Debug.Log(  "Float parse error: " + exc.Message);
									}
								}
								if(cn.Name == "action" && _IsTextNode(cn)) {
									text = cn.ChildNodes[0].Value;
								}
								if(cn.Name == "name" && _IsTextNode(cn)) {
									name = cn.ChildNodes[0].Value;
								}
							}
							
							// 既存のアセットの中に対応するデータがあるかどうかを検索
							MyCustomAssetInnerClass ic = null;
							if(a.inner != null) {
								ic = System.Array.Find(a.inner, _NameEqual(name));
							}

							// なかったら新しく作成
							if(ic == null) {
								ic = ScriptableObject.CreateInstance<MyCustomAssetInnerClass>();
							}
							ic.speed  = speed;
							ic.action = text;
							ic.name   = name;
							innerObjArray.Add(ic);
						}
					}
					childNode = childNode.NextSibling;
				}
			}
		}
		catch (System.Exception exc) {
			Debug.Log(  exc.GetType() +  ": " + exc.Message);
			return false;
		}

		// saving
		a.color = newColor;
		
		if(null != nameStr) {
			a.name = nameStr;
		}

		if(null != messagesStr) {
 			a.messages = messagesStr;
		}

		// 古い要素がある場合、新しいオブジェクトリストに
		// 入っていないものはXML上で削除されているので、アセットからも削除
		if(null != a.inner) {
			foreach(MyCustomAssetInnerClass ic in a.inner) {
				MyCustomAssetInnerClass obj = innerObjArray.Find(_ObjectEqual(ic));
				if(null == obj) {
					DestroyImmediate(ic, true);//アセットから削除
				}
			}
		} 
		// そうでなければ、ロードしたオブジェクトを短に割当て
		a.inner = new MyCustomAssetInnerClass[innerObjArray.Count];
		innerObjArray.CopyTo(a.inner);
		
		return true;
    }
}