using GameUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    [SerializeField] private long currentAppTimestamp = 0L;
    [SerializeField] private long diffTimestamp = 0L;

    [SerializeField] private long currentTimestamp = 0L;
    private DateTime currentDateTime;

    // -------- KST 도우미 --------
    private static TimeZoneInfo _kst;
    private static TimeZoneInfo KST
    {
        get
        {
            if (_kst != null) return _kst;
            try { _kst = TimeZoneInfo.FindSystemTimeZoneById("Asia/Seoul"); }
            catch
            {
                try { _kst = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"); }
                catch { _kst = TimeZoneInfo.CreateCustomTimeZone("KST", TimeSpan.FromHours(9), "Korea Standard Time", "KST"); }
            }
            return _kst;
        }
    }

    private static readonly DateTime UtcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // 입력 DateTime을 "KST 시각"으로 보정해서 돌려준다.
    private static DateTime EnsureKst(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Utc)
            return TimeZoneInfo.ConvertTimeFromUtc(dt, KST);
        if (dt.Kind == DateTimeKind.Local)
            return TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Local, KST);
        // Unspecified는 KST로 간주
        return dt;
    }

    // KST DateTime(또는 KST로 간주할 DateTime)을 Unix 초(UTC 기반)로 변환
    private static long KstDateTimeToUnixSeconds(DateTime kst)
    {
        var kstFixed = EnsureKst(kst);
        var utc = TimeZoneInfo.ConvertTimeToUtc(kstFixed, KST);
        return new DateTimeOffset(utc).ToUnixTimeSeconds();
    }

    // Unix 초(UTC) -> KST DateTime
    private static DateTime UnixSecondsToKst(long unixSeconds)
    {
        var utc = UtcEpoch.AddSeconds(unixSeconds);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, KST); // Kind: Unspecified (타임존 특정)
    }

    // -------- 기존 API (KST 기준으로 수정) --------

    public DateTime TimeStampToDateTime(long value)
    {
        // Unix(UTC) -> KST DateTime
        return UnixSecondsToKst(value);
    }

    public long DateTimeToTimeStamp(DateTime value)
    {
        // 입력을 KST로 해석하고 Unix(UTC) 초로 변환
        return KstDateTimeToUnixSeconds(value);
    }

    public long GetCurrentTimeStamp()
    {
        // 현재 시각(UTC)의 Unix 초 + diff
        // (diff는 서버/사용자 설정에 의해 조정된 'KST 의도 시각'을 UTC 타임스탬프로 이동)
        currentAppTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        currentTimestamp = currentAppTimestamp + diffTimestamp;
        return currentTimestamp;
    }

    public DateTime GetCurrentDateTime()
    {
        // 현재 타임스탬프를 KST DateTime으로
        currentDateTime = TimeStampToDateTime(GetCurrentTimeStamp());
        return currentDateTime;
    }

    public void SetTimestamp(string time)
    {
        // 문자열을 KST 시각으로 해석
        // (명시적 타임존이 들어오면 해당 타임존으로 파싱된 뒤 KST로 변환)
        DateTime parsed = DateTime.Parse(time, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        currentDateTime = EnsureKst(parsed);

        currentTimestamp = KstDateTimeToUnixSeconds(currentDateTime); // 목표 시각(UTC unix)
        currentAppTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(); // 지금(UTC unix)
        diffTimestamp = currentTimestamp - currentAppTimestamp;
    }

    public void RefreshTimestamp(DateTime dt)
    {
        // 주어진 dt를 KST로 해석해 기준 교정
        currentDateTime = EnsureKst(dt);
        currentTimestamp = KstDateTimeToUnixSeconds(currentDateTime);

        currentAppTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        diffTimestamp = currentTimestamp - currentAppTimestamp;
    }

    public long GetNextDailyResetTimeStamp(DateTime _dt)
    {
        // 입력 dt를 KST로 해석, 다음날 00:00(KST) 산출 후 Unix(UTC)로
        DateTime dtKst = EnsureKst(_dt);
        DateTime nextKstMidnight = new DateTime(dtKst.Year, dtKst.Month, dtKst.Day, 0, 0, 0, DateTimeKind.Unspecified).AddDays(1);
        return KstDateTimeToUnixSeconds(nextKstMidnight);
    }

    public long GetNextWeeklyResetTimeStamp(DateTime _dt)
    {
        // 기준: 매주 월요일 00:00 KST
        DateTime dtKst = EnsureKst(_dt);
        int days = ((int)DayOfWeek.Monday - (int)dtKst.DayOfWeek + 7) % 7;
        if (days == 0) days = 7; // 이미 월요일이면 다음 주
        DateTime nextMonKstMidnight = new DateTime(dtKst.Year, dtKst.Month, dtKst.Day, 0, 0, 0, DateTimeKind.Unspecified).AddDays(days);
        return KstDateTimeToUnixSeconds(nextMonKstMidnight);
    }

    public DateTime GetNextWeeklyResetDateTime(DateTime _dt)
    {
        // KST DateTime 반환
        DateTime dtKst = EnsureKst(_dt);
        int days = ((int)DayOfWeek.Monday - (int)dtKst.DayOfWeek + 7) % 7;
        if (days == 0) days = 7;
        DateTime nextMonKstMidnight = new DateTime(dtKst.Year, dtKst.Month, dtKst.Day, 0, 0, 0, DateTimeKind.Unspecified).AddDays(days);
        return nextMonKstMidnight; // Kind: Unspecified(KST 컨텍스트)
    }

    public DateTime ParseToDateTimeUTC(string strTime)
    {
        // 함수명은 유지하되 KST 기준으로 파싱 반환 (이름 변경 없이 내부 동작만 KST로)
        // - 타임존 미포함 문자열: KST로 간주
        // - 타임존 포함 문자열: 해당 존→KST로 변환
        DateTime parsed = DateTime.Parse(strTime, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        return EnsureKst(parsed);
    }
    public DateTime ParseToDateTimeKST(string strTime)
    {
        // 함수명은 유지하되 KST 기준으로 파싱 반환 (이름 변경 없이 내부 동작만 KST로)
        // - 타임존 미포함 문자열: KST로 간주
        // - 타임존 포함 문자열: 해당 존→KST로 변환
        DateTime parsed = DateTime.Parse(strTime, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        return parsed;
    }
    public int GetEventWeekByMonday(DateTime? startTime, int maxWeek = 4)
    {
        if (startTime == null)
            return 0;

        DateTime now = GetCurrentDateTime();
        DateTime start = startTime.Value;

        if (now < start)
            return 0;

        DateTime firstNextMonday = GetNextMonday00(start);

        // 시작일부터 다음 월요일 전까지는 1주차
        if (now < firstNextMonday)
            return 1;

        int week = 2 + (int)((now.Date - firstNextMonday.Date).TotalDays / 7);

        return Mathf.Clamp(week, 1, maxWeek);
    }
    DateTime GetNextMonday00(DateTime date)
    {
        DateTime day = date.Date;

        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)day.DayOfWeek + 7) % 7;

        // 시작일이 월요일이면 다음 주 월요일로
        if (daysUntilMonday == 0)
            daysUntilMonday = 7;

        return day.AddDays(daysUntilMonday);
    }
    [SerializeField] List<TimeFunc> timeFuncs = null;
    public void AddTimeCallback(string _key, float _addSeconds, Action _onEnd)
    {
        TimeFunc timeFunc = timeFuncs.Find(x => x.Key.Equals(_key));
        long endTimestamp = GetCurrentTimeStamp() + (long)_addSeconds; // 기준 KST 시계가 반영된 Unix 초
        if (timeFunc == null)
        {
            timeFunc = new TimeFunc(_key, endTimestamp, _onEnd);
            timeFuncs.Add(timeFunc);
        }
        else
        {
            timeFunc.EndTimestamp = endTimestamp;
            timeFunc.OnEnd = _onEnd;
        }
    }

    public void RemoveTimeCallback(string _key)
    {
        TimeFunc timeFunc = timeFuncs.Find(x => x.Key.Equals(_key));
        if (timeFunc != null)
        {
            timeFuncs.Remove(timeFunc);
            timeFunc.OnEnd = null;
        }
    }

    private void FixedUpdate()
    {
        if (Time.frameCount % 10 == 0)
        {
            for (int i = 0; i < timeFuncs.Count; i++)
            {
                TimeFunc timeFunc = timeFuncs[i];
                if (timeFunc == null) continue;
                if (timeFunc.EndTimestamp - GetCurrentTimeStamp() < 0)
                {
                    timeFuncs.Remove(timeFunc);
                    timeFunc.OnEnd?.Invoke();
                    timeFunc = null;
                    i--;
                }
            }
        }
    }
}

