#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class TagsLayersEnumBuilder : EditorWindow {
	[MenuItem("Edit/Rebuild Tags And Layers Enums")]
	static void RebuildTagsAndLayersEnums() {
		var enumsPath = Application.dataPath + "/Scripts/Enums/";

		if (!Directory.Exists(enumsPath))
		{
			Directory.CreateDirectory(enumsPath);
		}

		rebuildTagsFile(enumsPath + "Tags.cs");
		rebuildLayersFile(enumsPath + "Layers.cs");

		AssetDatabase.ImportAsset(enumsPath + "Tags.cs", ImportAssetOptions.ForceUpdate);
		AssetDatabase.ImportAsset(enumsPath + "Layers.cs", ImportAssetOptions.ForceUpdate);
	}

	static void rebuildTagsFile(string filePath) {
		StringBuilder sb = new StringBuilder();

		sb.Append("//This class is auto-generated, do not modify (TagsLayersEnumBuilder.cs)\n");
		sb.Append("public abstract class Tags {\n");

		var srcArr = UnityEditorInternal.InternalEditorUtility.tags;
		var tags = new String[srcArr.Length];
		Array.Copy(srcArr, tags, tags.Length);
		Array.Sort(tags, StringComparer.InvariantCultureIgnoreCase);

		for (int i = 0, n = tags.Length; i < n; ++i) {
			string tagName = tags[i];

			sb.Append("\tpublic const string " + tagName + " = \"" + tagName + "\";\n");
		}

		sb.Append("}\n");

#if !UNITY_WEBPLAYER
		File.WriteAllText(filePath, sb.ToString());
#endif
	}

	static void rebuildLayersFile(string filePath) {
		StringBuilder sb = new StringBuilder();
		
		sb.Append("//This class is auto-generated, do not modify (use Tools/TagsLayersEnumBuilder)\n");
		sb.Append("public abstract class Layers {\n");
		
		var layers = UnityEditorInternal.InternalEditorUtility.layers;

		for (int i = 0, n = layers.Length; i < n; ++i) {
			string layerName = layers[i];
			
			sb.Append("\tpublic const string " + GetVariableName(layerName) + " = \"" + layerName + "\";\n");
		}
		
		sb.Append("\n");
		
		for (int i = 0, n = layers.Length; i < n; ++i) {
			string layerName = layers[i];
			int layerNumber = LayerMask.NameToLayer(layerName);
			string layerMask = layerNumber == 0 ? "1" : ("1 << " + layerNumber);
			
			sb.Append("\tpublic const int " + GetVariableName(layerName) + "Mask" + " = " + layerMask + ";\n");
		}

		sb.Append("\n");
		
		for (int i = 0, n = layers.Length; i < n; ++i) {
			string layerName = layers[i];
			int layerNumber = LayerMask.NameToLayer(layerName);
			
			sb.Append("\tpublic const int " + GetVariableName(layerName) + "Number" + " = " + layerNumber + ";\n");
		}
		
		sb.Append("}\n");

#if !UNITY_WEBPLAYER
		File.WriteAllText(filePath, sb.ToString());
#endif
	}

	private static string GetVariableName(string str) {
		return str.Replace(" ", "");
	}
}
#endif