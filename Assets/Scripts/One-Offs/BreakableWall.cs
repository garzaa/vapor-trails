using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

[RequireComponent(typeof(PersistentEnabled))]
public class BreakableWall : MonoBehaviour {
    public GameObject breakPrefab;
    public string attackName;
    public AudioClip burstNoise;
    public bool attackLandEvent = true;
    
    Tilemap tilemap;

    void Start() {
        tilemap = GetComponent<Tilemap>();
    }

    public void OnCollisionEnter2D(Collision2D col) {
        Collider2D other = col.collider;
        if (other.tag.Equals(Tags.PlayerHitbox)) {
            if (string.IsNullOrEmpty(attackName) || attackName.Equals(other.GetComponent<PlayerAttack>().attackName)) {
                PlayerAttack a = other.GetComponent<PlayerAttack>();
                GlobalController.pc.OnAttackLand(a);
                if (attackLandEvent) {
                    GlobalController.pc.GetComponent<Animator>().SetTrigger("AttackLand");
                    a.SelfKnockBack();
                    a.MakeHitmarker(a.transform);
                }
                Burst();
            }
        }
    }

    void Burst() {
        GetComponent<TilemapRenderer>().enabled = false;
        GetComponent<TilemapCollider2D>().enabled = false;
        SoundManager.WorldSound(burstNoise);
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles =  tilemap.GetTilesBlock(bounds);


        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null) {
                    Instantiate(breakPrefab, new Vector3(x*0.64f, y*0.64f, 0), Quaternion.identity);
                    
                }
            }
        }
        StartCoroutine(WaitAndDisable());   
    }

    IEnumerator WaitAndDisable() {
        yield return new WaitForSecondsRealtime(10);
        gameObject.SetActive(false);
    }
}