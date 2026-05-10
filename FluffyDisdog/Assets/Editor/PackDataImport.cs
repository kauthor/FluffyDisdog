using System.IO;
using ExcelDataReader;
using FluffyDisdog.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class PackDataImport:EditorWindow
    {
        [MenuItem("Window/Excel Importer/Pack Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(PackDataImport));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Pack Table Export"))
            {
                string assetPath = "Assets/DataTable/PackTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/06.PackData.xlsx";

                PackTable tableAsset = AssetDatabase.LoadAssetAtPath<PackTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<PackTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 PackTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 PackTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        PackData[] baseArr = new PackData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            string data1 = rows[j][0].ToString();
                            string data2 = rows[j][1].ToString();
                            int data3 = int.Parse( rows[j][2].ToString());
                            int data4 = int.Parse( rows[j][3].ToString());
                            int data5 = int.Parse( rows[j][4].ToString());
                            string data6 = rows[j][5].ToString();

                            baseArr[j - 1] = new PackData()
                            {
                                id = data1,
                                nameId = data2,
                                gachaKey = data3,
                                cardShow = data4,
                                cardPick = data5,
                                packResource = data6,
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
                    entry.address = "PackTable";
                    Debug.Log("Addressables에 PackTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif

                Debug.Log("PackTable 임포트 완료");
            }
        }
    }
}