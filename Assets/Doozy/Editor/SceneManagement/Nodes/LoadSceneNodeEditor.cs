// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.SceneManagement;
using Doozy.Runtime.SceneManagement.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.SceneManagement.Nodes
{
    [CustomEditor(typeof(LoadSceneNode))]
    public class LoadSceneNodeEditor : FlowNodeEditor
    {
        public override Color nodeAccentColor => EditorColors.SceneManagement.Component;
        public override EditorSelectableColorInfo nodeSelectableAccentColor => EditorSelectableColors.SceneManagement.Component;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.SceneManagement.Icons.LoadSceneNode;

        private FluidField allowSceneActivationFluidField { get; set; }
        private FluidField getSceneByFluidField { get; set; }
        private FluidField loadSceneModeFluidField { get; set; }
        private FluidField progressorIdFluidField { get; set; }
        private FluidField sceneActivationDelayFluidField { get; set; }
        private FluidField sceneBuildIndexFluidField { get; set; }
        private FluidField sceneNameFluidField { get; set; }
        private FluidField waitForSceneToLoadFluidField { get; set; }
        private FluidToggleSwitch allowSceneActivationSwitch { get; set; }
        private FluidToggleSwitch connectProgressorSwitch { get; set; }
        private FluidToggleSwitch waitForSceneToLoadSwitch { get; set; }

        private SerializedProperty propertyAllowSceneActivation { get; set; }
        private SerializedProperty propertyConnectProgressor { get; set; }
        private SerializedProperty propertyGetSceneBy { get; set; }
        private SerializedProperty propertyLoadSceneMode { get; set; }
        private SerializedProperty propertyProgressorId { get; set; }
        private SerializedProperty propertySceneActivationDelay { get; set; }
        private SerializedProperty propertySceneBuildIndex { get; set; }
        private SerializedProperty propertySceneName { get; set; }
        private SerializedProperty propertyWaitForSceneToLoad { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            allowSceneActivationFluidField?.Recycle();
            getSceneByFluidField?.Recycle();
            loadSceneModeFluidField?.Recycle();
            sceneActivationDelayFluidField?.Recycle();
            sceneBuildIndexFluidField?.Recycle();
            sceneNameFluidField?.Recycle();
            waitForSceneToLoadFluidField?.Recycle();

            allowSceneActivationSwitch?.Recycle();
            connectProgressorSwitch?.Recycle();
            waitForSceneToLoadSwitch?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyAllowSceneActivation = serializedObject.FindProperty(nameof(LoadSceneNode.AllowSceneActivation));
            propertyConnectProgressor = serializedObject.FindProperty(nameof(LoadSceneNode.ConnectProgressor));
            propertyGetSceneBy = serializedObject.FindProperty(nameof(LoadSceneNode.GetSceneBy));
            propertyLoadSceneMode = serializedObject.FindProperty(nameof(LoadSceneNode.LoadSceneMode));
            propertyProgressorId = serializedObject.FindProperty(nameof(LoadSceneNode.ProgressorId));
            propertySceneActivationDelay = serializedObject.FindProperty(nameof(LoadSceneNode.SceneActivationDelay));
            propertySceneBuildIndex = serializedObject.FindProperty(nameof(LoadSceneNode.SceneBuildIndex));
            propertySceneName = serializedObject.FindProperty(nameof(LoadSceneNode.SceneName));
            propertyWaitForSceneToLoad = serializedObject.FindProperty(nameof(LoadSceneNode.WaitForSceneToLoad));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(LoadSceneNode)))
                .SetIcon(EditorSpriteSheets.SceneManagement.Icons.LoadSceneNode)
                .SetAccentColor(EditorColors.SceneManagement.Component)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton()
                ;

            loadSceneModeFluidField =
                FluidField.Get<EnumField>(propertyLoadSceneMode)
                    .SetLabelText("Load Scene Mode")
                    .SetStyleMaxWidth(112);

            EnumField getSceneByEnumField =
                DesignUtils.NewEnumField(propertyGetSceneBy).SetStyleFlexGrow(1);

            getSceneByFluidField =
                FluidField.Get()
                    .SetLabelText("Get Scene By")
                    .AddFieldContent(getSceneByEnumField)
                    .SetStyleMaxWidth(112);

            sceneNameFluidField =
                FluidField.Get<TextField>(propertySceneName)
                    .SetLabelText("Scene Name");

            sceneBuildIndexFluidField =
                FluidField.Get<TextField>(propertySceneBuildIndex)
                    .SetLabelText("Scene Build Index");

            sceneNameFluidField.SetStyleDisplay(propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.Name ? DisplayStyle.Flex : DisplayStyle.None);
            sceneBuildIndexFluidField.SetStyleDisplay(propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.BuildIndex ? DisplayStyle.Flex : DisplayStyle.None);
            getSceneByEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                sceneNameFluidField.SetStyleDisplay((GetSceneBy)evt.newValue == GetSceneBy.Name ? DisplayStyle.Flex : DisplayStyle.None);
                sceneBuildIndexFluidField.SetStyleDisplay((GetSceneBy)evt.newValue == GetSceneBy.BuildIndex ? DisplayStyle.Flex : DisplayStyle.None);
            });

            allowSceneActivationSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Allow Scene Activation")
                    .SetToggleAccentColor(nodeSelectableAccentColor)
                    .BindToProperty(propertyAllowSceneActivation);

            allowSceneActivationFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .AddFieldContent(allowSceneActivationSwitch);

            allowSceneActivationFluidField.fieldContent.SetStyleJustifyContent(Justify.Center);
            
            waitForSceneToLoadSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Wait For Scene To Load")
                    .SetToggleAccentColor(nodeSelectableAccentColor)
                    .BindToProperty(propertyWaitForSceneToLoad);

            waitForSceneToLoadFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .AddFieldContent(waitForSceneToLoadSwitch);

            waitForSceneToLoadFluidField.fieldContent.SetStyleJustifyContent(Justify.Center);

            sceneActivationDelayFluidField =
                FluidField.Get<FloatField>(propertySceneActivationDelay)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Cooldown)
                    .SetLabelText("Scene Activation Delay");

            sceneActivationDelayFluidField.SetEnabled(propertyAllowSceneActivation.boolValue);
            allowSceneActivationSwitch.SetOnValueChanged(evt => sceneActivationDelayFluidField.SetEnabled(evt.newValue));

            connectProgressorSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Connect Progressor")
                    .SetToggleAccentColor(nodeSelectableAccentColor)
                    .BindToProperty(propertyConnectProgressor);

            PropertyField progressorIdPropertyField = DesignUtils.NewPropertyField(propertyProgressorId);

            progressorIdFluidField =
                FluidField.Get()
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.Progressor)
                    .AddFieldContent
                    (
                        DesignUtils.column
                            .AddChild(connectProgressorSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(progressorIdPropertyField)
                    )
                ;

            progressorIdPropertyField.SetEnabled(propertyConnectProgressor.boolValue);
            connectProgressorSwitch.SetOnValueChanged(evt => progressorIdPropertyField.SetEnabled(evt.newValue));

            AutoRefreshNodeView(); // <<< IMPORTANT - this updates the NodeView
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild
                (
                    DesignUtils.column
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(getSceneByFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(sceneNameFluidField)
                                .AddChild(sceneBuildIndexFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(loadSceneModeFluidField)
                        )
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(waitForSceneToLoadFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(allowSceneActivationFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(sceneActivationDelayFluidField)
                        )
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(progressorIdFluidField)
                )
                ;
        }
    }
}
