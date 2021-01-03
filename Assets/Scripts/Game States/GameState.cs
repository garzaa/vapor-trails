using UnityEngine;

[CreateAssetMenu()]
public class GameState : ScriptableObject {
    public bool writeImmediately;
    [TextArea] public string description;
}