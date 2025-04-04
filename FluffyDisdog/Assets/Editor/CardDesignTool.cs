using System.Collections.Generic;
using System.Linq;
using FluffyDisdog;
using FluffyDisdog.Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CardDesignTool : EditorWindow
    {
        private string[] cellNames; // 셀의 이름 또는 데이터를 담을 배열
        private int numCells = 10;  // 셀의 개수

        private ToolType[] tabs;
        private ToolType selectedTab = ToolType.None; // 현재 선택된 탭
        private string[][] tabContent; // 탭별 내용

        private Dictionary<ToolType, ToolData> _toolDatas;

        private ToolTable table;

        private ToolData currentData;
        
        [MenuItem("LevelDesign/Card")]
        static void Open()
        {
            var window = GetWindow<CardDesignTool>();
            window.Init(10);
        }

        private void Init(int n = 1)
        {
            numCells = n;
            
            table = AssetDatabase.LoadAssetAtPath<ToolTable>("Assets/DataTable/ToolTable.asset");

            if (table == null)
            {
                return;
            }

            _toolDatas = table.TryCache();

            tabs = new ToolType[(int)ToolType.MAX];
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i] = (ToolType)i;
            }

            selectedTab = tabs[0];
            
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

        private int currentSelectId = -1;
        private int currentRatio = 0;
        private bool currentInteractable = true;
        private int[] ratioTemp;
        private bool[] interactableTemp;
        
        int newWidth=1;
        int newHeight = 1;
        private bool isCenter = false;

        private ToolTag currentTag;
        private bool needSync = false;
        
        private ToolAdditionalOption currentOption;
        private int currentOptionValue = 0;
        private Vector2 scrollPosition;
        void OnGUI()
        {
            //currentSelectId = -1;
            GUILayout.BeginHorizontal(); // 수평 레이아웃 시작

            // 좌측 탭 영역
            GUILayout.BeginVertical(GUILayout.Width(100)); // 탭의 넓이를 고정
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(200), GUILayout.Height(500));
            for (int i = 0; i < tabs.Length; i++)
            {
                if (GUILayout.Button(tabs[i].ToString(), GUILayout.Height(30)))
                {
                    selectedTab = tabs[i]; // 선택된 탭
                    currentSelectId = -1;
                    newWidth = 1;
                    newHeight = 1;
                    needSync = false;
                }
            }
            GUILayout.EndScrollView();

            if (currentData==null || selectedTab != currentData.type || needSync)
            {
                if (_toolDatas.TryGetValue(selectedTab, out var ret))
                    currentData = ret.Copy();
                else currentData = null;

                if (currentData != null)
                {
                    ratioTemp = currentData?.GetRatioValues();
                    interactableTemp = currentData?.GetInteractable();
                    currentTag = currentData.tag;
                    currentOption = currentData.option;
                    currentOptionValue = currentData.optionValue;
                }

                needSync = false;
            }
            
            
            
            GUILayout.EndVertical();

            if (currentData != null)
            {
                // 우측 내용 영역
                GUILayout.BeginVertical(GUILayout.Width(500));
                GUILayout.Label($"Current Tab: {selectedTab}", GUILayout.Height(30));

                GUILayout.BeginVertical(); // 세로 방향 레이아웃 시작
                
                newWidth = EditorGUILayout.IntField("재설정할 가로폭", newWidth);
                newHeight = EditorGUILayout.IntField("재설정할 세로폭", newHeight);
                    
                if (GUILayout.Button("범위 재설정",
                    GUILayout.Width(100), GUILayout.Height(60)))
                {
                    newWidth = Mathf.Min(8, Mathf.Max(1, newWidth));
                    newHeight = Mathf.Min(8, Mathf.Max(1, newHeight));
                    currentData = new ToolData()
                    {
                        cellHeight = newHeight,
                        cellWidth = newWidth,
                        Center = 0,
                        ratio = new int[newHeight * newWidth],
                        interact = new bool[newHeight * newWidth],
                        type = selectedTab
                    };
                    ratioTemp = currentData?.GetRatioValues();
                    interactableTemp = currentData?.GetInteractable();
                    _toolDatas[selectedTab] = currentData;
                    table.SetData(_toolDatas.Values.ToArray());
                    needSync = true;
                    
                    EditorUtility.SetDirty(table);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                
                GUILayout.Space(20);
                currentTag =  (ToolTag)EditorGUILayout.EnumFlagsField("태그 설정", currentTag);
                currentOption =  (ToolAdditionalOption)EditorGUILayout.EnumPopup("추가 옵션", currentOption);
                currentOptionValue = EditorGUILayout.IntField("추가 옵션 값", currentOptionValue);
                GUILayout.Space(30);
            
                for (int i = 0; i < currentData.cellHeight; i++)
                {
                    GUILayout.BeginHorizontal();

                    for (int j = 0; j < currentData.cellWidth; j++)
                    {
                        GUIStyle bg = new GUIStyle(GUI.skin.button);
                        bg.normal.textColor = Color.black;
                        bool isC = currentData.Center == i * currentData.cellWidth + j;

                        if (isC)
                        {
                            bg.normal.background = MakeBackgroundTexture(60, 60, 
                                Color.yellow );
                        }
                        else if(currentData.GetInteractable(j, i))
                            bg.normal.background = MakeBackgroundTexture(60, 60, 
                                Color.gray );
                        else
                        {
                            bg.normal.background = MakeBackgroundTexture(60, 60, 
                                Color.white );
                        }
                        
                        if (GUILayout.Button(ratioTemp[i * currentData.cellWidth + j].ToString(),bg,GUILayout.Width(60), GUILayout.Height(60)))
                        {
                            currentSelectId = i * currentData.cellWidth + j;
                            currentRatio = ratioTemp[currentSelectId];
                            currentInteractable = interactableTemp[currentSelectId];
                            isCenter = currentSelectId == currentData.Center;
                        }
                    }
                
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical(); // 세로 방향 레이아웃 종료
                GUILayout.Space(30);
                if (currentSelectId > -1)
                {
                    GUILayout.BeginVertical(); // 세로 방향 레이아웃 시작
                    
                    if (GUILayout.Button("Revert",GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        ratioTemp = currentData.GetRatioValues();
                        currentRatio = ratioTemp[currentSelectId];
                        currentInteractable = interactableTemp[currentSelectId];
                        isCenter = currentSelectId == currentData.Center;
                    }

                    currentRatio= EditorGUILayout.IntSlider("파괴확률",currentRatio, 0, 100);
                    ratioTemp[currentSelectId] = currentRatio;
                    currentInteractable = EditorGUILayout.Toggle("타격판정 여부", currentInteractable);
                    interactableTemp[currentSelectId] = currentInteractable;
                    isCenter = EditorGUILayout.Toggle("중심점?", isCenter);
                    if(isCenter)
                       currentData.Center = currentSelectId;
                    
                    GUILayout.Space(30);
                    GUILayout.EndVertical(); // 세로 방향 레이아웃 종료
                    needSync = true;
                    
                }
                if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(30)))
                {
                    currentData.ratio = ratioTemp;
                    currentData.interact = interactableTemp;
                    currentData.tag = currentTag;
                    currentData.option = currentOption;
                    currentData.optionValue = currentOptionValue;
                    _toolDatas[selectedTab] = currentData;
                    table.SetData(_toolDatas.Values.ToArray());
                    
                    EditorUtility.SetDirty(table);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    needSync = true;
                }

                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginVertical(GUILayout.Width(500));
                
                if (GUILayout.Button("Make",GUILayout.Width(100), GUILayout.Height(30)))
                {
                    currentData = new ToolData()
                    {
                        cellHeight = 1,
                        cellWidth = 1,
                        Center = 0,
                        ratio = new int[1] {100},
                        type = selectedTab
                    };
                    _toolDatas.TryAdd(selectedTab, currentData);
                    table.SetData(_toolDatas.Values.ToArray());
                    
                    needSync = true;
                    
                    EditorUtility.SetDirty(table);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                
                GUILayout.EndVertical();
            }
            

            GUILayout.EndHorizontal(); // 수평 레이아웃 종료
            
        }

        private void HandleCellClick(int cellIndex)
        {
            // 클릭된 셀의 동작 처리
            Debug.Log($"Handling Cell {cellIndex + 1}");
        }
        
        
        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D backgroundTexture = new Texture2D(width, height);

            backgroundTexture.SetPixels(pixels);
            backgroundTexture.Apply();

            return backgroundTexture;
        }
    }
}