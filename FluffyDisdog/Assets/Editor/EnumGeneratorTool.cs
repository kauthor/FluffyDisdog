using System.Collections.Generic;
using System.Linq;
using FluffyDisdog;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EnumGeneratorTool : EditorWindow
    {
        private Vector2 scrollPosition;
        private ToolType[] tabs;
        private List<string> currentList;
        
        public static void Open()
        {
            var window = GetWindow<EnumGeneratorTool>();
            window.Init();
        }

        void Init()
        {
            currentList = new List<string>();
            tabs = new ToolType[(int)ToolType.MAX -1];
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i] = (ToolType)i;
                currentList.Add( tabs[i].ToString() );
            }
        }
        
        void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(100)); // 탭의 넓이를 고정
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(200), GUILayout.Height(500));

            for (int i = 0; i < currentList .Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                currentList[i] = EditorGUILayout.TextField($"항목 {i + 1}", currentList[i]);
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    currentList.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            
            GUILayout.EndScrollView();

            
            if (GUILayout.Button("항목 추가"))
            {
                currentList.Add("NewValue");
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Enum 스크립트 생성"))
            {
               EnumGenerator.GenerateCardName(currentList.ToArray());
            }
            
            
            GUILayout.EndVertical();
        }
    }
}