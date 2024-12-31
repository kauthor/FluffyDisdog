using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CardDesignTool : EditorWindow
    {
        private string[] cellNames; // 셀의 이름 또는 데이터를 담을 배열
        private int numCells = 10;  // 셀의 개수
        
        private string[] tabs = { "Tab 1", "Tab 2", "Tab 3" }; // 탭 이름
        private int selectedTab = 0; // 현재 선택된 탭
        private string[][] tabContent; // 탭별 내용
        
        [MenuItem("LevelDesign/Card")]
        static void Open()
        {
            var window = GetWindow<CardDesignTool>();
            window.Init(10);
        }

        private void Init(int n = 1)
        {
            numCells = n;
            cellNames = new string[numCells];
            for (int i = 0; i < numCells; i++)
            {
                cellNames[i] = $"Cell {i + 1}";
            }
            
            // 각 탭에 대한 셀 데이터 초기화
            tabContent = new string[tabs.Length][];
            for (int i = 0; i < tabs.Length; i++)
            {
                tabContent[i] = new string[5];
                for (int j = 0; j < tabContent[i].Length; j++)
                {
                    tabContent[i][j] = $"{tabs[i]} - Item {j + 1}";
                }
            }
        }
        
        void OnGUI()
        {
            GUILayout.BeginHorizontal(); // 수평 레이아웃 시작

            // 좌측 탭 영역
            GUILayout.BeginVertical(GUILayout.Width(100)); // 탭의 넓이를 고정
            for (int i = 0; i < tabs.Length; i++)
            {
                if (GUILayout.Button(tabs[i], GUILayout.Height(30)))
                {
                    selectedTab = i; // 선택된 탭 변경
                }
            }
            GUILayout.EndVertical();

            // 우측 내용 영역
            GUILayout.BeginVertical();
            GUILayout.Label($"Current Tab: {tabs[selectedTab]}", GUILayout.Height(30));

            // 선택된 탭의 셀 내용 표시
            GUILayout.BeginVertical(); // 세로 방향 레이아웃 시작

            for (int i = 0; i < cellNames.Length; i++)
            {
                if (GUILayout.Button(cellNames[i], GUILayout.Width(30),GUILayout.Height(30)))
                {
                    Debug.Log($"Clicked: {cellNames[i]}");
                    HandleCellClick(i); // 셀 클릭 처리
                }
            }

            GUILayout.EndVertical(); // 세로 방향 레이아웃 종료

            GUILayout.EndVertical();

            GUILayout.EndHorizontal(); // 수평 레이아웃 종료
            
        }

        private void HandleCellClick(int cellIndex)
        {
            // 클릭된 셀의 동작 처리
            Debug.Log($"Handling Cell {cellIndex + 1}");
        }
    }
}