[Serializable]
public class TimeFunc
{
    public string Key = string.Empty;
    public long EndTimestamp = 0L;
    public Action OnEnd = null;

    public TimeFunc() { }
    public TimeFunc(string _key, long _endTimestamp, Action _onEnd)
    {
        Key = _key;
        EndTimestamp = _endTimestamp;
        OnEnd = _onEnd;
    }
}


//using GameUtils;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TimeManager : Singleton<TimeManager>
//{
//    [SerializeField] private long currentAppTimestamp = 0L;
//    [SerializeField] private long diffTimestamp = 0L;

//    [SerializeField] private long currentTimestamp = 0L;
//    private DateTime currentDateTime;

//    public DateTime TimeStampToDateTime(long value)
//    {
//        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
//        dt = dt.AddSeconds(value);
//        return dt;
//    }
//    public long DateTimeToTimeStamp(DateTime value)
//    {
//        return ((DateTimeOffset)value).ToUnixTimeSeconds();
//    }
//    public long GetCurrentTimeStamp()
//    {
//        currentAppTimestamp = DateTimeToTimeStamp(DateTime.UtcNow);
//        currentTimestamp = currentAppTimestamp + diffTimestamp;
//        return currentTimestamp;
//    }
//    public DateTime GetCurrentDateTime()
//    {
//        currentDateTime = TimeStampToDateTime(GetCurrentTimeStamp());
//        return currentDateTime;
//    }

