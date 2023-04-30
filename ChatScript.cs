using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatScript : MonoBehaviour
{
 
	// 定义Chat API的URL
	private string m_ApiUrl = "https://api.openai.com/v1/completions";
    //配置参数
    [SerializeField]private GetOpenAI.PostData m_PostDataSetting;
    //聊天UI层
    [SerializeField]private GameObject m_ChatPanel;
    //输入的信息
    [SerializeField]private InputField m_InputWord;
    //输入的API
    [SerializeField]private InputField m_InputAPI;
    //返回的信息
    [SerializeField]private Text m_TextBack;
    //播放设置
    [SerializeField]private Toggle m_PlayToggle;
    //Vits语音
    [SerializeField]private VITS_Speech m_VITS_Player;
    [SerializeField]private GameObject m_ChatAPIPanel;
    
    //获取API
    public string m_OpenAI_Key="";
    public void OpenAPIPanel()
    {
        m_ChatPanel.SetActive(false);
        m_ChatAPIPanel.SetActive(true);
    }
    //输入API给系统
    public void openAndInputApi()
    {
        if(m_InputAPI.text.Equals(""))
        {
            m_ChatAPIPanel.SetActive(false);
            m_ChatPanel.SetActive(true);
        }
            
        else 
        {
            m_OpenAI_Key=m_InputAPI.text;
            m_ChatAPIPanel.SetActive(false);
            m_ChatPanel.SetActive(true);
        }
    }
   
   //发送信息
    public void SendData()
    {
        if(m_InputWord.text.Equals(""))
            return;

        //记录聊天
        m_ChatHistory.Add(m_InputWord.text);

        string _msg=m_PostDataSetting.prompt+m_lan+" "+m_InputWord.text;
        //发送数据
        StartCoroutine (GetPostData (_msg,CallBack));
        m_InputWord.text="";
        m_TextBack.text="...";

        
    }
    // //回车发送消息
    // [SerializeField]private Button m_button;
    // public void BackspaceSendData()
    // {
    //     if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
    //     {
    //         if (EventSystem.current.currentSelectedGameObject == gameObject)
    //         {
    //             m_button.onClick.Invoke();
    //         }
    //     }
    // }


    //AI回复的信息
    private void CallBack(string _callback){
        _callback=_callback.Trim();
        m_TextBack.text="";
        //开始逐个显示返回的文本
        m_WriteState=true;
        StartCoroutine(SetTextPerWord(_callback));

         //记录聊天
        m_ChatHistory.Add(_callback);

        if(m_PlayToggle.isOn){
            StartCoroutine(Speek(_callback));
        }
       

    }


    // private IEnumerator Speek(string _msg){
    //     yield return new WaitForEndOfFrame();
    //     //播放合成并播放音频
    //     m_VitsPlayer.Speek(_msg);
    // }

	// private IEnumerator GetPostData(string _postWord,System.Action<string> _callback)
	// {
    //     using(UnityWebRequest request = new UnityWebRequest (m_ApiUrl, "POST")){   
    //     GetOpenAI.PostData _postData = new GetOpenAI.PostData
	// 	{
	// 		model = m_PostDataSetting.model,
	// 		prompt = _postWord,
	// 		max_tokens = m_PostDataSetting.max_tokens,
    //         temperature=m_PostDataSetting.temperature,
    //         top_p=m_PostDataSetting.top_p,
    //         frequency_penalty=m_PostDataSetting.frequency_penalty,
    //         presence_penalty=m_PostDataSetting.presence_penalty,
    //         stop=m_PostDataSetting.stop
	// 	};

	// 	string _jsonText = JsonUtility.ToJson (_postData);
	// 	byte[] data = System.Text.Encoding.UTF8.GetBytes (_jsonText);
	// 	request.uploadHandler = (UploadHandler)new UploadHandlerRaw (data);
	// 	request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer ();

	// 	request.SetRequestHeader ("Content-Type","application/json");
	// 	request.SetRequestHeader("Authorization",string.Format("Bearer {0}",m_OpenAI_Key));

	// 	yield return request.SendWebRequest ();

	// 	if (request.responseCode == 200) {
	// 		string _msg = request.downloadHandler.text;
	// 		GetOpenAI.TextCallback _textback = JsonUtility.FromJson<GetOpenAI.TextCallback> (_msg);
	// 		if (_textback!=null && _textback.choices.Count > 0) {
                    
    //             string _backMsg=Regex.Replace(_textback.choices [0].text, @"[\r\n]", "").Replace("？","");
    //             _callback(_backMsg);
	// 		}
		
	// 	}
    //     }

		
	// }
    private IEnumerator Speek(string _msg){
        yield return new WaitForEndOfFrame();
        //播放合成并播放音频
        m_VITS_Player.Speek(_msg);
    }

	private IEnumerator GetPostData(string _postWord,System.Action<string> _callback)
	{
        using(UnityWebRequest request = new UnityWebRequest (m_ApiUrl, "POST")){   
        GetOpenAI.PostData _postData = new GetOpenAI.PostData
		{
			model = m_PostDataSetting.model,
			prompt = _postWord,
			max_tokens = m_PostDataSetting.max_tokens,
            temperature=m_PostDataSetting.temperature,
            top_p=m_PostDataSetting.top_p,
            frequency_penalty=m_PostDataSetting.frequency_penalty,
            presence_penalty=m_PostDataSetting.presence_penalty,
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


    #region 文字逐个显示
    //逐字显示的时间间隔
    [SerializeField]private float m_WordWaitTime=0.2f;
    //是否显示完成
    [SerializeField]private bool m_WriteState=false;
    private IEnumerator SetTextPerWord(string _msg){
        int currentPos=0;
        while(m_WriteState){
            yield return new WaitForSeconds(m_WordWaitTime);
            currentPos++;
            //更新显示的内容
            m_TextBack.text=_msg.Substring(0,currentPos);

            m_WriteState=currentPos<_msg.Length;

        }
    }

    #endregion


    #region 聊天记录
    //保存聊天记录
    [SerializeField]private List<string> m_ChatHistory;
    //缓存已创建的聊天气泡
    [SerializeField]private List<GameObject> m_TempChatBox;
    //聊天记录显示层
    [SerializeField]private GameObject m_HistoryPanel;
    //聊天文本放置的层
    [SerializeField]private RectTransform m_rootTrans;
    //发送聊天气泡
    [SerializeField]private ChatPrefab m_PostChatPrefab;
    //回复的聊天气泡
    [SerializeField]private ChatPrefab m_RobotChatPrefab;
    //滚动条
    [SerializeField]private ScrollRect m_ScroTectObject;
    //获取聊天记录
   
    public void OpenAndGetHistory(){
        m_ChatPanel.SetActive(false);
        m_HistoryPanel.SetActive(true);

        ClearChatBox();
        StartCoroutine(GetHistoryChatInfo());
    }
    //返回
    public void BackChatMode(){
        m_ChatPanel.SetActive(true);
        m_HistoryPanel.SetActive(false);
    }

    //清空已创建的对话框
    private void ClearChatBox(){
        while(m_TempChatBox.Count!=0){
            if(m_TempChatBox[0]){
                Destroy(m_TempChatBox[0].gameObject);
                m_TempChatBox.RemoveAt(0);
            }
        }
        m_TempChatBox.Clear();
    }

    //获取聊天记录列表
    private IEnumerator GetHistoryChatInfo()
    {

        yield return new WaitForEndOfFrame();

       for(int i=0;i<m_ChatHistory.Count;i++){
        if(i%2==0){
            ChatPrefab _sendChat=Instantiate(m_PostChatPrefab,m_rootTrans.transform);
            _sendChat.SetText(m_ChatHistory[i]);
            m_TempChatBox.Add(_sendChat.gameObject);
            continue;
        }

         ChatPrefab _reChat=Instantiate(m_RobotChatPrefab,m_rootTrans.transform);
        _reChat.SetText(m_ChatHistory[i]);
        m_TempChatBox.Add(_reChat.gameObject);
       }

        //重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine(){
        yield return new WaitForEndOfFrame();
         //滚动到最近的消息
        m_ScroTectObject.verticalNormalizedPosition=0;
    }


    #endregion


    #region 切换妹子
    //
    [SerializeField]private GameObject m_LoGirl;
    [SerializeField]private GameObject m_Girl;
    [SerializeField]private string m_lan="使用中文回答";
    //
    public void SetLoGirlShowed(GameObject _settingPanel){
        if(!m_LoGirl.activeSelf)
        {
            m_LoGirl.SetActive(true);
            m_Girl.SetActive(false);
        }
        //m_VitsPlayer.SetSound("zh-CN-XiaoyiNeural");

        _settingPanel.SetActive(false);
    }
    //zh-CN-XiaoxiaoNeural
    public void SetGirlShowed(GameObject _settingPanel){
        if(!m_Girl.activeSelf)
        {
            m_LoGirl.SetActive(false);
            m_Girl.SetActive(true);
        }
         //m_VitsPlayer.SetSound("zh-CN-liaoning-XiaobeiNeural");

        _settingPanel.SetActive(false);
    }

    #endregion


}
