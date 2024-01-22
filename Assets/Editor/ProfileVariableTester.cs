using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

// *1 Evaluate values at runtime - curly brace notated values
using UnityEngine.AddressableAssets.Initialization;

class ProfileVariables
{
	public int count;
	public List<string> names;
	public List<string> values;
	public List<string> editorValues;

	public List<string> runtimeValues; // *1

	public ProfileVariables(List<string> names)
	{
		this.count = names.Count;
		this.names = names;
		values = new List<string>(count);
		editorValues = new List<string>(count);
		runtimeValues = new List<string>(count); // *1
	}

	public override string ToString()
	{
		string message = string.Empty;
		for (int i = 0; i < count; i++)
		{
			message += $"Names i: {names[i]}{i} \n " +
				$"*Initial value = '{values[i]}' \n " +
				$"**editor values:       '{editorValues[i]}' \n " +
				$"***Runtime values: '{runtimeValues[i]}'\n";
		}

		return message;
	}
}

public class ProfileVariableTester
{
	[MenuItem("LoadyProfiles/Test Profile Variable")]
	private static void TestProfileVariable()
	{
		AddressableAssetSettings addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
		AddressableAssetProfileSettings profileSettings = addressableAssetSettings.profileSettings;
		string activeProfileID = addressableAssetSettings.activeProfileId;

		var variables = new ProfileVariables(profileSettings.GetVariableNames());

		for (int i = 0; i < variables.count; i++)
		{
			string variableName = variables.names[i];
			var value = profileSettings.GetValueByName(activeProfileID, variableName);
			variables.values.Add(value);

			var editorValue = profileSettings.EvaluateString(activeProfileID, value);
			variables.editorValues.Add(editorValue);

			var runtimeValue = AddressablesRuntimeProperties.EvaluateString(editorValue);
			variables.runtimeValues.Add(runtimeValue); // *1
		}
		Debug.Log(variables);
	}
}