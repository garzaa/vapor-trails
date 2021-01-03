// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System.Collections;
    using Rewired;

    public interface ICustomSelectable : ICancelHandler {

        Sprite disabledHighlightedSprite { get; set; }
        Color disabledHighlightedColor { get; set; }
        string disabledHighlightedTrigger { get; set; }

        bool autoNavUp { get; set; }
        bool autoNavDown { get; set; }
        bool autoNavLeft { get; set; }
        bool autoNavRight { get; set; }

        event UnityAction CancelEvent;

    }
}
