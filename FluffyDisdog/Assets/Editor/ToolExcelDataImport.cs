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
            EditorWindow.GetWindow(typeof(ToolExcelDataImport));
        }

        private void OnGUI()
        {
            //var pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", "");
            //string path =  "/Plan/Table/LiveData/03.RelicData.xlsx";
            
            //scrip = EditorGUILayout.ObjectField("Scriptable Object", scrip, typeof(ScriptableObject), true)
             //   as BaseTable;
            
             //1.base Table
             
            if (GUILayout.Button("Card Table Export"))
            {
                string assetPath = "Assets/DataTable/ToolExcelTable.asset";
                string pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", ""); // 너의 환경에 따라 조정 가능
                string excelPath = "/Plan/Table/LiveData/03.CardData.xlsx";

                ToolExcelDataTable tableAsset = AssetDatabase.LoadAssetAtPath<ToolExcelDataTable>(assetPath);

                if (tableAsset == null)
                {
                    tableAsset = ScriptableObject.CreateInstance<ToolExcelDataTable>();
                    AssetDatabase.CreateAsset(tableAsset, assetPath);
                    Debug.Log("새 ToolExcelTable.asset 생성됨");
                }
                else
                {
                    Debug.Log("기존 ToolExcelTable.asset 불러와 덮어씀");
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
                            /*string data1 = rows[j][1].ToString();
                            string data2 = rows[j][2].ToString();
                            string data3 = rows[j][3].ToString();
                            string data4 = rows[j][4].ToString();

                            int type = int.Parse(data1);
                            int tag = int.Parse(data2);
                            int op = int.Parse(data3);
                            int value = int.Parse(data4);

                            baseArr[j - 1] = new ToolExcelData((ToolType)(type), (ToolTag)tag, (ToolAdditionalOption)op, value);*/
                            
                            
                            string data1 = rows[j][0].ToString();
                            string data2 = rows[j][1].ToString();
                            string data3 = rows[j][2].ToString();
                            string data4 = rows[j][3].ToString();
                            string data5 = rows[j][4].ToString();
                            string data6 = rows[j][5].ToString();
                            string data7 = rows[j][6].ToString();
                            string data8 = rows[j][7].ToString();
                            string data9 = rows[j][8].ToString();
                            string data10 = rows[j][9].ToString();
                            string data11 = rows[j][10].ToString();
                            string data12 = rows[j][11].ToString();
                            string data13 = rows[j][12].ToString();
                            string data14 = rows[j][13].ToString();
                            string data15 = rows[j][14].ToString();
                            string data16 = rows[j][15].ToString();
                            string data17 = rows[j][16].ToString();
                            string data18 = rows[j][17].ToString();
                            
                            int id = int.Parse(data1);
                            string key = data2;
                            string localNKey = data3;
                            string localDKey = data4;
                            int cardType = int.Parse(data5);
                            int charType= int.Parse(data6);
                            int upType= int.Parse(data7);
                            string upKey= data8;
                            int rarity= int.Parse(data9);
                            int bit= int.Parse(data10);
                            int tagBit= int.Parse(data11);
                            int mana = 0;//int.Parse(data12);
                            int[] addOpId1 = new int[3]
                            {
                                int.Parse(data12), int.Parse(data13), int.Parse(data14)
                            };
                            int cardDexId= int.Parse(data15);
                            int sort= int.Parse(data16);
                            string gridId= data17;
                            string imgId= data18;

                            baseArr[j - 1] = new ToolExcelData(id, key, localNKey, localDKey, cardType, charType,
                                upType, upKey, rarity, bit, tagBit, mana, addOpId1, cardDexId, sort, gridId, imgId);
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

                Debug.Log("ToolExcelTable 임포트 완료");
            }
        }
    }
}