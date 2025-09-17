using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleBuilder
{
    [MenuItem("Tools/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string path = "Assets/AssetBundles";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Build bundles for the current platform
        BuildPipeline.BuildAssetBundles(
            path,
            BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget
        );

        Debug.Log("AssetBundles built to: " + path);
    }
}
