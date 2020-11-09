using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ShaderGlobalVars : MonoBehaviour {
    void Start() {
        StartCoroutine(UpdateStepTime());
    }

    void Update() {
        Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
    }

    IEnumerator UpdateStepTime() {
        Shader.SetGlobalFloat("_StepTime", Time.time);
        yield return new WaitForSeconds(1f/16f);
        StartCoroutine(UpdateStepTime());
    }
}