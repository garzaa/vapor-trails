// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

//#define REWIRED_CONTROL_MAPPER_USE_TMPRO

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
    using Rewired;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
    using Text = TMPro.TMP_Text;
    using Font = TMPro.TMP_FontAsset;
#else
    using Text = UnityEngine.UI.Text;
    using Font = UnityEngine.Font;
#endif

    [System.Serializable]
    public class ThemeSettings : ScriptableObject {

        [SerializeField]
        private ImageSettings _mainWindowBackground;
        [SerializeField]
        private ImageSettings _popupWindowBackground;
        [SerializeField]
        private ImageSettings _areaBackground;
        [SerializeField]
        private SelectableSettings _selectableSettings;
        [SerializeField]
        private SelectableSettings _buttonSettings;
        [SerializeField]
        private SelectableSettings _inputGridFieldSettings;
        [SerializeField]
        private ScrollbarSettings _scrollbarSettings;
        [SerializeField]
        private SliderSettings _sliderSettings;
        [SerializeField]
        private ImageSettings _invertToggle;
        [SerializeField]
        private Color _invertToggleDisabledColor;
        [SerializeField]
        private ImageSettings _calibrationBackground;
        [SerializeField]
        private ImageSettings _calibrationValueMarker;
        [SerializeField]
        private ImageSettings _calibrationRawValueMarker;
        [SerializeField]
        private ImageSettings _calibrationZeroMarker;
        [SerializeField]
        private ImageSettings _calibrationCalibratedZeroMarker;
        [SerializeField]
        private ImageSettings _calibrationDeadzone;
        [SerializeField]
        private TextSettings _textSettings;
        [SerializeField]
        private TextSettings _buttonTextSettings;
        [SerializeField]
        private TextSettings _inputGridFieldTextSettings;

        public void Apply(ThemedElement.ElementInfo[] elementInfo) {
            if(elementInfo == null) return;
            for(int i = 0; i < elementInfo.Length; i++) {
                if(elementInfo[i] == null) continue;
                Apply(elementInfo[i].themeClass, elementInfo[i].component);
            }
        }

        private void Apply(string themeClass, Component component) {
            if(component as Selectable != null) {
                Apply(themeClass, (Selectable)component);
                return;
            }

            if(component as Image != null) {
                Apply(themeClass, (Image)component);
                return;
            }

            if(component as Text != null) {
                Apply(themeClass, (Text)component);
                return;
            }

            if(component as UIImageHelper != null) {
                Apply(themeClass, (UIImageHelper)component);
                return;
            }
        }

        private void Apply(string themeClass, Selectable item) {
            if(item == null) return;

            SelectableSettings_Base settings;
            if(item as Button != null) {
                switch(themeClass) {
                    case "inputGridField":
                        settings = _inputGridFieldSettings;
                        break;
                    default:
                        settings = _buttonSettings;
                        break;
                }

            } else if(item as Scrollbar != null) settings = _scrollbarSettings;
            else if(item as Slider != null) settings = _sliderSettings;
            else if(item as Toggle != null) {
                switch(themeClass) {
                    case "button":
                        settings = _buttonSettings;
                        break;
                    default:
                        settings = _selectableSettings;
                        break;
                }

            } else settings = _selectableSettings;

            settings.Apply(item);
        }

        private void Apply(string themeClass, Image item) {
            if(item == null) return;

            switch(themeClass) {
                case "area":
                    if (_areaBackground != null) _areaBackground.CopyTo(item);
                    break;
                case "popupWindow":
                    if (_popupWindowBackground != null) _popupWindowBackground.CopyTo(item);
                    break;
                case "mainWindow":
                    if (_mainWindowBackground != null) _mainWindowBackground.CopyTo(item);
                    break;
                case "calibrationValueMarker":
                    if (_calibrationValueMarker != null) _calibrationValueMarker.CopyTo(item);
                    break;
                case "calibrationRawValueMarker":
                    if (_calibrationRawValueMarker != null) _calibrationRawValueMarker.CopyTo(item);
                    break;
                case "calibrationBackground":
                    if (_calibrationBackground != null) _calibrationBackground.CopyTo(item);
                    break;
                case "calibrationZeroMarker":
                    if (_calibrationZeroMarker != null) _calibrationZeroMarker.CopyTo(item);
                    break;
                case "calibrationCalibratedZeroMarker":
                    if (_calibrationCalibratedZeroMarker != null) _calibrationCalibratedZeroMarker.CopyTo(item);
                    break;
                case "calibrationDeadzone":
                    if (_calibrationDeadzone != null) _calibrationDeadzone.CopyTo(item);
                    break;
                case "invertToggle":
                    if (_invertToggle != null) _invertToggle.CopyTo(item);
                    break;
                case "invertToggleBackground":
                    if (_inputGridFieldSettings != null) _inputGridFieldSettings.imageSettings.CopyTo(item);
                    break;
                case "invertToggleButtonBackground":
                    if (_buttonSettings != null) _buttonSettings.imageSettings.CopyTo(item);
                    break;
            }
        }

        private void Apply(string themeClass, Text item) {
            if(item == null) return;

            TextSettings settings;

            switch(themeClass) {
                case "button":
                    settings = _buttonTextSettings;
                    break;
                case "inputGridField":
                    settings = _inputGridFieldTextSettings;
                    break;
                default:
                    settings = _textSettings;
                    break;
            }

            if(settings.font != null) item.font = settings.font;
            item.color = settings.color;
            item.lineSpacing = settings.lineSpacing;
            if(settings.sizeMultiplier != 1.0f) {
                item.fontSize = (int)(item.fontSize * settings.sizeMultiplier);
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
                item.fontSizeMax = (int)(item.fontSizeMax * settings.sizeMultiplier);
                item.fontSizeMin = (int)(item.fontSizeMin * settings.sizeMultiplier);
#else
                item.resizeTextMaxSize = (int)(item.resizeTextMaxSize * settings.sizeMultiplier);
                item.resizeTextMinSize = (int)(item.resizeTextMinSize * settings.sizeMultiplier);
#endif
            }
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
            item.characterSpacing = settings.chracterSpacing;
            item.wordSpacing = settings.wordSpacing;
#endif
            if(settings.style != FontStyleOverride.Default) {
                item.fontStyle = GetFontStyle(settings.style);
            }
        }

        private void Apply(string themeClass, UIImageHelper item) {
            if(item == null) return;

            item.SetEnabledStateColor(_invertToggle.color);
            item.SetDisabledStateColor(_invertToggleDisabledColor);
            item.Refresh();
        }

#if REWIRED_CONTROL_MAPPER_USE_TMPRO
        private static TMPro.FontStyles GetFontStyle(FontStyleOverride style) {
            switch(style) {
                case FontStyleOverride.Bold:
                    return TMPro.FontStyles.Bold;
                case FontStyleOverride.BoldAndItalic:
                    return TMPro.FontStyles.Bold | TMPro.FontStyles.Italic;
                case FontStyleOverride.Italic:
                    return TMPro.FontStyles.Italic;
                case FontStyleOverride.Default:
                case FontStyleOverride.Normal:
                    return TMPro.FontStyles.Normal;
                default:
                    throw new System.NotImplementedException();
            }
        }
#else
        private static FontStyle GetFontStyle(FontStyleOverride style) {
                return (FontStyle)((int)style - 1);
        }
#endif

        [System.Serializable]
        private abstract class SelectableSettings_Base {

            [SerializeField]
            protected Selectable.Transition _transition;
            [SerializeField]
            protected CustomColorBlock _colors;
            [SerializeField]
            protected CustomSpriteState _spriteState;
            [SerializeField]
            protected CustomAnimationTriggers _animationTriggers;

            public Selectable.Transition transition { get { return _transition; } }
            public CustomColorBlock selectableColors { get { return _colors; } }
            public CustomSpriteState spriteState { get { return _spriteState; } }
            public CustomAnimationTriggers animationTriggers { get { return _animationTriggers; } }

            public virtual void Apply(Selectable item) {
                Selectable.Transition transition = _transition;
                bool transitionChanged = item.transition != transition;
                item.transition = transition;

                ICustomSelectable customSel = item as ICustomSelectable;

                if(transition == Selectable.Transition.ColorTint) {
                    // Two-step color change to get around delay bug due to fade duration
                    CustomColorBlock cb = _colors;
                    cb.fadeDuration = 0.0f;
                    item.colors = cb;
                    cb.fadeDuration = _colors.fadeDuration;
                    item.colors = cb;
                    if(customSel != null) customSel.disabledHighlightedColor = cb.disabledHighlightedColor;

                } else if(transition == Selectable.Transition.SpriteSwap) {
                    item.spriteState = _spriteState;
                    if(customSel != null) customSel.disabledHighlightedSprite = _spriteState.disabledHighlightedSprite;
                } else if(transition == Selectable.Transition.Animation) {
                    item.animationTriggers.disabledTrigger = _animationTriggers.disabledTrigger;
                    item.animationTriggers.highlightedTrigger = _animationTriggers.highlightedTrigger;
                    item.animationTriggers.normalTrigger = _animationTriggers.normalTrigger;
                    item.animationTriggers.pressedTrigger = _animationTriggers.pressedTrigger;
                    if(customSel != null) customSel.disabledHighlightedTrigger = _animationTriggers.disabledHighlightedTrigger;
                }

                if(transitionChanged) item.targetGraphic.CrossFadeColor(item.targetGraphic.color, 0.0f, true, true); // force color to revert to default or it will be left with color tint
            }
        }

        [System.Serializable]
        private class SelectableSettings : SelectableSettings_Base {

            [SerializeField]
            private ImageSettings _imageSettings;
            public ImageSettings imageSettings { get { return _imageSettings; } }

            public override void Apply(Selectable item) {
                if(item == null) return;
                base.Apply(item);

                if(_imageSettings != null) _imageSettings.CopyTo(item.targetGraphic as Image);
            }
        }

        [System.Serializable]
        private class SliderSettings : SelectableSettings_Base {

            [SerializeField]
            private ImageSettings _handleImageSettings;
            [SerializeField]
            private ImageSettings _fillImageSettings;
            [SerializeField]
            private ImageSettings _backgroundImageSettings;

            public ImageSettings handleImageSettings { get { return _handleImageSettings; } }
            public ImageSettings fillImageSettings { get { return _fillImageSettings; } }
            public ImageSettings backgroundImageSettings { get { return _backgroundImageSettings; } }

            private void Apply(Slider item) {
                if(item == null) return;

                if(_handleImageSettings != null) _handleImageSettings.CopyTo(item.targetGraphic as Image);
                if(_fillImageSettings != null) {
                    RectTransform rt = item.fillRect;
                    if(rt != null) _fillImageSettings.CopyTo(rt.GetComponent<Image>());
                }
                if(_backgroundImageSettings != null) {
                    Transform t = item.transform.Find("Background");
                    if(t != null) {
                        _backgroundImageSettings.CopyTo(t.GetComponent<Image>());
                    }
                }
            }

            public override void Apply(Selectable item) {
                base.Apply(item);
                Apply(item as Slider);
            }
        }

        [System.Serializable]
        private class ScrollbarSettings : SelectableSettings_Base {

            [SerializeField]
            private ImageSettings _handleImageSettings;
            [SerializeField]
            private ImageSettings _backgroundImageSettings;

            public ImageSettings handle { get { return _handleImageSettings; } }
            public ImageSettings background { get { return _backgroundImageSettings; } }

            private void Apply(Scrollbar item) {
                if(item == null) return;
                
                if(_handleImageSettings != null) _handleImageSettings.CopyTo(item.targetGraphic as Image);
                if(_backgroundImageSettings != null) _backgroundImageSettings.CopyTo(item.GetComponent<Image>());
            }

            public override void Apply(Selectable item) {
                base.Apply(item);
                Apply(item as Scrollbar);
            }
        }

        [System.Serializable]
        private class ImageSettings {

            [SerializeField]
            private Color _color = Color.white;
            [SerializeField]
            private Sprite _sprite;
            [SerializeField]
            private Material _materal;
            [SerializeField]
            private Image.Type _type;
            [SerializeField]
            private bool _preserveAspect;
            [SerializeField]
            private bool _fillCenter;
            [SerializeField]
            private Image.FillMethod _fillMethod;
            [SerializeField]
            private float _fillAmout;
            [SerializeField]
            private bool _fillClockwise;
            [SerializeField]
            private int _fillOrigin;

            public Color color { get { return _color; } }
            public Sprite sprite { get { return _sprite; } }
            public Material materal { get { return _materal; } }
            public Image.Type type { get { return _type; } }
            public bool preserveAspect { get { return _preserveAspect; } }
            public bool fillCenter { get { return _fillCenter; } }
            public Image.FillMethod fillMethod { get { return _fillMethod; } }
            public float fillAmout { get { return _fillAmout; } }
            public bool fillClockwise { get { return _fillClockwise; } }
            public int fillOrigin { get { return _fillOrigin; } }

            public virtual void CopyTo(Image image) {
                if(image == null) return;
                image.color = _color;
                image.sprite = _sprite;
                image.material = _materal;
                image.type = _type;
                image.preserveAspect = _preserveAspect;
                image.fillCenter = _fillCenter;
                image.fillMethod = _fillMethod;
                image.fillAmount = _fillAmout;
                image.fillClockwise = _fillClockwise;
                image.fillOrigin = _fillOrigin;
            }
        }

        [System.Serializable]
        private struct CustomColorBlock {

            [SerializeField]
            private float m_ColorMultiplier;
            [SerializeField]
            private Color m_DisabledColor;
            [SerializeField]
            private float m_FadeDuration;
            [SerializeField]
            private Color m_HighlightedColor;
            [SerializeField]
            private Color m_NormalColor;
            [SerializeField]
            private Color m_PressedColor;
            [SerializeField]
            private Color m_SelectedColor;
            [SerializeField]
            private Color m_DisabledHighlightedColor;

            public float colorMultiplier { get { return m_ColorMultiplier; } set { m_ColorMultiplier = value; } }
            public Color disabledColor { get { return m_DisabledColor; } set { m_DisabledColor = value; } }
            public float fadeDuration { get { return m_FadeDuration; } set { m_FadeDuration = value; } }
            public Color highlightedColor { get { return m_HighlightedColor; } set { m_HighlightedColor = value; } }
            public Color normalColor { get { return m_NormalColor; } set { m_NormalColor = value; } }
            public Color pressedColor { get { return m_PressedColor; } set { m_PressedColor = value; } }
            public Color selectedColor { get { return m_SelectedColor; } set { m_SelectedColor = value; } }
            public Color disabledHighlightedColor { get { return m_DisabledHighlightedColor; } set { m_DisabledHighlightedColor = value; } }

            public static implicit operator ColorBlock(CustomColorBlock item) {
                return new ColorBlock() {
#if UNITY_2019_PLUS
                    selectedColor = item.m_SelectedColor,
#endif
                    colorMultiplier = item.m_ColorMultiplier,
                    disabledColor = item.m_DisabledColor,
                    fadeDuration = item.m_FadeDuration,
                    highlightedColor = item.m_HighlightedColor,
                    normalColor = item.m_NormalColor,
                    pressedColor = item.m_PressedColor
                };
            }
        }

        [System.Serializable]
        private struct CustomSpriteState {

            public Sprite disabledSprite { get { return m_DisabledSprite; } set { m_DisabledSprite = value; } }
            public Sprite highlightedSprite { get { return m_HighlightedSprite; } set { m_HighlightedSprite = value; } }
            public Sprite pressedSprite { get { return m_PressedSprite; } set { m_PressedSprite = value; } }
            public Sprite selectedSprite { get { return m_SelectedSprite; } set { m_SelectedSprite = value; } }
            public Sprite disabledHighlightedSprite { get { return m_DisabledHighlightedSprite; } set { m_DisabledHighlightedSprite = value; } }

            [SerializeField]
            private Sprite m_DisabledSprite;
            [SerializeField]
            private Sprite m_HighlightedSprite;
            [SerializeField]
            private Sprite m_PressedSprite;
            [SerializeField]
            private Sprite m_SelectedSprite;
            [SerializeField]
            private Sprite m_DisabledHighlightedSprite;

            public static implicit operator SpriteState(CustomSpriteState item) {
                return new SpriteState() {
#if UNITY_2019_PLUS
                    selectedSprite = item.m_SelectedSprite,
#endif
                    disabledSprite = item.m_DisabledSprite,
                    highlightedSprite = item.m_HighlightedSprite,
                    pressedSprite = item.m_PressedSprite,
                };
            }
        }

        [System.Serializable]
        private class CustomAnimationTriggers {

            public CustomAnimationTriggers() {
                m_DisabledTrigger = string.Empty;
                m_HighlightedTrigger = string.Empty;
                m_NormalTrigger = string.Empty;
                m_PressedTrigger = string.Empty;
                m_SelectedTrigger = string.Empty;
                m_DisabledHighlightedTrigger = string.Empty;
            }

            public string disabledTrigger { get { return m_DisabledTrigger; } set { m_DisabledTrigger = value; } }
            public string highlightedTrigger { get { return m_HighlightedTrigger; } set { m_HighlightedTrigger = value; } }
            public string normalTrigger { get { return m_NormalTrigger; } set { m_NormalTrigger = value; } }
            public string pressedTrigger { get { return m_PressedTrigger; } set { m_PressedTrigger = value; } }
            public string selectedTrigger { get { return m_SelectedTrigger; } set { m_SelectedTrigger = value; } }
            public string disabledHighlightedTrigger { get { return m_DisabledHighlightedTrigger; } set { m_DisabledHighlightedTrigger = value; } }

            [SerializeField]
            private string m_DisabledTrigger;
            [SerializeField]
            private string m_HighlightedTrigger;
            [SerializeField]
            private string m_NormalTrigger;
            [SerializeField]
            private string m_PressedTrigger;
            [SerializeField]
            private string m_SelectedTrigger;
            [SerializeField]
            private string m_DisabledHighlightedTrigger;

            public static implicit operator AnimationTriggers(CustomAnimationTriggers item) {
                return new AnimationTriggers() {
#if UNITY_2019_PLUS
                    selectedTrigger = item.m_SelectedTrigger,
#endif
                    disabledTrigger = item.m_DisabledTrigger,
                    highlightedTrigger = item.m_HighlightedTrigger,
                    normalTrigger = item.m_NormalTrigger,
                    pressedTrigger = item.m_PressedTrigger
                };
            }
        }

        [System.Serializable]
        private class TextSettings {
            [SerializeField]
            private Color _color = Color.white;
            [SerializeField]
            private Font _font;
            [SerializeField]
            private FontStyleOverride _style = FontStyleOverride.Default;
            [SerializeField]
            private float _sizeMultiplier = 1.0f;
            [SerializeField]
            private float _lineSpacing = 1.0f;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
            [SerializeField]
            private float _characterSpacing = 1.0f;
            [SerializeField]
            private float _wordSpacing = 1.0f;
#endif
            public Color color { get { return _color; } }
            public Font font { get { return _font; } }
            public FontStyleOverride style { get { return _style; } }
            public float sizeMultiplier { get { return _sizeMultiplier; } }
            public float lineSpacing { get { return _lineSpacing; } }
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
            public float chracterSpacing { get { return _characterSpacing; } }
            public float wordSpacing { get { return _wordSpacing; } }
#endif
        }

        private enum FontStyleOverride {
            Default = 0,
            Normal = 1,
            Bold = 2,
            Italic = 3,
            BoldAndItalic = 4,
        }
    }
}