//    public void SetTimestamp(string time)
//    {
//        currentDateTime = DateTime.Parse(time);
//        currentTimestamp = DateTimeToTimeStamp(currentDateTime);

//        currentAppTimestamp = DateTimeToTimeStamp(DateTime.UtcNow);
//        diffTimestamp = currentTimestamp - currentAppTimestamp;

//        // CGameUtils.Debug_Log(string.Format("DateTime : {0} Timestamp : {1}", currentDateTime, currentTimestamp));
//    }
//    public void RefreshTimestamp(DateTime dt)
//    {
//        currentDateTime = dt;
//        currentTimestamp = DateTimeToTimeStamp(dt);

//        currentAppTimestamp = DateTimeToTimeStamp(DateTime.UtcNow);
//        diffTimestamp = currentTimestamp - currentAppTimestamp;
//    }
//    public long GetNextDailyResetTimeStamp(DateTime _dt)
//    {
//        DateTime dt_Now = _dt;
//        DateTime dt_Rest = new DateTime(dt_Now.Year, dt_Now.Month, dt_Now.Day, 0, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
//        return DateTimeToTimeStamp(dt_Rest);
//    }
//    public long GetNextWeeklyResetTimeStamp(DateTime _dt)
//    {
//        DateTime dt_Now = _dt;
//        int addDays = 0;
//        switch (dt_Now.DayOfWeek)
//        {
//            case DayOfWeek.Sunday:// 0
//                addDays = 1;
//                break;
//            case DayOfWeek.Monday:// 1
//                addDays = 7;
//                break;
//            case DayOfWeek.Tuesday:// 2
//                addDays = 6;
//                break;
//            case DayOfWeek.Wednesday:// 3
//                addDays = 5;
//                break;
//            case DayOfWeek.Thursday:// 4
//                addDays = 4;
//                break;
//            case DayOfWeek.Friday:// 5
//                addDays = 3;
//                break;
//            case DayOfWeek.Saturday:// 6
//                addDays = 2;
//                break;
//            default:
//                break;
//        }
//        DateTime dt_Rest = new DateTime(dt_Now.Year, dt_Now.Month, dt_Now.Day, 0, 0, 0, 0, DateTimeKind.Utc).AddDays(addDays);
//        return DateTimeToTimeStamp(dt_Rest);
//    }
//    public DateTime GetNextWeeklyResetDateTime(DateTime _dt)
//    {
//        DateTime dt_Now = _dt;
//        int addDays = 0;
//        switch (dt_Now.DayOfWeek)
//        {
//            case DayOfWeek.Sunday:// 0
//                addDays = 1;
//                break;
//            case DayOfWeek.Monday:// 1
//                addDays = 7;
//                break;
//            case DayOfWeek.Tuesday:// 2
//                addDays = 6;
//                break;
//            case DayOfWeek.Wednesday:// 3
//                addDays = 5;
//                break;
//            case DayOfWeek.Thursday:// 4
//                addDays = 4;
//                break;
//            case DayOfWeek.Friday:// 5
//                addDays = 3;
//                break;
//            case DayOfWeek.Saturday:// 6
//                addDays = 2;
//                break;
//            default:
//                break;
//        }
//        DateTime dt_Rest = new DateTime(dt_Now.Year, dt_Now.Month, dt_Now.Day, 0, 0, 0, 0, DateTimeKind.Utc).AddDays(addDays);
//        return dt_Rest;
//    }

