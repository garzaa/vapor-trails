using UnityEngine;

[CreateAssetMenu]
public class Beacon : ScriptableObject {
    [TextArea] [SerializeField] string description;
    public SceneContainer left;
    public SceneContainer right;
}
