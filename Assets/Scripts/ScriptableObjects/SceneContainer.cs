using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

[CreateAssetMenu(fileName = "Scene Container", menuName = "Data Containers/Scene Container")]
public class SceneContainer : ScriptableObject {
    public SceneField scene;
    public string nameOverride;
}
