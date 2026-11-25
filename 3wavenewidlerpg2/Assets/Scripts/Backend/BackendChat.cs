using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BackEnd;
using BackndChat;
using LitJson;

public class PlayerInfo
{
    // 플레이어 이름
    public string GamerName = string.Empty;
    // 플레이어 아바타 이름
    public string Avatar = string.Empty;
}

public enum ChatType
{
    Public = 1,
    Guild = 2,
}

public class BackendChat : IChatClientListener
{
    private ChatClient ChatClient = null;

    private ChannelInfo channelInfo = null;
    public ChannelInfo ChannelInfo { get { return channelInfo; } }
    private ChannelInfo guildChannelInfo;
    public ChannelInfo GuildChannelInfo { get { return guildChannelInfo; } }

    public Action<ChannelInfo> OnJoinChannelCallback = null;
    public Action<ChannelInfo> OnLeaveChannelCallback = null;
    public Action OnJoinChannelPlayerCallback = null;
    public Action OnLeaveChannelPlayerCallback = null;
    public Action<MessageInfo> OnChatMessageCallback = null;
    public Action<WhisperMessageInfo> OnWhisperMessageCallback = null;
    public Action<MessageInfo> OnHideMessageCallback = null;
    public Action<MessageInfo> OnDeleteMessageCallback = null;
    public Action<SUCCESS_MESSAGE, object> OnReportCallback = null;

    bool isJoinOpenChannel = false;
    bool isJoinGuildChannel = false;

    bool bInitialzed = false;
    public void Init()
    {
        if (bInitialzed)
            return;

        OnJoinChannelCallback = null;
        OnLeaveChannelCallback = null;
        OnJoinChannelPlayerCallback = null;
        OnLeaveChannelPlayerCallback = null;
        OnChatMessageCallback = null;
        OnWhisperMessageCallback = null;
        OnHideMessageCallback = null;
        OnDeleteMessageCallback = null;
        OnReportCallback = null;

        isJoinOpenChannel = false;
        isJoinGuildChannel = false;

        bInitialzed = true;
    }
    public void Setup()
    {
        if (!bInitialzed)
            Init();

        // xxxx-xxxx-xxxxxx-xxxxxxx - 채팅 인증키
        // ChatClient = new ChatClient(this, "default");

        // 인스펙터를 설정하지 않았을 경우, 아래와 같이 인자값으로 Chat UUID를 등록할 수 있습니다.
        ChatClient = new ChatClient(this, new ChatClientArguments
        {
            UUID = string.Empty, // 새 게임 Chat UUID 입력 필요
            Avatar = "default",
        });
    }

    public void BlockUser(string nickname, Action<bool> callback)
    {
        
    }
    public bool UnblockUser(string nickname)
    {
        return false;
    }
    public void ReportUser(ulong index, string tag, string msg, string reason, Action<bool> complete)
    {
        if (ChatClient == null)
        {
            complete?.Invoke(false);
            return;
        }

        // 콘솔에서 지정한 신고 키워드와 사유를 얻어 올 수 있습니다. 현재 유저의 언어 기반으로 정보를 얻어 옵니다.
        // Dictionary<string, string> reasons = ChatClient.GetReportReasons()

        // 신고 함수 입니다. 메시지 인덱스, 메시지 태그, 신고 키워드, 신고 사유를 넣어서 보내 줍니다.
        ChatClient.SendReportChatMessage(index, tag, msg, reason);
        complete?.Invoke(true);
    }

    public string[] GetBlockUserList()
    {
        List<string> blockedPlayers = ChatClient.GetBlockGamers();

        if (blockedPlayers == null || blockedPlayers.Count <= 0)
            return null;

        string[] blockList = new string[blockedPlayers.Count];
        for (int i = 0; i < blockedPlayers.Count; i++)
        {
            blockList[i] = blockedPlayers[i].ToString();
        }
        return blockList;
    }

    // 프라이빗 채널 타입 만 SDK에서 채널을 생성 하실 수 있습니다.
    // 다른 채널 타입은 콘솔을 이용해 주시길 바랍니다.
    // 프라이빗 채널을 생성 하는 함수 입니다. 채널 그룹, 채널 번호, 채널 이름, 최대 인원, 비밀번호을 넣어서 보내 줍니다.
    // 비밀번호를 넣지 않을 경우 공개 채널로 생성 됩니다.
    // 채널 번호를 0으로 보낼 경우 서버에서 자동으로 채널 번호를 부여 합니다.
    public void SendCreatePrivateChannel(string channelGroup, ulong channelNumber = 0, string channelName = "default", uint maxCount = 50, string password = "")
    {
        // ChatClient.SendCreatePrivateChannel("guild", 1, "guildname", 100, "");
        ChatClient.SendCreatePrivateChannel(channelGroup, channelNumber, channelName, maxCount, password);
    }
    // 오픈 채널 타입의 채널 입장 함수 입니다. 채널 그룹, 채널 이름을 넣어서 보내 줍니다.
    public void SendJoinOpenChannel(string channelGroup, string channelName)
    {
        ChatClient.SendJoinOpenChannel(channelGroup, channelName);
        Debug.Log("SendJoinChannel");
    }
    // 프라이빗 채널 타입의 채널 입장 함수 입니다. 채널 그룹, 채널 번호를 넣어서 보내 줍니다.
    public void SendJoinPrivateChannel(string channelGroup, ulong channelNumber, string password = "")
    {
        // ChatClient.SendJoinPrivateChannel("guild", 1);
        ChatClient.SendJoinPrivateChannel(channelGroup, channelNumber, password);
    }
    // 채널에서 나가는 함수 입니다. 채널 그룹, 채널 이름, 채널 번호를 넣어서 보내 줍니다.
    public void SendLeaveChannel(string channelGroup, string channelName, ulong channelNumber = 0)
    {
        // ChatClient.SendLeaveChannel("guild", "guildname", 1);
        ChatClient.SendLeaveChannel(channelGroup, channelName, channelNumber);
    }

