// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIButton), true)]
    [CanEditMultipleObjects]
    public sealed class UIButtonEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIButton castedTarget => (UIButton)target;
        public IEnumerable<UIButton> castedTargets => targets.Cast<UIButton>();

        public static IEnumerable<Texture2D> buttonIconTextures => EditorSpriteSheets.UIManager.Icons.UIButton;

        private FluidField idField { get; set; }

        private SerializedProperty propertyId { get; set; }

        protected override void SearchForAnimators()
        {
            selectableAnimators ??= new List<BaseUISelectableAnimator>();
            selectableAnimators.Clear();
            
            //check if prefab was selected
            if (castedTargets.Any(s => s.gameObject.scene.name == null)) 
            {
                selectableAnimators.AddRange(castedSelectable.GetComponentsInChildren<BaseUISelectableAnimator>());
                return;
            }
            
            //not prefab
            selectableAnimators.AddRange(FindObjectsOfType<BaseUISelectableAnimator>());
        }
        
        protected override void FindProperties()
        {
            base.FindProperties();

            propertyId = serializedObject.FindProperty(nameof(UIButton.Id));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText("UIButton")
                .SetIcon(buttonIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048117262/UIButton?atlOrigin=eyJpIjoiOTY4ZDg1Yjk5NDYwNDRmMmFmZDg4OWQyM2VlMjBmNmQiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Components.UIButton.html")
                .AddYouTubeButton();

            idField = 
                FluidField.Get()
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyId));
        }

        protected override VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(statesTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(behavioursTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(navigationTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.SystemButton_RenameComponent
                        (
                            castedTarget.gameObject,
                            () => $"Button - {castedTarget.Id.Name}"
                        )
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            ((UISelectable)target).gameObject,
                            nameof(RectTransform),
                            nameof(UIButton)
                        )
                    );
        }
        
        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(idField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
