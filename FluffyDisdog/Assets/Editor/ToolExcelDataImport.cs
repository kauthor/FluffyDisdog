using System.IO;
using ExcelDataReader;
using FluffyDisdog;
using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class ToolExcelDataImport : EditorWindow
    {
        [MenuItem("Window/Excel Importer/Tool Excel Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(RelicDataImport));
        }

        private void OnGUI()
        {
            //var pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", "");
            //string path =  "/Plan/Table/LiveData/03.RelicData.xlsx";
            
            //scrip = EditorGUILayout.ObjectField("Scriptable Object", scrip, typeof(ScriptableObject), true)
             //   as BaseTable;
            
             //1.base Table
             
            if (GUILayout.Button("Relic Table Export"))
            {
                string assetPath = "Assets/DataTable/RelicTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/03.RelicData.xlsx";

                ToolExcelDataTable tableAsset = AssetDatabase.LoadAssetAtPath<ToolExcelDataTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<ToolExcelDataTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 RelicTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 RelicTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        ToolExcelData[] baseArr = new ToolExcelData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            string data1 = rows[j][1].ToString();
                            string data2 = rows[j][2].ToString();
                            string data3 = rows[j][3].ToString();
                            string data4 = rows[j][4].ToString();

                            int type = int.Parse(data1);
                            int tag = int.Parse(data2);
                            int op = int.Parse(data3);
                            int value = int.Parse(data4);

                            baseArr[j - 1] = new ToolExcelData((ToolType)(type), (ToolTag)tag, (ToolAdditionalOption)op, value);
                        }

                        tableAsset.SetToolExcelData(baseArr);
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
                    entry.address = "ToolExcelTable";
                    Debug.Log("Addressables에 ToolExcelTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
        #endif

                Debug.Log("RelicTable 임포트 완료");
            }
        }
    }
}