using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
#endif

public class CustomMenu
{
#if UNITY_EDITOR
    [MenuItem("Scenes/TitleScene")]
    static void OpenTitleScene() { OpenScene("TitleScene"); }
    [MenuItem("Scenes/LoadingScene")]
    static void OpenLoadingScene() { OpenScene("LoadingScene"); }
    [MenuItem("Scenes/InGameScene")]
    static void OpenInGameScene() { OpenScene("InGameScene"); }
    [MenuItem("Scenes/RaidScene")]
    static void OpenRaidScene() { OpenScene("RaidScene"); }
    [MenuItem("Scenes/WorldServerScene")]
    static void OpenWorldServerScene() { OpenScene("WorldServerScene"); }
    //[MenuItem("Scenes/TestScene")]
    //static void OpenTestScene() { OpenScene("TestScene"); }

    static void OpenScene(string sceneName)
    {
        string scenePath = "Assets/Scenes/";
        scenePath += sceneName;
        scenePath += ".unity";
        EditorSceneManager.OpenScene(scenePath);
    }

    // 뒤끝 월드 서버 업로드용 빌드. 서버는 클라이언트 씬(Title 등) 없이 WorldServerScene으로 시작한다.
    // zip 이름과 실행 파일 이름이 같아야 하고, 실행 파일은 zip 최상위에 있어야 한다.
    [MenuItem("Build/World Server (Linux)")]
    static void BuildWorldServer()
    {
        const string name = "NightmareRaidServer";
        string dir = $"Builds/WorldServer/{name}";
        if (Directory.Exists(dir)) Directory.Delete(dir, true);
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/WorldServerScene.unity", "Assets/Scenes/RaidScene.unity" },
            locationPathName = $"{dir}/{name}.x86_64",
            target = BuildTarget.StandaloneLinux64,
            targetGroup = BuildTargetGroup.Standalone,
            subtarget = (int)StandaloneBuildSubtarget.Server,
        });
        if (report.summary.result != BuildResult.Succeeded) return;
        string zip = $"Builds/WorldServer/{name}.zip";
        File.Delete(zip);
        ZipFile.CreateFromDirectory(dir, zip);
        EditorUtility.RevealInFinder(zip);
    }
#endif
}
