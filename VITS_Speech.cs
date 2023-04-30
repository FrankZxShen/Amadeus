using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using static VITS_Speech;


public class VITS_Speech : MonoBehaviour
{
    /// <summary>
    /// VITS的api地址
    /// </summary>
    [SerializeField] private string m_VitsApiPath;
    /// <summary>
    /// 设置
    /// </summary>
    public SendDataSetting m_Setting;
    /// <summary>
    /// 音频组件
    /// </summary>
    public AudioSource m_AudioSource;


    public void Speek(string _msg)
    {
        List<object> dataList = new List<object>();
        dataList.Add(_msg);
        dataList.Add(m_Setting.lan.ToString());
        dataList.Add(m_Setting.speeker.ToString());
        dataList.Add(m_Setting.noise_scale);
        dataList.Add(m_Setting.noise_scale_w);
        dataList.Add(m_Setting.length_scale);

        Dictionary<string, List<object>> dataDict = new Dictionary<string, List<object>>();
        dataDict.Add("data", dataList);

        string jsonData = JsonConvert.SerializeObject(dataDict);

        StartCoroutine(GetSpeech(jsonData));
    }

    /// <summary>
    /// VITS合成，获得音频文件
    /// </summary>
    /// <param name="_sendData"></param>
    /// <returns></returns>
    public IEnumerator GetSpeech(string _sendData)
    {
        using (UnityWebRequest request = new UnityWebRequest(m_VitsApiPath, "POST"))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_sendData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                string _msg = request.downloadHandler.text;
                JObject jsonData = JObject.Parse(_msg);
                Debug.Log(_msg);
                //获取到音频的地址
                string name = (string)jsonData["data"][1]["name"];
                StartCoroutine(LoadAudioClip(name));
            }
        }
    }
    /// <summary>
    /// 读取音频并播放
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
   private IEnumerator LoadAudioClip(string filePath)
    {
        string url = "file://" + filePath; // 组成URL
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV); // 发送请求
        yield return www.SendWebRequest(); // 等待请求响应

        if (www.result == UnityWebRequest.Result.Success)
        {
            // 获取音频文件
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            m_AudioSource.clip = clip;
            m_AudioSource.Play(); // 播放音频文件
        }
        else
        {
            Debug.LogError("Load audio clip failed. " + www.error);
        }
    }

    #region 数据定义
    /// <summary>
    /// 合成设置
    /// </summary>
    [System.Serializable]public class SendDataSetting
    {
        /// <summary>
        /// 语言
        /// </summary>
        [Header("语言")]public Lan lan= Lan.中文;
        /// <summary>
        /// 声音
        /// </summary>
        [Header("声音")] public string speeker= "胡桃";
        /// <summary>
        /// 控制感情变化程度
        /// </summary>
        [Header("控制感情变化程度")] public float noise_scale;
        /// <summary>
        /// 控制音素发音长度
        /// </summary>
        [Header("控制音素发音长度")] public float noise_scale_w;
        /// <summary>
        /// 控制整体语速
        /// </summary>
        [Header("控制整体语速")]public float length_scale;
    }

    public enum Lan
    {
        中文,
        日语,
        Chinese,
        japane
    }


    #endregion


}
