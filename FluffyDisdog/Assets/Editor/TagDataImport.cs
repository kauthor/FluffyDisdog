using System.IO;
using ExcelDataReader;
using FluffyDisdog.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class TagDataImport :EditorWindow
    {
        [MenuItem("Window/Excel Importer/Tag Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(TagDataImport));
        }

        private void OnGUI()
        {
            
            if (GUILayout.Button("Tag Table Export"))
            {
                string assetPath = "Assets/DataTable/TagTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); 
                string excelPath = "/Plan/Table/LiveData/05.TagData.xlsx";

                TagTable tableAsset = AssetDatabase.LoadAssetAtPath<TagTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<TagTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 TagTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 TagTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        TagData[] baseArr = new TagData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            string data1 = rows[j][0].ToString();
                            string data2 = rows[j][1].ToString();
                            string data3 = rows[j][2].ToString();
                            string data4 = rows[j][3].ToString();
                            string data5 = rows[j][4].ToString();
                            string data6 = rows[j][5].ToString();
                            string data7 = rows[j][6].ToString();
                            string data8 = rows[j][7].ToString();

                            int relicId = int.Parse(data1);
                            int bit = int.Parse(data2);
                            int[] values = new[]
                            {
                                int.Parse(data6), int.Parse(data6), int.Parse(data6)
                            };

                            baseArr[j - 1] = new TagData(relicId, bit,data3,data4,data5, values);
                        }

                        tableAsset.SetTagData(baseArr);
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
                    entry.address = "TagTable";
                    Debug.Log("Addressables에 TagTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
        #endif

                Debug.Log("TagTable 임포트 완료");
            }
        }
    }
}