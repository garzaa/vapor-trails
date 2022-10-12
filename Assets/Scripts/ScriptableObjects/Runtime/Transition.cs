using UnityEngine;

[CreateAssetMenu(fileName = "Transition", menuName = "Runtime/Transition")]
public class Transition : ScriptableObject {
    public bool subway;
    public Beacon beacon;
    public Vector2 position;
    public GameCheckpoint checkpointToImport = null;

    public void Clear() {
        subway = false;
        beacon = null;
        position = Vector2.zero;
        checkpointToImport = null;
    }

    public bool IsEmpty() {
        return (subway == false) && (beacon == null) && (position.Equals(Vector2.zero)) && (checkpointToImport == null);
    }
}
