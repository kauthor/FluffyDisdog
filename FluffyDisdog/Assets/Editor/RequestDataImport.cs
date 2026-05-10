using System.IO;
using ExcelDataReader;
using FluffyDisdog.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Editor
{
    public class RequestDataImport:EditorWindow
    {
        [MenuItem("Window/Excel Importer/Request Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(RequestDataImport));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Request Table Export"))
            {
                string assetPath = "Assets/DataTable/RequestTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/11.RequestData.xlsx";

                RequestTable tableAsset = AssetDatabase.LoadAssetAtPath<RequestTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<RequestTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 RequestTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 RequestTable.asset 불러와 덮어씀");
                }

                // 엑셀 파일 읽기
                using (var stream = File.Open(pathProj + excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        var rows = result.Tables[i].Rows;
                        RequestData[] baseArr = new RequestData[rows.Count - 1];

                        for (int j = 1; j < rows.Count; j++)
                        {
                            int data1 =  int.Parse(rows[j][0].ToString());
                            int data2 = int.Parse(rows[j][1].ToString());
                            int data3 = int.Parse(rows[j][2].ToString());
                            int data4 = int.Parse(rows[j][3].ToString());
                            int data5 = int.Parse(rows[j][4].ToString());
                            int data6 = int.Parse(rows[j][5].ToString());
                            int data7 = int.Parse(rows[j][6].ToString());
                            int data8 = int.Parse(rows[j][7].ToString());
                            int data9 = int.Parse(rows[j][8].ToString());
                            int data10 = int.Parse(rows[j][9].ToString());
                            int data11 = int.Parse(rows[j][10].ToString());
                                

                            baseArr[j - 1] = new RequestData()
                            {
                                id = data1,
                                requestGrade = data2,
                                cost = data3,
                                maxInvest = data4,
                                successRate = data5,
                                successRatePerVisit = data6,
                                successRateInvest = data7,
                                jackpotRate = data8,
                                jackpotRateInvest = data9,
                                failBoxId = data10,
                                successBoxId = data11,
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
                    entry.address = "RequestTable";
                    Debug.Log("Addressables에 RequestTable 등록됨");
                }
                else
                {
                    Debug.Log("Addressables 이미 등록됨");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif

                Debug.Log("RequestTable 임포트 완료");
            }
        }
    }
}