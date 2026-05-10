using System.IO;
using ExcelDataReader;
using FluffyDisdog.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class ShopItemDataImport:EditorWindow
    {
        [MenuItem("Window/Excel Importer/Shop Item Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(ShopItemDataImport));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("ShopItem Table Export"))
            {
                string assetPath = "Assets/DataTable/ShopItemTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/08.ShopItemData.xlsx";

                ShopItemTable tableAsset = AssetDatabase.LoadAssetAtPath<ShopItemTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<ShopItemTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 ShopItemTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 ShopItemTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        ShopItemData[] baseArr = new ShopItemData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            int data1 =  int.Parse(rows[j][0].ToString());
                            int data2 = int.Parse(rows[j][1].ToString());
                            string data3 = rows[j][2].ToString();
                            int data4 = int.Parse( rows[j][3].ToString());
                            int data5 = int.Parse( rows[j][4].ToString());

                            baseArr[j - 1] = new ShopItemData()
                            {
                                id = data1,
                                itemType = data2,
                                itemId = data3,
                                costMin = data4,
                                costMax = data5,
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
                    entry.address = "ShopItemTable";
                    Debug.Log("Addressables에 ShopItemTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif

                Debug.Log("ShopItemTable 임포트 완료");
            }
        }
    }
}