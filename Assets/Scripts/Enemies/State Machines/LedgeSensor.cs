using UnityEngine;

[RequireComponent(typeof(WallSensor))]
public class LedgeSensor : Sensor {
    public float maxVertRange = 0.64f;
    public float detectedLedgeDistance;
    float wallDistance;
    Vector2 size;
    int layerMask;

    new void Start() {
        base.Start();
        size = GetComponent<BoxCollider2D>().bounds.size * 0.75f;
        layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
        wallDistance = GetComponent<WallSensor>().distance;
    }

    void LateUpdate() {
        detectedLedgeDistance = 999f;
        if (GetComponent<WallSensor>().nearWall) {
            // cast a box up each square at the wall check and get the
            // nearest empty space
            for (float i=0; i<=maxVertRange; i+=0.64f) {
                Vector2 destination = new Vector2(
                    this.transform.position.x+(wallDistance*e.ForwardScalar()),
                    this.transform.position.y + i
                );
                RaycastHit2D hit = Physics2D.Raycast(
                    destination,
                    Vector2.right * e.ForwardScalar(),
                    0.1f,
                    layerMask
                );
                Debug.DrawLine(destination, destination + Vector2.right * e.ForwardScalar() * 0.1f, Color.red);
                Debug.DrawLine(
                    transform.position,
                    destination,
                    Color.cyan
                );
                if (hit.transform == null) {
                    detectedLedgeDistance = i;
                    break;
                }
                
            }
        }
        animator.SetFloat("LedgeDistance", detectedLedgeDistance);
    }
}
