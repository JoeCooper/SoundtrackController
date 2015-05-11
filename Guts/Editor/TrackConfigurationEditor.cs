using UnityEngine;
using UnityEditor;
using System.IO;

namespace NobleMuffins.SoundtrackPlus {
	public class TrackConfigurationEditor : Editor {
		
		[MenuItem("Assets/Create/Music Track Configuration")]
		public static void CreateInstance()
		{
			CreateAsset<MusicTrackConfiguration>("Music Track");
		}
		
		public static void CreateAsset<T> (string typeName = null) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T> ();
			
			string path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "") 
			{
				path = "Assets";
			} 
			else if (Path.GetExtension (path) != "") 
			{
				path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}
			
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + (typeName ?? typeof(T).ToString()) + ".asset");
			
			AssetDatabase.CreateAsset (asset, assetPathAndName);
			
			AssetDatabase.SaveAssets ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
		}
	}
}