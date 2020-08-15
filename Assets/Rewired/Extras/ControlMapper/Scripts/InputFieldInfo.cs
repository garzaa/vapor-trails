// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using Rewired;

    [AddComponentMenu("")]
    public class InputFieldInfo : UIElementInfo {
        public int actionId { get; set; }
        public AxisRange axisRange { get; set; }
        public int actionElementMapId { get; set; }
        public ControllerType controllerType { get; set; }
        public int controllerId { get; set; }
    }
}