//    public DateTime ParseToDateTimeUTC(string strTime)
//    {
//        DateTime dt = DateTime.Parse(strTime, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
//        return dt;

//        //string[] splits = strTime.Split('.');
//        //int year = CGameUtils.TryParse<int>(splits[0]);
//        //int month = CGameUtils.TryParse<int>(splits[1]);
//        //int day = CGameUtils.TryParse<int>(splits[2]);
//        //DateTime dt = DateTime.ParseExact(string.Format("{0}/{1}/{2} 00:00:00", year.ToString("0000"), month.ToString("00"), day.ToString("00")), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
//        //return dt;
//    }

//    [SerializeField] List<TimeFunc> timeFuncs = null;
//    public void AddTimeCallback(string _key, float _addSeconds, Action _onEnd)
//    {
//        TimeFunc timeFunc = timeFuncs.Find(x => x.Key.Equals(_key));
//        long endTimestamp = GetCurrentTimeStamp() + (long)_addSeconds;
//        if (timeFunc == null)
//        {
//            timeFunc = new TimeFunc(_key, endTimestamp, _onEnd);
//            timeFuncs.Add(timeFunc);
//        }
//        else
//        {
//            timeFunc.EndTimestamp = endTimestamp;
//            timeFunc.OnEnd = _onEnd;
//        }
//    }
//    public void RemoveTimeCallback(string _key)
//    {
//        TimeFunc timeFunc = timeFuncs.Find(x => x.Key.Equals(_key));
//        if (timeFunc != null)
//        {
//            timeFuncs.Remove(timeFunc);
//            timeFunc.OnEnd = null;
//        }
//    }
//    private void FixedUpdate()
//    {
//        if (Time.frameCount % 10 == 0)
//        {
//            for (int i = 0; i < timeFuncs.Count; i++)
//            {
//                TimeFunc timeFunc = timeFuncs[i];
//                if (timeFunc == null) continue;
//                if (timeFunc.EndTimestamp - GetCurrentTimeStamp() < 0)
//                {
//                    timeFuncs.Remove(timeFunc);
//                    timeFunc.OnEnd?.Invoke();
//                    timeFunc = null;
//                    i--;
//                }
//            }
//        }
//    }
//}

//[Serializable]
//public class TimeFunc
//{
//    public string Key = string.Empty;
//    public long EndTimestamp = 0L;
//    public Action OnEnd = null;

//    public TimeFunc() { }
//    public TimeFunc(string _key, long _endTimestamp, Action _onEnd)
//    {
//        Key = _key;
//        EndTimestamp = _endTimestamp;
//        OnEnd = _onEnd;
//    }
//}