using UnityEngine;

[CreateAssetMenu(fileName = "Beacon", menuName = "Data Containers/Beacon")]
public class Beacon : ScriptableObject {
    [TextArea] [SerializeField] string description;
    public SceneReference leftScene;
    public SceneReference rightScene;
}