    // 뒤끝 베이스 길드 기능을 사용하고 계신 경우에만 사용 가능한 길드 채널 관련 함수 입니다.
    // 길드 생성(CreateGuildV3)후 아래의 코드를 호출 해 주셔야 합니다.
    // 길드 가입 수락 처리(OnApprovedGuildJoin)후 아래의 코드를 호출 해 주셔야 합니다.
    // 길드 채널 입장 함수 입니다.
    public void SendJoinGuildChannel()
    {
        ChatClient.SendJoinGuildChannel();
    }
    // 길드 탈퇴(WithdrawGuildV3)후에는 아래의 코드를 호출 해 주셔야 합니다.
    // 길드 채널에서 나가는 함수 입니다.
    public void SendLeaveGuildChannel()
    {
        ChatClient.SendLeaveGuildChannel();
    }

    // 채팅 메세지를 입력 하는 함수 입니다. 채널 그룹, 채널 이름, 채널 번호, 메세지를 넣어서 보내 줍니다.
    public void SendChatMessage(string channelGroup, string channelName, ulong channelNumber, string message)
    {
        ChatClient.SendChatMessage(channelGroup, channelName, channelNumber, message);
    }
    // 귓속말을 입력 하는 함수입니다. 유저 이름(귓속말 보낼), 메세지를 넣어서 보내 줍니다.
    public void SendWhisperMessage(string gamerName, string message)
    {
        ChatClient.SendWhisperMessage(gamerName, message);
    }

    // 콘솔에서 지정한 신고 키워드와 사유를 얻어 올 수 있습니다. 현재 유저의 언어 기반으로 정보를 얻어 옵니다.
    public Dictionary<string, string> GetReportReasons()
    {
        // 키워드, 사유
        return ChatClient.GetReportReasons();
    }

    // 신고 함수 입니다. 메세지 인덱스, 메세지 태그, 신고 키워드, 신고 사유를 넣어서 보내 줍니다.
    public void SendReportChatMessage(ulong index, string tag, string keyword, string reason)
    {
        ChatClient.SendReportChatMessage(index, tag, keyword, reason);
    }

    public bool IsOpenChatConnected => isJoinOpenChannel;
    public bool IsGuildChatConnected => isJoinGuildChannel;

    public void Update()
    {
        if (ChatClient != null)
            ChatClient?.Update();
    }

    #region Callback

    // 채널 입장 시 오는 콜백 함수 입니다. 채널 정보를 넘겨 줍니다.
    public void OnJoinChannel(ChannelInfo _channelInfo)
    {
        if (_channelInfo.ChannelName.Equals("guild"))
        {
            isJoinGuildChannel = true;
            guildChannelInfo = _channelInfo;
        }
        else
        {
            isJoinOpenChannel = true;
            channelInfo = _channelInfo;
        }

        OnJoinChannelCallback?.Invoke(_channelInfo);
    }

    // 채널 나갈 시 오는 콜백 함수 입니다. 채널 정보를 넘겨 줍니다.
    public void OnLeaveChannel(ChannelInfo _channelInfo)
    {
        if (_channelInfo.ChannelName.Equals("guild"))
        {
            isJoinGuildChannel = false;
            guildChannelInfo = _channelInfo;
        }
        else
        {
            isJoinOpenChannel = false;
            channelInfo = _channelInfo;
        }

        OnLeaveChannelCallback?.Invoke(_channelInfo);
    }

    // 채널에 새로운 유저가 접속 시 오는 콜백 함수 입니다. 유저 정보를 넘겨 줍니다.
    public void OnJoinChannelPlayer(string channelGroup, string channelName, ulong channelNumber, BackndChat.PlayerInfo player)
    {
        if (OnJoinChannelPlayerCallback != null)
            OnJoinChannelPlayerCallback();
    }

