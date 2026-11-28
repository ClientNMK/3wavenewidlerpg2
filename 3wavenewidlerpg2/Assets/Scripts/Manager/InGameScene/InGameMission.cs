using System.Collections.Generic;
using UnityEngine;

// 미션 진행 (MissionChart, 저장 없이 이번 플레이만). 일일은 매일 0시, 주간은 월요일 0시(KST)에 초기화
public class InGameMission
{
    readonly Dictionary<int, double> progress = new();     // Mission_Id별
    readonly HashSet<int> claimed = new();                  // 받은 일일·주간 미션 (반복은 받을 때 쌓인 수에서 뺌)
    long dailyResetTime = 0;
    long weeklyResetTime = 0;

    public long DailyResetTime { get { return dailyResetTime; } }
    public long WeeklyResetTime { get { return weeklyResetTime; } }

    public void Start()
    {
        SetResetTimes();
        Add(MissionProgressType.Login, 1);
    }

    void SetResetTimes()
    {
        System.DateTime now = TimeManager.Instance.GetCurrentDateTime();
        dailyResetTime = TimeManager.Instance.GetNextDailyResetTimeStamp(now);
        weeklyResetTime = TimeManager.Instance.GetNextWeeklyResetTimeStamp(now);
    }

    public void Update(float _deltaTime)
    {
        Add(MissionProgressType.PlayTime, _deltaTime);
        long now = TimeManager.Instance.GetCurrentTimeStamp();
        bool isDaily = now >= dailyResetTime;
        bool isWeekly = now >= weeklyResetTime;
        if (!isDaily && !isWeekly)
            return;

        foreach (MissionData mission in DataManager.Instance.MissionData.GetMissions())
            if ((isDaily && mission.Reset_Type == MissionResetType.Daily) || (isWeekly && mission.Reset_Type == MissionResetType.Weekly))
            {
                progress.Remove(mission.Mission_Id);
                claimed.Remove(mission.Mission_Id);
            }
        SetResetTimes();
        // 날이 바뀌면 다시 로그인한 것으로
        Add(MissionProgressType.Login, 1);
    }

    public void Add(MissionProgressType _type, double _value)
    {
        foreach (MissionData mission in DataManager.Instance.MissionData.GetMissions())
            if (mission.Progress_Type == _type && !claimed.Contains(mission.Mission_Id))
                progress[mission.Mission_Id] = GetProgress(mission) + _value;
    }

    public double GetProgress(MissionData _mission)
    {
        return progress.TryGetValue(_mission.Mission_Id, out double value) ? value : 0;
    }

    public bool IsClaimed(MissionData _mission)
    {
        return claimed.Contains(_mission.Mission_Id);
    }

    // 받을 수 있는 몫: 일일·주간은 1, 반복은 쌓인 수 ÷ 목표(내림)
    public int GetClaimCount(MissionData _mission)
    {
        if (IsClaimed(_mission))
            return 0;
        int count = (int)(GetProgress(_mission) / _mission.Complete_Value);
        return _mission.Reset_Type == MissionResetType.Repeat ? count : Mathf.Min(count, 1);
    }

    // 받은 몫 수를 돌려줌. 반복은 남은 수부터 다시 셈, 일일을 받으면 '일일 미션 N개 완료' 진행
    public int Claim(MissionData _mission)
    {
        int count = GetClaimCount(_mission);
        if (_mission.Reset_Type == MissionResetType.Repeat)
            progress[_mission.Mission_Id] = GetProgress(_mission) - count * _mission.Complete_Value;
        else
            claimed.Add(_mission.Mission_Id);
        if (_mission.Reset_Type == MissionResetType.Daily)
            Add(MissionProgressType.DailyComplete, 1);
        return count;
    }
}
