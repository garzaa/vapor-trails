using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Scene Container", menuName = "Runtime/Game Checkpoint")]
public class GameCheckpoint : ScriptableObject {
    public List<Item> items;
    public List<GameState> states;
}
