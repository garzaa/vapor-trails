using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace UnityEngine {
    [Serializable]
    [CreateAssetMenu(fileName = "New Prefab Tile", menuName = "Tiles/Prefab Tile")]
    public class PrefabTile : TileBase {
        public Sprite sprite;
        public GameObject toSpawn;

        override public void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = this.sprite;
            tileData.gameObject = this.toSpawn;
        }

        override public bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            go.transform.parent = null;
            return true;
        }

    }
}