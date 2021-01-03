// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
#define UNITY_2020_PLUS
#endif
#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    public static class UISelectionUtility {

#if UNITY_2019_PLUS
        private static Selectable[] s_reusableAllSelectables = new Selectable[0];
#endif

        // Find the next selectable object in the specified world-space direction.
        public static Selectable FindNextSelectable(Selectable selectable, Transform transform, Vector3 direction) {
            RectTransform rectTransform = transform as RectTransform;
            if(rectTransform == null) return null;

            IList<Selectable> allSelectables;
            int selectableCount;

#if UNITY_2019_PLUS
            
            // Resize array as needed to fit all Selectables
            if (Selectable.allSelectableCount > s_reusableAllSelectables.Length) {
                s_reusableAllSelectables = new Selectable[Selectable.allSelectableCount];
            }

            // Unity made a breaking API change in 2019.1.5 that removed the ref keyword. There is no clean way to catch it.
#if UNITY_2019_1_0 || UNITY_2019_1_1 || UNITY_2019_1_2 || UNITY_2019_1_3 || UNITY_2019_1_4 // 2019.1 up to 2019.1.4
            selectableCount = Selectable.AllSelectablesNoAlloc(ref s_reusableAllSelectables);
            allSelectables = s_reusableAllSelectables;
#else // all future versions
            selectableCount = Selectable.AllSelectablesNoAlloc(s_reusableAllSelectables);
            allSelectables = s_reusableAllSelectables;
#endif
#else // pre-2019 versions
            allSelectables = Selectable.allSelectables;
            selectableCount = allSelectables.Count;
#endif

            direction.Normalize();

            Vector2 localDir = direction;
            Vector2 searchStartPos = Rewired.UI.ControlMapper.UITools.GetPointOnRectEdge(rectTransform, localDir); // search from point on rect edge from center out in direction
            bool isHoriz = localDir == Vector2.right * -1f || localDir == Vector2.right;

            float minCenterDistSqMag = Mathf.Infinity;
            float minDirectLineSqMag = Mathf.Infinity;
            Selectable bestCenterDistPick = null;
            Selectable bestDirectLinePick = null;

            const float length = 999999f; // Mathf.Infinity fails
            Vector2 directLineCastEndPos = searchStartPos + localDir * length;

            for(int i = 0; i < selectableCount; ++i) {
                Selectable targetSelectable = allSelectables[i];

                if(targetSelectable == selectable || targetSelectable == null) continue; // skip if self or null
                if(targetSelectable.navigation.mode == Navigation.Mode.None) continue; // skip if non-navigable

                // Allow selection of non-interactable elements because it makes navigating easier and more predictable
                // but the CanvasGroup interactable value is private in Selectable
#if !UNITY_WSA
                // Reflect to get group intaractability if non-interactable
                bool canvasGroupAllowsInteraction = targetSelectable.IsInteractable() || Rewired.Utils.ReflectionTools.GetPrivateField<Selectable, bool>(targetSelectable, "m_GroupsAllowInteraction");
                if(!canvasGroupAllowsInteraction) continue; // skip if disabled by canvas group, otherwise allow it
#else
                // Can't do private field reflection in Metro
                if(!targetSelectable.IsInteractable()) continue; // skip if disabled
#endif

                var targetSelectableRectTransform = targetSelectable.transform as RectTransform;
                if(targetSelectableRectTransform == null) continue;

                // Check direct line cast from center edge of object in direction pressed
                float directLineSqMag;
                Rect targetSelecableRect = UITools.InvertY(UITools.TransformRectTo(targetSelectableRectTransform, transform, targetSelectableRectTransform.rect));

                // Check for direct line rect intersection
                if(Rewired.Utils.MathTools.LineIntersectsRect(searchStartPos, directLineCastEndPos, targetSelecableRect, out directLineSqMag)) {
                    if(isHoriz) directLineSqMag *= 0.25f; // give extra bonus to horizontal directions because most of the UI groups are laid out horizontally                     
                    if(directLineSqMag < minDirectLineSqMag) {
                        minDirectLineSqMag = directLineSqMag;
                        bestDirectLinePick = targetSelectable;
                    }
                }

                // Check distance to center
                Vector2 targetSelectableCenter = Rewired.Utils.UnityTools.TransformPoint(targetSelectableRectTransform, transform, targetSelectableRectTransform.rect.center);
                Vector2 searchPosToTargetSelectableCenter = targetSelectableCenter - searchStartPos;

                const float maxSafeAngle = 75.0f;

                // Get the angle the target center deviates from straight
                float angle = Mathf.Abs(Vector2.Angle(localDir, searchPosToTargetSelectableCenter));

                if(angle > maxSafeAngle) continue; // only consider if within a reasonable angle of the desired direction

                float score = searchPosToTargetSelectableCenter.sqrMagnitude;

                // Lower score is better
                if(score < minCenterDistSqMag) {
                    minCenterDistSqMag = score;
                    bestCenterDistPick = targetSelectable;
                }
            }

            // Choose between direct line and center dist
            if(bestDirectLinePick != null && bestCenterDistPick != null) {
                if(minDirectLineSqMag > minCenterDistSqMag) {
                    return bestCenterDistPick;
                }
                return bestDirectLinePick;
            }

#if UNITY_2019_PLUS
            System.Array.Clear(s_reusableAllSelectables, 0, s_reusableAllSelectables.Length);
#endif

            return bestDirectLinePick ?? bestCenterDistPick;
        }
    }
}
