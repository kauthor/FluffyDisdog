using System.IO;
using ExcelDataReader;
using FluffyDisdog.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class BoxItemDataImport:EditorWindow   
    {
        [MenuItem("Window/Excel Importer/Box Item Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(BoxItemDataImport));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Box Item Table Export"))
            {
                string assetPath = "Assets/DataTable/BoxItemTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/14.BoxItemData.xlsx";

                BoxItemTable tableAsset = AssetDatabase.LoadAssetAtPath<BoxItemTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<BoxItemTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 BoxItemTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 BoxItemTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        BoxItemData[] baseArr = new BoxItemData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            int data1 = int.Parse( rows[j][0].ToString());
                            int data2 = int.Parse( rows[j][1].ToString());
                            int data3 = int.Parse( rows[j][2].ToString());
                            int data4 = int.Parse( rows[j][3].ToString());
                            int data5 = int.Parse( rows[j][4].ToString());
                            

                            baseArr[j - 1] = new BoxItemData()
                            {
                                id=data1,
                                boxId = data2,
                                rewardType = data3,
                                rewardValue = data4,
                                rewardCount = data5
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
                    entry.address = "BoxItemTable";
                    Debug.Log("Addressables에 BoxItemTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif

                Debug.Log("BoxItemTable 임포트 완료");
            }
        }
    }
}