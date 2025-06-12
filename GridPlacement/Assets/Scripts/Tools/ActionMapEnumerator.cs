#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public class ActionMapEnumerator
{
    private const string FileName = "ActionMaps";
    private const string FileExtension = ".cs";
    private static string previousClassText;
    private static string pathToThis = string.Empty;

    [MenuItem("Tools/Update Action Maps")]
    public static void GenerateActionMaps()
    {
        // Get the InputActionAsset (Change path if needed)
        string inputAssetPath = "Assets/InputSystem_Actions.inputactions"; // Modify this path!
        InputActionAsset inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputAssetPath);

        if (inputAsset == null)
        {
            Debug.LogWarning("InputActionAsset not found at: " + inputAssetPath);
            return;
        }

        // Get action map names
        string[] actionMaps = inputAsset.actionMaps.Select(map => map.name).ToArray();

        // Get the path to this script
        GetPathToThis();

        // Combine the directory path with the file name to get the full file path
        string filePath = Path.Combine(pathToThis, FileName + FileExtension);

        // Generate the class text
        string classText = GenerateEnumClass(actionMaps);

        // Save only if there's a change
        if (previousClassText != classText)
        {
            File.WriteAllText(filePath, classText);
            previousClassText = classText;
            Debug.Log("Updated Enumerated Action Maps");
            AssetDatabase.Refresh(); // Refresh to reflect changes in the Unity Editor
        }
        else
        {
            Debug.Log("No changes detected in Action Maps.");
        }
    }

    private static void GetPathToThis()
    {
        if (string.IsNullOrEmpty(pathToThis))
        {
            string thisName = "ActionMapEnumerator.cs";
            string[] res = Directory.GetFiles(Application.dataPath, thisName, SearchOption.AllDirectories);

            if (res.Length == 0)
            {
                Debug.LogError("Couldn't find path to ActionMapEnumerator");
            }
            else
            {
                pathToThis = res[0].Replace(thisName, "").Replace("\\", "/");
            }
        }
    }

    private static string GenerateEnumClass(string[] actionMaps)
    {
        string enumMembers = string.Join(",\n    ", actionMaps.Select(ClearSpecialCharacters));

        string switchCases = string.Join("\n        ", actionMaps.Select(map =>
            $"case ActionMapType.{ClearSpecialCharacters(map)}: return _inputAsset.FindActionMap(\"{map}\");"));

        return
$@"using UnityEngine.InputSystem;

public enum ActionMapType
{{
    {enumMembers}
}}

public static class ActionMaps
{{
    private static InputActionAsset _inputAsset;

    public static void Initialize(InputActionAsset asset)
    {{
        _inputAsset = asset;
    }}

    public static InputActionMap Get(ActionMapType type)
    {{
        switch (type)
        {{
            {switchCases}
            default: return null;
        }}
    }}
}}";
    }

    private static string ClearSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9]", ""); // Removes special characters
    }
}

#endif
