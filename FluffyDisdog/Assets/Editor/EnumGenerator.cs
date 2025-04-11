using System;
using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;


namespace Editor
{
    public static class EnumGenerator
    {
        public static void GenerateCardName(string[] enumNames)
        {
            // 사용자 입력 받기


            string assetPath = Application.dataPath + "/Script/TileClass/ToolType.cs";

            string enumdesc = "None=-1,";
            int enumNum = 0;
            foreach (var n in enumNames)
            {
                enumdesc += $"{n}={enumNum++},";
            }

            enumdesc += $"Max={enumNum}";

            // 클래스 내용 생성
            string classContent = $@"
using System; 

namespace FluffyDisdog
{{
    public enum ToolType
    {{
        {enumdesc}
    }}
}}";

            try
            {
                // 디렉터리가 없으면 생성
                if (!Directory.Exists(assetPath))
                {
                    Directory.CreateDirectory(assetPath);
                }

                // 파일 작성
                File.WriteAllText(assetPath, classContent);
                Debug.Log($"\n성공적으로 생성되었습니다: {assetPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}