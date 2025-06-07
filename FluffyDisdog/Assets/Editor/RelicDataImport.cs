using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ExcelDataReader;
using FluffyDisdog.Data.RelicData;

namespace Editor
{
    public class RelicDataImport : EditorWindow
    {
        //private BaseTable scrip=null;
        
        [MenuItem("Window/Excel Importer/Base Data")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(RelicDataImport));
        }

        private void OnGUI()
        {
            var pathProj = Application.dataPath.Replace("FluffyDisdog/Assets", "FluffyDisdog");
            string path =  "/Plan/Table/LiveData/03.RelicData.xlsx";
            
            //scrip = EditorGUILayout.ObjectField("Scriptable Object", scrip, typeof(ScriptableObject), true)
             //   as BaseTable;
            
             //1.base Table
             
            if (GUILayout.Button("Relic Table Export"))
            {
                RelicDataTable baseT = ScriptableObject.CreateInstance<RelicDataTable>();
                //var fields = scrip.GetType().GetField("baseDatas");
                using (var stream = File.Open(pathProj+ path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();

                        
                        //시트 개수만큼 반복
                        for (int i = 0; i < result.Tables.Count; i++)
                        {
                            RelicData[] baseArr = new RelicData[result.Tables[i].Rows.Count-1];
                            //해당 시트의 행데이터(한줄씩)로 반복
                            for (int j = 1; j < result.Tables[i].Rows.Count; j++)
                            {
                                //해당행의 0,1,2 셀의 데이터 파싱
                                string data1 = result.Tables[i].Rows[j][0].ToString();
                                string data2 = result.Tables[i].Rows[j][1].ToString();
                                string data3 = result.Tables[i].Rows[j][2].ToString();
                                
                                int data1Parse = Int32.Parse(data1);
                                int data2Parse = Int32.Parse(data2);
                                int data3Parse = Int32.Parse(data3);
                                
                                float[] values = new float[2]
                                {
                                    data2Parse,data3Parse
                                }; 
                                RelicData newB = new RelicData(data1Parse, values);
                                baseArr[j-1] = newB;
                            }

                            baseT.SetRelicData(baseArr);
                        }
                    }
                }
                
               
                AssetDatabase.CreateAsset(baseT, "Assets/Tables/RelicTable.asset");
                AssetDatabase.SaveAssets();
            }

           
            
        }
    }
}