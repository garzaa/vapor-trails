using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerGroundCheck : MonoBehaviour {
    public GroundData groundData = new GroundData();
    
    public BoxCollider2D playerCollider;

    int defaultLayerMask;

    RaycastHit2D leftGrounded;
    RaycastHit2D rightGrounded;
    bool grounded;
    bool onLedge;
    Vector2 currentNormal = Vector2.up;
    GameObject currentGround;

    List<RaycastHit2D>platforms = new List<RaycastHit2D>();

    void Awake() {
        defaultLayerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
    }

    void Update() {
        RefreshGroundData(groundData);

        leftGrounded = LeftGrounded();
        rightGrounded = RightGrounded();
        grounded = leftGrounded || rightGrounded;
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

        if (grounded) {
            groundData.groundObject = (leftGrounded ? leftGrounded : rightGrounded).collider.gameObject;
        }
    }

    List<RaycastHit2D> TouchingPlatforms() {
        platforms.Clear();

        platforms.AddRange(GetPlatforms(playerCollider.BottomLeftCorner()));
        platforms.AddRange(GetPlatforms(playerCollider.BottomRightCorner()));

        if (platforms.Count == 0) {
            return null;
        }

        return platforms;
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
        return DefaultLinecast(playerCollider.BottomLeftCorner());
    }

    RaycastHit2D RightGrounded() {
        return DefaultLinecast(playerCollider.BottomRightCorner());
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
            Debug.DrawLine(start, hit.point);
            return hit.normal;
        } else {
            Debug.DrawLine(start, end);
            return Vector2.up;
        }
    }

    RaycastHit2D DefaultLinecast(Vector2 origin) {
        Vector2 start = origin + Vector2.up * 0.05f;
        Vector2 end = origin + (-currentNormal * 0.10f);

        Debug.DrawLine(start, end);
        return Physics2D.Linecast(
            start,
            end,
            defaultLayerMask
        );
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
