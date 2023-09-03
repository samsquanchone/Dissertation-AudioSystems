using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExportPackage
{
    [MenuItem("Export/Export with tags and layers, Input settings")]
    public static void export()
    {
        string[] projectContent = new string[] { "Assets", "ProjectSettings/TagManager.asset", "ProjectSettings/InputManager.asset", "ProjectSettings/ProjectSettings.asset" };
        AssetDatabase.ExportPackage(projectContent, "AudioSystemsPackage.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        Debug.Log("Project Exported");
    }
}
