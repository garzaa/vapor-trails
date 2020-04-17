using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameState", order = 1)]
public class GameState : ScriptableObject {
    public string stateName;
    [TextArea] public string description;
}