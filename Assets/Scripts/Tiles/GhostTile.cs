using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GhostTile : RuleTile {
	public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject instantiatedGameObject) {
        Debug.Log("setting location "+location+" to null");
        base.StartUp(location, tilemap, instantiatedGameObject);

        if (!Application.isPlaying) {
            return true;
        }

        Tilemap parentTilemap = tilemap.GetComponent<Tilemap>();

        if (instantiatedGameObject != null) {
            Renderer parentRenderer = parentTilemap.GetComponent<TilemapRenderer>();

            GameObject g = Instantiate(instantiatedGameObject, instantiatedGameObject.transform.position, instantiatedGameObject.transform.rotation, parentTilemap.transform);
            foreach (Renderer childRenderer in g.GetComponentsInChildren<Renderer>()) {
                childRenderer.sortingLayerID = parentRenderer.sortingLayerID;
                childRenderer.sortingOrder = parentRenderer.sortingOrder;
            }
        }

        parentTilemap.SetTile(location, null);

        return true;
	}
}
