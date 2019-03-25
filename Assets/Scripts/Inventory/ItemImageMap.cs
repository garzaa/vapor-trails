using UnityEngine;

public struct ItemImageMap {
    public Sprite itemThumbnail;
    public Sprite itemDetail;

    public ItemImageMap(Sprite t, Sprite d) {
        this.itemThumbnail = t;
        this.itemDetail = d;
    }
}