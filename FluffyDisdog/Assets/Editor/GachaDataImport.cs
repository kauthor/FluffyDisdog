using System.IO;
using ExcelDataReader;
using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class GachaDataImport:EditorWindow
    {
        [MenuItem("Window/Excel Importer/Gacha Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(GachaDataImport));
        }

        private void OnGUI()
        {
            //var pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", "");
            //string path =  "/Plan/Table/LiveData/03.RelicData.xlsx";
            
            //scrip = EditorGUILayout.ObjectField("Scriptable Object", scrip, typeof(ScriptableObject), true)
             //   as BaseTable;
            
             //1.base Table
             
            if (GUILayout.Button("Gacha Table Export"))
            {
                string assetPath = "Assets/DataTable/GachaTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/07.GachaData.xlsx";

                GachaTable tableAsset = AssetDatabase.LoadAssetAtPath<GachaTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<GachaTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 GachaTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 GachaTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        GachaData[] baseArr = new GachaData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            string data1 = rows[j][0].ToString();
                            string data2 = rows[j][1].ToString();
                            string data3 = rows[j][2].ToString();
                            string data4 = rows[j][3].ToString();
                            string data5 = rows[j][4].ToString();
                            string data6 = rows[j][5].ToString();
                            string data7 = rows[j][6].ToString();
                            
                            int value1 = int.Parse(data1);
                            int value2 = int.Parse(data2);
                            int value3 = int.Parse(data3);
                            int value4 = int.Parse(data4);
                            int value5 = int.Parse(data5);
                            string value6 = data6;
                            int value7 = int.Parse(data7);
                            

                            baseArr[j - 1] = new GachaData()
                            {
                                id = value1,
                                gachaType = value2,
                                gachaId =  value3,
                                rate = value4,
                                rewardType = value5,
                                rewardValue = value6,
                                rewardCount = value7
                            };
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
                    entry.address = "GachaTable";
                    Debug.Log("Addressables에 GachaTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
        #endif

                Debug.Log("GachaTable 임포트 완료");
            }
        }
    }
}