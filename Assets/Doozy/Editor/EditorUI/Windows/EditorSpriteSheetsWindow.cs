// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.ScriptableObjects.SpriteSheets;
using Doozy.Editor.EditorUI.Windows.Internal;
using UnityEditor;

namespace Doozy.Editor.EditorUI.Windows
{
    public class EditorSpriteSheetsWindow : EditorUIDatabaseWindow<EditorSpriteSheetsWindow>
    {
        private const string WINDOW_TITLE = "Editor SpriteSheets";

        [MenuItem(EditorUIWindow.k_WindowMenuPath + WINDOW_TITLE + "/Window", priority = -550)]
        private static void ShowWindow() => InternalOpenWindow(WINDOW_TITLE);
        
        [MenuItem(EditorUIWindow.k_WindowMenuPath + WINDOW_TITLE + "/Refresh", priority = -500)]
        private static void RefreshDatabase()
        {
            if (EditorUtility.DisplayDialog
                (
                    $"Refresh the {WINDOW_TITLE} database?",
                    "This will regenerate the database with the latest registered sprite sheets, from the source files." +
                    "\n\n" +
                    "Takes around 1 to 30 seconds, depending on the number of source files and your computer's performance." +
                    "\n\n" +
                    "This operation cannot be undone!",
                    "Yes",
                    "No"
                )
               )
            EditorDataSpriteSheetDatabase.instance.RefreshDatabase();
        }
    }
}