    // 채널에서 유저가 나갈 경우 오는 콜백 함수 입니다. 유저 정보를 넘겨 줍니다.
    public void OnLeaveChannelPlayer(string channelGroup, string channelName, ulong channelNumber, BackndChat.PlayerInfo player)
    {
        if (OnLeaveChannelPlayerCallback != null)
            OnLeaveChannelPlayerCallback();
    }

    // 채팅 메세지가 오는 콜백 함수 입니다. 메세지 정보를 넘겨 줍니다.
    public void OnChatMessage(MessageInfo messageInfo)
    {
        if (OnChatMessageCallback != null)
            OnChatMessageCallback(messageInfo);
        GameUtils.GameUtils.Debug_Log(messageInfo.Message);
    }

    // 귓속말 메세지가 오는 콜백 함수 입니다. 메세지 정보를 넘겨 줍니다.
    public void OnWhisperMessage(WhisperMessageInfo messageInfo)
    {
        if (OnWhisperMessageCallback != null)
            OnWhisperMessageCallback(messageInfo);
        GameUtils.GameUtils.Debug_Log(messageInfo.Message);
    }

    // 콘솔에서 메세지를 숨길 경우 오는 콜백 함수 입니다. 메세지 정보를 넘겨 줍니다.
    public void OnHideMessage(MessageInfo messageInfo)
    {
        if (OnHideMessageCallback != null)
            OnHideMessageCallback(messageInfo);
        GameUtils.GameUtils.Debug_Log(messageInfo.Message);
    }

    // 콘솔에서 메세지를 삭제 할 경우 오는 콜백 함수 입니다. 메세지 정보를 넘겨 줍니다.
    public void OnDeleteMessage(MessageInfo messageInfo)
    {
        if (OnDeleteMessageCallback != null)
            OnDeleteMessageCallback(messageInfo);
        GameUtils.GameUtils.Debug_Log(messageInfo.Message);
    }

    // 번역된 채팅 메세지를 확인할 수 있는 콜백 함수입니다.
    public void OnTranslateMessage(List<MessageInfo> messages)
    {

    }

    // 성공 콜백을 참조 해 주시길 바랍니다.
    public void OnSuccess(SUCCESS_MESSAGE success, object param)
    {
        Debug.LogError(success);
        switch (success)
        {
            case SUCCESS_MESSAGE.REPORT:
                {
                    // 신고 접수가 완료가 되면 오는 콜백 함수 입니다.
                    if (OnReportCallback != null)
                        OnReportCallback(success, param);
                }
                break;
        }
    }
    public void OnError(ERROR_MESSAGE error, object param)
    {
        string message = error.ToString();
        switch (error)
        {
            case ERROR_MESSAGE.CHAT_BAN:
                {
                    ErrorMessageChatBanParam errorMessageChatBanParam = (ErrorMessageChatBanParam)param;
                    if (errorMessageChatBanParam == null) return;

                    var banTime = System.DateTime.Now.AddSeconds(errorMessageChatBanParam.RemainSeconds);
                    message = error.ToString() + " : " + banTime.ToString("yyyy-MM-dd HH:mm:ss") + " 까지";
                    Debug.Log(message);
                    GameManager.Instance.UISystemScene.ShowToastMessagePanel(message);
                }
                break;

            case ERROR_MESSAGE.CHANNEL_FULL:
            case ERROR_MESSAGE.INVALID_PASSWORD:
            case ERROR_MESSAGE.ALREADY_CREATED_CHANNEL:
            case ERROR_MESSAGE.CHANNEL_GROUP_TOO_SHORT:
            case ERROR_MESSAGE.CHANNEL_GROUP_TOO_LONG:
            case ERROR_MESSAGE.CHANNEL_NAME_TOO_SHORT:
            case ERROR_MESSAGE.CHANNEL_NAME_TOO_LONG:
            case ERROR_MESSAGE.DUPLICATE_CHANNEL_GROUP:
            case ERROR_MESSAGE.PASSWORD_TOO_LONG:
            case ERROR_MESSAGE.CHANNEL_GROUP_FILTERED:
            case ERROR_MESSAGE.CHANNEL_NAME_FILTERED:
                {
                    ErrorMessageChannelParam errorMessageChannelParam = (ErrorMessageChannelParam)param;
                    if (errorMessageChannelParam == null) return;
                    message = error.ToString() + " : " + errorMessageChannelParam.ChannelGroup + " / " + errorMessageChannelParam.ChannelName + " / " + errorMessageChannelParam.ChannelNumber;
                    Debug.Log(message);
                }
                break;

            default:
                Debug.Log(message);
                break;
        }
    }
    #endregion

    public void OnDestroy()
    {
        if (ChatClient != null)
            ChatClient?.Dispose();
    }

    public void OnApplicationQuit()
    {
        if (ChatClient != null)
            ChatClient?.Dispose();
    }

    public void OnUpdatePlayerInfo(string channelGroup, string channelName, ulong channelNumber, BackndChat.PlayerInfo player)
    {
        throw new NotImplementedException();
    }

    public void OnChangeGamerName(string oldGamerName, string newGamerName)
    {
        throw new NotImplementedException();
    }
}
