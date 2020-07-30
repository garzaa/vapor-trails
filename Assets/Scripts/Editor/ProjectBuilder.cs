using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ProjectBuilder {

    public static void BuildAll() {
        BuildWebGL();
    }

    static void BuildWebGL() {
        BuildPipeline.BuildPlayer(GetEnabledScenes(), "Bin/webgl/", BuildTarget.WebGL, BuildOptions.None);
    }

    static EditorBuildSettingsScene[] GetEnabledScenes() {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if(scene.enabled)
                scenes.Add(scene);
        }
        return scenes.ToArray();
    }
}