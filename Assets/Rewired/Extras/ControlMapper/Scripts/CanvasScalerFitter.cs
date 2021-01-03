// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    /// <summary>
    /// Adjusts the scale based on the current screen aspect ratio to try to fit the content sensibly.
    /// Uses break points to determine current scale settings.
    /// </summary>
    [RequireComponent(typeof(CanvasScalerExt))]
    public class CanvasScalerFitter : MonoBehaviour {

        [SerializeField]
        private BreakPoint[] breakPoints;

        private CanvasScalerExt canvasScaler;
        private int screenWidth;
        private int screenHeight;
        private System.Action ScreenSizeChanged;

        void OnEnable() {
            canvasScaler = GetComponent<CanvasScalerExt>();
            Update(); // update immediately
            canvasScaler.ForceRefresh(); // force the canvas scaler to update now to avoid a flash at the wrong size when first enabled
        }

        void Update() {
            // Check for screen size change
            if(Screen.width != screenWidth || Screen.height != screenHeight) { // screen size changed
                screenWidth = Screen.width;
                screenHeight = Screen.height;
                UpdateSize();
            }
        }

        private void UpdateSize() {
            if(canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize) return;
            if(breakPoints == null) return;

            float xRatio = (float)Screen.width / (float)Screen.height;

            float closest = Mathf.Infinity;
            int closestIndex = 0;
            for(int i = 0; i < breakPoints.Length; i++) {
                float ratio = Mathf.Abs(xRatio - breakPoints[i].screenAspectRatio);
                if(ratio > breakPoints[i].screenAspectRatio && !Utils.MathTools.IsNear(breakPoints[i].screenAspectRatio, 0.01f)) continue;
                if(ratio < closest) {
                    closest = ratio;
                    closestIndex = i;
                }
            }

            canvasScaler.referenceResolution = breakPoints[closestIndex].referenceResolution;
        }

        [System.Serializable]
        private class BreakPoint {
            [SerializeField]
            public string name;
            [SerializeField]
            public float screenAspectRatio;
            [SerializeField]
            public Vector2 referenceResolution;
        }
    }

}
