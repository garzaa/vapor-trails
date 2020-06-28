using UnityEngine;
using System.Collections.Generic;

public class EditorGameStates : MonoBehaviour {
    public List<GameState> states;

    void Start() {
        foreach (GameState s in states) {
           GlobalController.AddState(s);
        } 
    }
}