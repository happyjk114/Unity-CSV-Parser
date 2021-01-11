using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public interface IHasPrefabID
{
    string GetPrefabID { get; }
}
public class UnityParser : MonoBehaviour
{

    /// <summary>
    /// 데이터 파싱 함수입니다.
    /// 타입별 처리 int,long,double,float,string 까지 되어있고
    /// 추가타입이나 커스텀 타입은 함수 아래쪽에 더 넣으면 됩니다.
    /// Undo 불가
    /// </summary>
    /// <typeparam name="TargetClass"> 파싱해서 데이터 넣을 클래스.</typeparam>
    /// <param name="nameOfIdColumn">CSV파일에 있는 ID 분류명</param>
    /// <param name="txtDataPath">CSV파일 경로 ex)"Assets/Data/Gun.csv"</param>
    /// <param name="prefabFolder">데이터 넣을 프리팹들이 있는 경로 (Resources폴더 내부에 있어야함 ex)"Guns") </param>
    /// <param name="startDataRow">데이터가 시작되는 행. 기본값 0</param>
    /// <param name="StartDataColumn">데이터가 시작되는 열. 기본값 0</param>
    void ParseData<TargetClass>(string nameOfIdColumn, string txtDataPath, string prefabFolder, int startDataRow = 0, int StartDataColumn = 0) where TargetClass : MonoBehaviour, IHasPrefabID
    {
        //프리팹 정보 가져오고
        var prefabArr = Resources.LoadAll<TargetClass>(prefabFolder);
        if (prefabArr.Length < 1)
        {
            Debug.Log(prefabFolder + "경로에 프리팹이 없습니다.");
            return;
        }

        //프리팹 정보 ID로 저장
        Dictionary<string, TargetClass> prefabDic = new Dictionary<string, TargetClass>();
        for (int i = 0; i < prefabArr.Length; i++)
        {
            if (prefabDic.ContainsKey(prefabArr[i].GetPrefabID))
            {
                Debug.Log("ID 중복됨 : " + prefabArr[i].GetPrefabID);
                continue;
            }
            if (string.IsNullOrEmpty(prefabArr[i].GetPrefabID))
            {
                Debug.Log("해당 프리팹 아이디가 비어있음 : " + prefabArr[i].name);
                continue;
            }
            prefabDic.Add(prefabArr[i].GetPrefabID, prefabArr[i]);
        }

        //CSV데이터 가져옵니다.
        TextAsset txt = AssetDatabase.LoadAssetAtPath<TextAsset>(txtDataPath);
        string[] lines = txt.text.Split('\n');
        string[] classification = lines[startDataRow].Split(','); //분류명

        //몇번째 데이터인지 필드이름으로 저장
        Dictionary<string, int> classificationName2idx = new Dictionary<string, int>();
        int columnIdxOfId = -1;
        for (int i = 0; i < classification.Length; i++)
        {
            if (string.IsNullOrEmpty(classification[i]))//비어있는칸도 빼주고
            {
                continue;
            }
            if (nameOfIdColumn == classification[i]) //ID는 빼주고
            {
                columnIdxOfId = i;
            }
            if (classificationName2idx.ContainsKey(classification[i]))
            {
                Debug.Log("분류명 중복 : " + classification[i]);
                continue;
            }
            classificationName2idx.Add(classification[i], i);
        }
        if (columnIdxOfId == -1)
        {
            Debug.LogError("분류행에 ID를 찾을수 없어 종료합니다.");
            return;
        }



        //실제 데이터 기입.
        for (int i = startDataRow + 1; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split(',');
            if (prefabDic.ContainsKey(datas[columnIdxOfId]) == false)
            {
                Debug.Log("해당 아이디를 가진 프리팹이 없습니다 : " + datas[columnIdxOfId]);
                continue;
            }
            TargetClass tc = prefabDic[datas[columnIdxOfId]];
            Type type = tc.GetType();


            //시작하기전에 필드명으로 필드정보 찾을수 있게 해주고
            System.Reflection.FieldInfo[] fieldInfoArr = type.GetFields();
            Dictionary<string, System.Reflection.FieldInfo> fieldInfoDic = new Dictionary<string, System.Reflection.FieldInfo>();
            for (int j = 0; j < fieldInfoArr.Length; j++)
            {
                if (fieldInfoDic.ContainsKey(fieldInfoArr[j].Name))
                {
                    Debug.Log("해당 필드명이 이미 존재합니다? : " + fieldInfoArr[j].Name);
                    continue;
                }
                fieldInfoDic.Add(fieldInfoArr[j].Name, fieldInfoArr[j]);
            }


            for (int j = StartDataColumn; j < datas.Length; j++)
            {
                if (string.IsNullOrEmpty(datas[j])) //데이터 없는경우
                {
                    continue;
                }
                if (columnIdxOfId == j) //ID열인경우
                {
                    continue;
                }

                if (fieldInfoDic.ContainsKey(classification[j]) == false)
                {
                    Debug.Log("프리팹에 " + classification[j] + "이름의 필드가 없습니다");
                    continue;
                }

                if (fieldInfoDic[classification[j]].FieldType == typeof(int))
                {
                    fieldInfoDic[classification[j]].SetValue(tc, int.Parse(datas[j]));
                }
                else if (fieldInfoDic[classification[j]].FieldType == typeof(long))
                {
                    fieldInfoDic[classification[j]].SetValue(tc, long.Parse(datas[j]));
                }
                else if (fieldInfoDic[classification[j]].FieldType == typeof(double))
                {
                    fieldInfoDic[classification[j]].SetValue(tc, double.Parse(datas[j]));
                }
                else if (fieldInfoDic[classification[j]].FieldType == typeof(Single))
                {
                    fieldInfoDic[classification[j]].SetValue(tc, float.Parse(datas[j]));
                }
                else if (fieldInfoDic[classification[j]].FieldType == typeof(string))
                {
                    fieldInfoDic[classification[j]].SetValue(tc, datas[j]);
                }
                else
                {
                    Debug.Log(fieldInfoDic[classification[j]].FieldType + " : 해당하는 타입변환 코드가 없습니다.");
                }
            }

            PrefabUtility.SavePrefabAsset(tc.gameObject); //변경된 사항을 저장합니다
            Debug.Log(datas[columnIdxOfId] + " : 데이터 기입 완료");
        }
    }
}
