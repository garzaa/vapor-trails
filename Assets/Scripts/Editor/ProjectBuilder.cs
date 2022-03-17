using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class ProjectBuilder {

    static EditorBuildSettingsScene[] enabledScenes;

    public static void BuildAll() {
        enabledScenes = GetEnabledScenes();
	    PlayerSettings.bundleVersion = Git.BuildVersion;
        Build(BuildTarget.WebGL, "webgl");
        Build(BuildTarget.StandaloneWindows64, "win-exe", extension: ".exe");
        Build(BuildTarget.StandaloneWindows, "win32-exe", extension: ".exe");
        Build(BuildTarget.StandaloneOSX, "osx");
        Build(BuildTarget.StandaloneLinux64, "gnu-linux", extension: ".x86");
    }

    public static void BuildWindows() {
        enabledScenes = GetEnabledScenes();
        Build(BuildTarget.StandaloneWindows64, "win-exe", extension: ".exe");
    }

    static void Build(BuildTarget target, string folderSuffix, string extension="") {
        Debug.Log($"Starting build for {folderSuffix}");
        BuildReport report = BuildPipeline.BuildPlayer(enabledScenes, BuildFolder(folderSuffix.ToString(), extension), target, BuildOptions.None);
        if (report.summary.result.Equals(BuildResult.Succeeded)) {
            Debug.Log($"Finished! Build for {folderSuffix} succeeded with size {report.summary.totalSize}");
        } else {
            if (report.summary.result == BuildResult.Succeeded) {
                Debug.Log("Build Success for "+folderSuffix);
            }
            Debug.Log($"Build for {folderSuffix} finished with result: {report.summary.result}");
            Debug.Log($"Total errors: {report.summary.totalErrors}");
        }
    }

    static string BuildFolder(string platform, string extension) {
        return $"../demos/vapor-trails-{platform}/Vapor Trails{extension}";
    }

    static EditorBuildSettingsScene[] GetEnabledScenes() {
        return EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();
    }
}
