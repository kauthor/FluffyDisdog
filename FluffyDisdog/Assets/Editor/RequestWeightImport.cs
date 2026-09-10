using System.IO;
using ExcelDataReader;
using FluffyDisdog.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class RequestWeightImport:EditorWindow
    {
        [MenuItem("Window/Excel Importer/Request Weight Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(RequestWeightImport));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Request Weight Table Export"))
            {
                string assetPath = "Assets/DataTable/RequestWeightTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/12.RequestWeightData.xlsx";

                RequestWeightTable tableAsset = AssetDatabase.LoadAssetAtPath<RequestWeightTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<RequestWeightTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 RequestWeightTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 RequestWeightTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        RequestWeightData[] baseArr = new RequestWeightData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            int data1 = int.Parse( rows[j][0].ToString());
                            int data2 = int.Parse( rows[j][1].ToString());
                            string data3 =rows[j][2].ToString();
                            int data4 = int.Parse( rows[j][3].ToString());
                            int data5 = int.Parse( rows[j][4].ToString());
                            

                            baseArr[j - 1] = new RequestWeightData()
                            {
                                id=data1,
                                groupId = data2,
                                cardKey = data3,
                                gachaId = data4,
                                rate = data5,
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
                    entry.address = "RequestWeightTable";
                    Debug.Log("Addressables에 RequestWeightTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif

                Debug.Log("RequestWeightTable 임포트 완료");
            }
        }
    }
}