// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;


    /// <summary>
    /// This class exists only for the purpose of being able to force a refresh on the canvas scaler to prevent drawing artifacts when changing the scale on Awake/Enable
    /// </summary>
    [AddComponentMenu("")]
    public class CanvasScalerExt : CanvasScaler {
        
        /// <summary>
        /// Force a refresh on the canvas scaler.
        /// </summary>
        public void ForceRefresh() {
            Handle();
        }
    }
}
