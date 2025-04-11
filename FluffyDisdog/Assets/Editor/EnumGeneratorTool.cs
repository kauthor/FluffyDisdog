using System.Collections.Generic;
using System.IO;
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
        private string enumName = "ToolType";
        
        public static void Open()
        {
            var window = GetWindow<EnumGeneratorTool>();
            window.Init();
        }

        void Init(int type=0)
        {
            currentList = new List<string>();
            
            
            
            for (int i = 0; i < (int)ToolType.MAX; i++)
            {
                
                currentList.Add(  ((ToolType)i).ToString() );
            }
        }
        
        void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(500)); // 탭의 넓이를 고정
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(500), GUILayout.Height(600));

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
               //EnumGenerator.GenerateCardName(currentList.ToArray());
               CreateEnumScript();
            }
            
            
            GUILayout.EndVertical();
        }
        
        private void CreateEnumScript()
        {
            if (string.IsNullOrEmpty(enumName) || currentList.Count == 0)
            {
                EditorUtility.DisplayDialog("오류", "Enum 이름과 항목들을 입력하세요.", "확인");
                return;
            }

            string path = Application.dataPath + "/Script/FluffyDisdog/TileClass/ToolType.cs";

            if (string.IsNullOrEmpty(path))
                return;

            int curNum = 0;
            
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("namespace FluffyDisdog ");
                writer.WriteLine("{");
                writer.WriteLine("public enum " + enumName);
                writer.WriteLine("{");
                writer.WriteLine($"    None = -1,");
                for (int i = 0; i < currentList.Count; i++)
                {
                    string line = $"    {currentList[i]} = {curNum++} ,";
                    writer.WriteLine(line);
                }
                writer.WriteLine($"    MAX = {curNum},");
                writer.WriteLine("}");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("완료", $"Enum {enumName} 생성 완료!", "확인");
        }
    }
    
    
}