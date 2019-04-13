/*
MIT License

Copyright (c) 2019 Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace JCMG.SemVer.Editor
{
	[CustomPropertyDrawer(typeof(SemVersion))]
	public sealed class SemVersionDrawer : PropertyDrawer
	{
		private const string MajorPropertyName = "_major";
		private const string MinorPropertyName = "_minor";
		private const string PatchPropertyName = "_patch";
		private const string PrereleasePropertyName = "_prerelease";
		private const string BuildPropertyName = "_build";

		private const string ReplacementRegex = "[^A-Za-z0-9]+";

		private const string ValidCharactersMessage =
			"The Prerelease and Build fields can only consist of alphanumeric characters (no special " +
			"characters). Any invalid characters will be removed.";

		private const string LayoutGroupStyleName = "box";
		private const string PreviewLabel = "Preview";
		private const string AddLabel = "+";
		private const string SubtractLabel = "-";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUILayout.BeginVertical(LayoutGroupStyleName);
			GUILayout.Label(label, EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(PreviewLabel);
			GUILayout.Label(fieldInfo.GetValue(property.serializedObject.targetObject).ToString());
			EditorGUILayout.EndHorizontal();

			DrawVersionNumber(property.FindPropertyRelative(MajorPropertyName));
			DrawVersionNumber(property.FindPropertyRelative(MinorPropertyName));
			DrawVersionNumber(property.FindPropertyRelative(PatchPropertyName));

			EditorGUILayout.HelpBox(ValidCharactersMessage, MessageType.Info);

			var preReleaseProp = property.FindPropertyRelative(PrereleasePropertyName);
			preReleaseProp.stringValue = Regex.Replace(
				EditorGUILayout.TextField(preReleaseProp.displayName, preReleaseProp.stringValue),
				ReplacementRegex,
				string.Empty);

			var buildProp = property.FindPropertyRelative(BuildPropertyName);
			buildProp.stringValue = Regex.Replace(
				EditorGUILayout.TextField(buildProp.displayName, buildProp.stringValue),
				ReplacementRegex,
				string.Empty);

			EditorGUILayout.EndVertical();
			EditorGUI.EndProperty();
		}

		private void DrawVersionNumber(SerializedProperty property)
		{
			EditorGUILayout.BeginHorizontal();
			property.intValue = Mathf.Max(EditorGUILayout.IntField(
				property.displayName,
				property.intValue), 0);
			if (GUILayout.Button(SubtractLabel, GUILayout.Width(50f)))
			{
				property.intValue = Mathf.Max(property.intValue - 1, 0);
			}
			if (GUILayout.Button(AddLabel, GUILayout.Width(50f)))
			{
				property.intValue = property.intValue + 1;
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
