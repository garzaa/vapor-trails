using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxOnVisible : ParallaxLayer {
    Renderer r;

    public override void ExtendedStart() {
        r = GetComponent<Renderer>();
        parallaxEnabled = false;
    }

    public override void ExtendedUpdate() {
        if (r.IsVisibleFrom(Camera.main)) {
            print("ENABLED");
            this.parallaxEnabled = true;
        } else {
            this.parallaxEnabled = false;
            print("DISABLED");
        }
    }
}
