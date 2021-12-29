using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GroundCheck : MonoBehaviour {
    public GroundData groundData = new GroundData();
    [SerializeField] bool detecting = true;
    
    Collider2D col;

    int defaultLayerMask;

    RaycastHit2D leftGrounded;
    RaycastHit2D rightGrounded;
    Collider2D groundCollider;
    bool grounded;
    bool onLedge;
    Vector2 currentNormal = Vector2.up;
    Vector2 bottomCenter;
    Vector2 overlapBoxSize;
    GameObject currentGround;

    List<RaycastHit2D>platforms = new List<RaycastHit2D>();
    List<RaycastHit2D> nonPlatforms = new List<RaycastHit2D>();

    void Start() {
        col = GetComponent<Collider2D>();
        defaultLayerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
        overlapBoxSize = new Vector2();
        // 1 pixel down from the bottom of the player collider
        overlapBoxSize.y = 0.04f;
    }

    void Update() {
        RefreshGroundData(groundData);

        leftGrounded = LeftGrounded();
        rightGrounded = RightGrounded();
        groundCollider = GetGroundCollider();
        grounded = detecting && (groundCollider != null);
        onLedge = leftGrounded ^ rightGrounded;

        if (groundData.grounded && !grounded) {
            groundData.leftGround = true;
        } else if (!groundData.grounded && grounded) {
            groundData.hitGround = true;
        }

        if (!groundData.onLedge && onLedge) {
            groundData.ledgeStep = true;
        }

        groundData.platforms = TouchingPlatforms();

        groundData.grounded = grounded;
        groundData.onLedge = onLedge;

        currentNormal = GetGroundNormal();
        groundData.normal = currentNormal;
        groundData.normalRotation = Vector2.SignedAngle(Vector2.up, currentNormal);

        if (groundCollider != null) {
            groundData.groundObject = groundCollider.gameObject;
        }
    }

    List<RaycastHit2D> TouchingPlatforms() {
        platforms.Clear();

        platforms.AddRange(GetPlatforms(col.BottomLeftCorner()));
        platforms.AddRange(GetPlatforms(col.BottomRightCorner()));

        nonPlatforms.Clear();

        for (int i=0; i<platforms.Count; i++) {
            if (!platforms[i].collider.CompareTag(Tags.Platform)) {
                nonPlatforms.Add(platforms[i]);
            }
        }

        for (int i=0; i<nonPlatforms.Count; i++) {
            platforms.Remove(nonPlatforms[i]);
        }

        return platforms;
    }

    Collider2D GetGroundCollider() {
        // this can change based on animation state, so recompute it here to be safe
        overlapBoxSize.x = col.bounds.size.x * 0.95f;

        // get bottom center of box collider
        bottomCenter = (Vector2) col.bounds.center + (Vector2.down * col.bounds.extents.y);

        Collider2D hit = Physics2D.OverlapBox(
            bottomCenter,
            overlapBoxSize,
            0,
            defaultLayerMask
        );

        return hit;
    }

    RaycastHit2D[] GetPlatforms(Vector2 corner) {
        return Physics2D.CircleCastAll(
            corner,
            1.28f,
            Vector2.zero,
            0f,
            defaultLayerMask
        );
    }

    RaycastHit2D LeftGrounded() {
        return DefaultLinecast(col.BottomLeftCorner());
    }

    RaycastHit2D RightGrounded() {
        return DefaultLinecast(col.BottomRightCorner());
    }

    void RefreshGroundData(GroundData groundData) {
        groundData.leftGround = false;
        groundData.hitGround = false;
        groundData.ledgeStep = false;
    }

    Vector2 GetGroundNormal() {
        Vector2 start = transform.position;
        Vector2 end = (Vector2) transform.position + Vector2.down*0.5f;

        RaycastHit2D hit = Physics2D.Linecast(
            start,
            end,
            defaultLayerMask
        );

        if (hit.transform != null) {
            Debug.DrawLine(start, hit.point, Color.red);
            return hit.normal;
        } else {
            Debug.DrawLine(start, end, Color.green);
            return Vector2.up;
        }
    }

    RaycastHit2D DefaultLinecast(Vector2 origin) {
        Vector2 start = origin + Vector2.up * 0.05f;
        Vector2 end = origin + (-currentNormal * 0.1f);

        Debug.DrawLine(start, end, Color.red);
        return Physics2D.Linecast(
            start,
            end,
            defaultLayerMask
        );
    }

    public void DisableFor(float seconds) {
        StartCoroutine(WaitAndEnable(seconds));
    }

    IEnumerator WaitAndEnable(float seconds) {
        detecting = false;
        yield return new WaitForSeconds(seconds);
        detecting = true;
    }
}

[System.Serializable]
public class GroundData {
    public bool grounded;
    public bool onLedge;
    public bool leftGround;
    public bool hitGround;
    public bool ledgeStep;
    public Vector2 normal;
    public float normalRotation;
    public GameObject groundObject;
    public List<RaycastHit2D> platforms;
}
