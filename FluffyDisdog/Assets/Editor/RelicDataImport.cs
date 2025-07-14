using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ExcelDataReader;
using FluffyDisdog.Data.RelicData;
using UnityEditor.AddressableAssets;

namespace Editor
{
    public class RelicDataImport : EditorWindow
    {
        //private BaseTable scrip=null;
        
        [MenuItem("Window/Excel Importer/Relic Data")]
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

                RelicDataTable tableAsset = AssetDatabase.LoadAssetAtPath<RelicDataTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<RelicDataTable>();
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
                        RelicData[] baseArr = new RelicData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            string data1 = rows[j][0].ToString();
                            string data2 = rows[j][3].ToString();
                            string data3 = rows[j][4].ToString();

                            int relicId = int.Parse(data1.Remove(0, 5));
                            float value1 = float.Parse(data2);
                            float value2 = float.Parse(data3);

                            baseArr[j - 1] = new RelicData(relicId, new[] { value1, value2 }, data1);
                        }

                        tableAsset.SetRelicData(baseArr);
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
                    entry.address = "RelicTable";
                    Debug.Log("Addressables에 RelicTable 등록됨");
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