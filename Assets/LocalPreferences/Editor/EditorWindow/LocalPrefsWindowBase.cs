using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Neonagee.EditorInternal
{
    internal class LocalPrefsWindowBase : EditorWindow
    {
        internal static GUIStyle notificationBox;
        internal static GUIStyle notificationBoxSmall;
        internal static GUIStyle foldout;
        internal static GUIStyle macRemoveButton;
        internal static GUIStyle regionBg;
        internal static GUIStyle searchTextField;
        internal static GUIStyle searchCancelButton;

        internal interface IPrefGUI
        {
            void Expand();
            void Collapse();
        }

        internal void GetGUIStyles()
        {
            notificationBox = new GUIStyle("NotificationBackground")
            {
                fontSize = 18, 
                stretchWidth = true, 
                normal =
                {
                    textColor = Color.black,
                    background = MakeTex(600, 1, new Color(0.2509804f, 0.2509804f, 0.2509804f, 1f))
                },
            };
            
            notificationBoxSmall = new GUIStyle(notificationBox)
            {
                margin = new RectOffset(4, 4, 4, 4),
                padding = new RectOffset(4, 4, 4, 4),
                border = new RectOffset(12, 12, 12, 12),
                fontSize = 13,
                normal =
                {
                    textColor = Color.black,
                    background = MakeTex(600, 1, new Color(0.2509804f, 0.2509804f, 0.2509804f, 1f))
                },
            };
            
            foldout = new GUIStyle("Foldout")
            {
                fontSize = 12, 
                normal =
                {
                    textColor = Color.black,
                },
            };
            
            macRemoveButton = new GUIStyle("WinBtnMinMac");
            
            regionBg = new GUIStyle("RegionBg")
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(8, 8, 0, 12),
                border = new RectOffset(12, 12, 12, 16),
                contentOffset = new Vector2(0, 0),
                stretchWidth = true,
                stretchHeight = true,
                fixedHeight = 0,
                normal =
                {
                    textColor = new Color(0.2509804f, 0.2509804f, 0.2509804f, 1f),
                },
            };
            
            string regionBgTexGUID = AssetDatabase.FindAssets("RegionBgStyleNormalState")[0];
            Texture2D regionBgTex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(regionBgTexGUID));
            if (regionBgTex)
                regionBg.normal.background = regionBgTex;

            searchTextField = new GUIStyle("ToolbarSeachTextField");
            searchCancelButton = new GUIStyle("ToolbarSeachCancelButton");
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width*height];
     
            for(int i = 0; i < pix.Length; i++)
                pix[i] = col;
     
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
     
            return result;
        }
    }
    
}
