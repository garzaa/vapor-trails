using UnityEngine;

[CreateAssetMenu(fileName = "Game State", menuName = "Data Containers/Game State")]
public class GameState : ScriptableObject {
    public bool writeImmediately;
    [TextArea] public string description;
}
