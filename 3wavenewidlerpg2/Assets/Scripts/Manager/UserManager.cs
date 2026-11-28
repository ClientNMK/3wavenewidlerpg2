using System;
using UnityEngine;
using BackEnd;
using LitJson;

// 유저 저장 데이터 (뒤끝 게임 정보 테이블 UserProfileData, 내 데이터 한 줄)
// 지금은 방치 보상에 필요한 마지막 접속 시각·최고 스테이지만 저장
public class UserManager : Singleton<UserManager>
{
    const string TableName = "UserProfileData";
    const float SaveInterval = 60f;     // 접속 중 마지막 접속 시각을 이 간격으로 저장 (강제 종료 대비)

    string rowInDate = string.Empty;
    public long LastAccessTime { get; private set; } = 0;  // 서버 시각 Unix 초
    public int MaxStageId { get; private set; } = 1;
    public int HuntStageId { get; private set; } = 0;    // 사냥터 (0 = 없음)
    public bool IsHuntSaved { get; private set; } = false; // 저장 버튼으로 정했으면 자동(마지막으로 내려간 곳)은 멈춤

    float saveTimer = 0f;
    public bool IsLoaded { get; private set; } = false;

    // 타이틀에서 차트를 다 불러온 뒤. 처음이면 한 줄 만들고 지금 시각으로 시작
    public void Load(Action _onComplete)
    {
        BackendManager.Instance.BackendGameData.GetMyData(TableName, callback =>
        {
            // 실패하면 오류 팝업 (뒤끝 콘솔에 UserProfile 테이블이 없을 때 등)
            if (!callback.IsSuccess())
            {
                BackendManager.Instance.BackendError.ShowErrPopup(callback);
                return;
            }
            JsonData rows = callback.FlattenRows();
            if (rows.Count == 0)
            {
                LastAccessTime = TimeManager.Instance.GetCurrentTimeStamp();
                BackendManager.Instance.BackendGameData.InsertGameData(TableName, GetParam(), insert =>
                {
                    rowInDate = insert.GetInDate();
                    IsLoaded = true;
                    _onComplete();
                });
                return;
            }
            JsonData row = rows[0];
            rowInDate = row["inDate"].ToString();
            LastAccessTime = GameUtils.GameUtils.TryParse<long>("LastAccessTime", row);
            MaxStageId = Mathf.Max(1, GameUtils.GameUtils.TryParse<int>("MaxStageId", row));
            HuntStageId = GameUtils.GameUtils.TryParse<int>("HuntStageId", row);
            IsHuntSaved = GameUtils.GameUtils.TryParse<bool>("IsHuntSaved", row);
            IsLoaded = true;
            _onComplete();
        });
    }

    Param GetParam()
    {
        Param param = new Param();
        param.Add("LastAccessTime", LastAccessTime);
        param.Add("MaxStageId", MaxStageId);
        param.Add("HuntStageId", HuntStageId);
        param.Add("IsHuntSaved", IsHuntSaved);
        return param;
    }

    // 지금 시각을 마지막 접속 시각으로 저장
    public void SaveAccessTime()
    {
        LastAccessTime = TimeManager.Instance.GetCurrentTimeStamp();
        Save();
    }

    public void SetMaxStage(int _stageId)
    {
        if (_stageId <= MaxStageId)
            return;
        MaxStageId = _stageId;
        SaveAccessTime();
    }

    public void SetHuntStage(int _stageId, bool _isSaved)
    {
        HuntStageId = _stageId;
        IsHuntSaved |= _isSaved;
        Save();
    }

    void Save()
    {
        saveTimer = 0f;
        BackendManager.Instance.BackendGameData.UpdateGameData(TableName, rowInDate, Backend.UserInDate, GetParam());
    }

    private void Update()
    {
        if (!IsLoaded || GameManager.Instance.gameType != GameType.InGame)
            return;

        saveTimer += Time.unscaledDeltaTime;
        if (saveTimer >= SaveInterval)
            SaveAccessTime();
    }

    // 백그라운드로 가거나 끌 때 마지막 접속 시각 저장
    private void OnApplicationPause(bool _pause)
    {
        if (_pause && IsLoaded)
            SaveAccessTime();
    }

    override protected void OnApplicationQuit()
    {
        if (IsLoaded)
            SaveAccessTime();
        base.OnApplicationQuit();
    }
}
