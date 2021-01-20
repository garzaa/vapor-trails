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
    Vector2 currentNormal;
    GameObject currentGround;

    List<RaycastHit2D>platforms = new List<RaycastHit2D>();

    void Awake() {
        defaultLayerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
    }

    void Update() {
        RefreshGroundData(groundData);

        leftGrounded = LeftGrounded();
        rightGrounded = RightGrounded();
        grounded = leftGrounded && rightGrounded;
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
        if (grounded) {
            groundData.normal = (leftGrounded ? leftGrounded : rightGrounded).normal;
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
            0.32f,
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

    RaycastHit2D DefaultLinecast(Vector2 origin) {
        Vector2 start = origin + Vector2.up * 0.05f;
        Vector2 end = origin + Vector2.down * 0.05f;

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
    public GameObject groundObject;
    public List<RaycastHit2D> platforms;
}
