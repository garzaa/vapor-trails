using UnityEngine;
using System.Collections.Generic;

public class MoveSpeedOverride : MonoBehaviour {
    #pragma warning disable 0649
    [SerializeField]
    List<MovementOverride> editorOverrides;
	#pragma warning restore 0649

    [HideInInspector]
    public Dictionary<string, Vector2> overrides;

    void Start() {
        // copy everything over so we can have a fast dict and editor stuff
        foreach (MovementOverride m in editorOverrides) {
            overrides.Add(m.name, m.speed);
        }
    }
}

[System.Serializable]
public class MovementOverride {
    public string name;
    public Vector2 speed;
}
