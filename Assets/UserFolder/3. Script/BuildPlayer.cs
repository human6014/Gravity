using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//using UnityEditor.Build.Reporting;

public class BuildPlayer : MonoBehaviour
{

    //[MenuItem("Build/Build AOS")]
    //public static void MyBuild_AOS()
    //{
    //    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    //    buildPlayerOptions.scenes = new[] { "Assets/UserFolder/Scenes/GameScene.unity" };
    //    buildPlayerOptions.locationPathName = $"Builds/AOS_{PlayerSettings.bundleVersion}.apk";
    //    buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
    //    buildPlayerOptions.options = BuildOptions.None;

    //    BuildSummary summary = BuildPipeline.BuildPlayer(buildPlayerOptions).summary;

    //    if (summary.result == BuildResult.Succeeded) Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
    //    else if (summary.result == BuildResult.Failed) Debug.Log("Build failed");
    //}

}
