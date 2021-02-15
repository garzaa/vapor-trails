using UnityEngine;

[CreateAssetMenu]
public class Beacon : ScriptableObject {
    [TextArea] [SerializeField] string description;
    public SceneContainer leftScene;
    public SceneContainer rightScene;
}
