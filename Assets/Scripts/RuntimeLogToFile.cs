// Assets/Scripts/RuntimeLogToFile.cs
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class RuntimeLogToFile : MonoBehaviour
{
    [SerializeField]
    private bool isLog = true;

    [SerializeField]
    private bool isWarning = true;

    [SerializeField]
    private bool isError = true;

    private string fileName;
    private string filePath;


    public void Awake()
    {
        DontDestroyOnLoad(this);

        // 오늘 날짜로 파일의 이름을 결정
        fileName = System.DateTime.Now.ToString("yyyy-MM-dd");
    }


    private void Start()
    {

        // 생성될 파일의 경로설정
#if UNITY_EDITOR
        filePath = Application.dataPath;

#elif UNITY_ANDROID
        filePath = Application.persistentDataPath;

#endif
    }


    private void OnEnable()
    {
        Application.logMessageReceived += LogToTxt;
    }


    private void OnDisable()
    {
        Application.logMessageReceived -= LogToTxt;
    }


    public void LogToTxt(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            if (!isLog)
                return;
        }
        else if (type == LogType.Warning)
        {
            if (!isWarning)
                return;
        }
        else if (type == LogType.Error)
        {
            if (!isError)
                return;
        }

        // 로그 메세지를 텍스트 파일에 저장
        // 파일이 없다면 새롭게 생성
        using (StreamWriter sw = new StreamWriter(Path.Combine(filePath, fileName), true))
        {
            sw.WriteLine($"[{System.DateTime.Now}] {logString} \n");
        }
    }
}
