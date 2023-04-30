using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class GetOpenAI : MonoBehaviour
{
    //API key
	[SerializeField]private string m_OpenAI_Key="填写你的Key";
	// 定义Chat API的URL
	private string m_ApiUrl = "https://api.openai.com/v1/completions";
    //配置参数
    [SerializeField]private PostData m_PostDataSetting;
    //输入的信息
    [SerializeField]private InputField m_InputWord;
    //聊天文本放置的层
    [SerializeField]private RectTransform m_rootTrans;
    //发送聊天气泡
    [SerializeField]private ChatPrefab m_PostChatPrefab;
    //回复的聊天气泡
    [SerializeField]private ChatPrefab m_RobotChatPrefab;
    //滚动条
    [SerializeField]private ScrollRect m_ScroTectObject;

    //发送信息
    public void SendData()
    {
        if(m_InputWord.text.Equals(""))
            return;

        string _msg=m_InputWord.text;
        ChatPrefab _chat=Instantiate(m_PostChatPrefab,m_rootTrans.transform);
        _chat.SetText(_msg);
        //重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
        StartCoroutine (GetPostData (_msg,CallBack));
        m_InputWord.text="";
    }

    //AI回复的信息
    private void CallBack(string _callback){
        _callback=_callback.Trim();
        ChatPrefab _chat=Instantiate(m_RobotChatPrefab,m_rootTrans.transform);
        _chat.SetText(_callback);
        //重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
       
       StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine(){
        yield return new WaitForEndOfFrame();
         //滚动到最近的消息
        m_ScroTectObject.verticalNormalizedPosition=0;
    }

    //设置AI模型
    public void SetAIModel(Toggle _modelType){
        if(_modelType.isOn){
            m_PostDataSetting.model=_modelType.name;
        }
    }


	[System.Serializable]public class PostData{
		public string model;
		public string prompt;
		public int max_tokens; 
        public float temperature;
        public int top_p;
        public float frequency_penalty;
        public float presence_penalty;
        public string stop;
	}

	private IEnumerator GetPostData(string _postWord,System.Action<string> _callback)
	{

		using(UnityWebRequest request = new UnityWebRequest (m_ApiUrl, "POST")){   
        GetOpenAI.PostData _postData = new GetOpenAI.PostData
		{
			model = m_PostDataSetting.model,
			prompt = _postWord,
			max_tokens = m_PostDataSetting.max_tokens,//最大考虑的token的数量，数值较大可得到更为全面的回答，但速度更慢，上限为4000
            temperature=m_PostDataSetting.temperature,//创造性参数，可理解为人类的想象力，一般只在（0.7-0.9）之间
            top_p=m_PostDataSetting.top_p,//只考虑似然值不超过top.p的token来生成回答
            frequency_penalty=m_PostDataSetting.frequency_penalty,//（-2--2）值越大模型回答语言就更不寻常，更多样和富有创造性，降低词汇重复率，通常为（0.1--1）。(1–frequency_penalty)∗log_probability
            presence_penalty=m_PostDataSetting.presence_penalty,//模型如何根据到目前为止是否出现在文本中来惩罚新token
            stop=m_PostDataSetting.stop
		};

		string _jsonText = JsonUtility.ToJson (_postData);
		byte[] data = System.Text.Encoding.UTF8.GetBytes (_jsonText);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw (data);
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer ();

		request.SetRequestHeader ("Content-Type","application/json");
		request.SetRequestHeader("Authorization",string.Format("Bearer {0}",m_OpenAI_Key));

		yield return request.SendWebRequest ();

		if (request.responseCode == 200) {
			string _msg = request.downloadHandler.text;
			GetOpenAI.TextCallback _textback = JsonUtility.FromJson<GetOpenAI.TextCallback> (_msg);
			if (_textback!=null && _textback.choices.Count > 0) {
                    
                string _backMsg=Regex.Replace(_textback.choices [0].text, @"[\r\n]", "").Replace("？","");
                _callback(_backMsg);
			}
		}
        }
	}

    public void Quit(){
        Application.Quit();
    }

    void Update(){

        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }

	/// <summary>
	/// 返回的信息
	/// </summary>
	[System.Serializable]public class TextCallback{
		public string id;
		public string created;
		public string model;
		public List<TextSample> choices;

		[System.Serializable]public class TextSample{
			public string text;
			public string index;
			public string finish_reason;
		}

	}

}
