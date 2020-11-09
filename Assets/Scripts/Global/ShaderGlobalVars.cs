using UnityEngine;

[ExecuteInEditMode]
public class ShaderGlobalVars : MonoBehaviour {
    void Update() {
        Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
    }
}