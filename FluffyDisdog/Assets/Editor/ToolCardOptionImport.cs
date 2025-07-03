using System.IO;
using ExcelDataReader;
using FluffyDisdog;
using FluffyDisdog.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class ToolCardOptionImport:EditorWindow
    {
        [MenuItem("Window/Excel Importer/Tool Card Option Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(ToolCardOptionImport));
        }

        private void OnGUI()
        {
            
             
            if (GUILayout.Button("Option Table Export"))
            {
                string assetPath = "Assets/DataTable/ToolCardOptionTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/03.CardAddData.xlsx";

                ToolCardOptionTable tableAsset = AssetDatabase.LoadAssetAtPath<ToolCardOptionTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<ToolCardOptionTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 ToolCardOptionTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 ToolCardOptionTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        ToolCardOpData[] baseArr = new ToolCardOpData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            string data1 = rows[j][0].ToString();
                            string data2 = rows[j][1].ToString();
                            string data3 = rows[j][2].ToString();
                            string data4 = rows[j][3].ToString();
                            string data5 = rows[j][4].ToString();
                            
                            int id = int.Parse(data1);
                            string desc = data2;
                            int[] val = new int[3]
                            {
                                int.Parse(data3),int.Parse(data4),int.Parse(data5)
                            };
                            
                            ToolCardOpData data = new ToolCardOpData(id, desc, val);
                            baseArr[j-1] = data;
                        }

                        tableAsset.SetData(baseArr);
                    }
                }

                // 에셋 저장
                EditorUtility.SetDirty(tableAsset);
                AssetDatabase.SaveAssets();

        #if UNITY_EDITOR
                // Addressables 자동 등록 (중복 확인 포함)
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                var group = settings.DefaultGroup;
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                var entry = settings.FindAssetEntry(guid);

                if (entry == null)
                {
                    entry = settings.CreateOrMoveEntry(guid, group);
                    entry.address = "ToolCardOptionTable";
                    Debug.Log("Addressables에 ToolCardOptionTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
        #endif

                Debug.Log("ToolCardOptionTable 임포트 완료");
            }
        }
    }
}