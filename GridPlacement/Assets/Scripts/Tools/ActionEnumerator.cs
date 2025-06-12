#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionEnumerator
{
    private const string FileName = "ActionEnums";
    private const string FileExtension = ".cs";
    private static string previousClassText;
    private static string pathToThis = string.Empty;

    [MenuItem("Tools/Update Action Enums")]
    public static void GenerateActionEnums()
    {
        string inputAssetPath = "Assets/InputSystem_Actions.inputactions"; // Modify if needed
        InputActionAsset inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputAssetPath);

        if (inputAsset == null)
        {
            Debug.LogWarning("InputActionAsset not found at: " + inputAssetPath);
            return;
        }

        GetPathToThis();
        string filePath = Path.Combine(pathToThis, FileName + FileExtension);
        string classText = GenerateEnumClass(inputAsset);

        if (previousClassText != classText)
        {
            File.WriteAllText(filePath, classText);
            previousClassText = classText;
            Debug.Log("Updated Enumerated Actions");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("No changes detected in Actions.");
        }
    }

    private static void GetPathToThis()
    {
        if (string.IsNullOrEmpty(pathToThis))
        {
            string thisName = "ActionEnumerator.cs";
            string[] res = Directory.GetFiles(Application.dataPath, thisName, SearchOption.AllDirectories);

            if (res.Length == 0)
            {
                Debug.LogError("Couldn't find path to ActionEnumerator");
            }
            else
            {
                pathToThis = res[0].Replace(thisName, "").Replace("\\", "/");
            }
        }
    }

    private static string GenerateEnumClass(InputActionAsset inputAsset)
    {
        var actionMapEnums = inputAsset.actionMaps.Select(map =>
        {
            string mapName = ClearSpecialCharacters(map.name);
            string actionsEnum = string.Join(",\n    ", map.actions.Select(action => ClearSpecialCharacters(action.name)));
            return $"public enum {mapName}Actions\n{{\n    {actionsEnum}\n}}";
        });

        var actionGetters = inputAsset.actionMaps.SelectMany(map => map.actions.Select(action =>
            $"case {ClearSpecialCharacters(map.name)}Actions.{ClearSpecialCharacters(action.name)}: return _inputAsset.FindAction(\"{map.name}/{action.name}\");"));

        return
$@"using UnityEngine.InputSystem;

{string.Join("\n\n", actionMapEnums)}

public static class ActionMapsAction
{{
    private static InputActionAsset _inputAsset;

    public static void Initialize(InputActionAsset asset)
    {{
        _inputAsset = asset;
    }}

    public static InputAction Get<T>(T actionType) where T : System.Enum
    {{
        switch (actionType)
        {{
            {string.Join("\n        ", actionGetters)}
            default: return null;
        }}
    }}
}}";
    }

    private static string ClearSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9]", "");
    }
}

#endif
