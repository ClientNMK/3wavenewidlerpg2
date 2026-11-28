using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Text;
using System;
using LitJson;
using System.Text.RegularExpressions;
using BackEnd;
using BackEnd.GlobalSupport;
using UnityEngine.Networking;
using Spine.Unity;

public enum ContinentType
{
    KR,
    JP,
    ASIA,
    NA,
}

public enum ParameterValueType
{
    Normal = 0,
    Percet,
    Second
}

namespace GameUtils
{
    public static class GameUtils
    {
        private const int rondomCount = 1000;
        private static int m_nIndex = 0;
        private static int[] m_nArr = new int[rondomCount];
        private static Transform popuptrs = null;
        const string Zero = "0";
        public enum CurrencyType { KR, EN, }
        static readonly string[] CurrencyUnits = CreateCurrencyUnits(true, 300);
        static readonly string[] CurrencyUnits2 = CreateCurrencyUnits(false, 300);

        private static string[] CreateCurrencyUnits(bool korean, int maxUnitIndex)
        {
            List<string> units = new();

            if (korean)
            {
                units.AddRange(new[]
                {
            "", "만", "억", "조", "경", "해", "자", "양", "구", "간", "정", "재", "극"
        });
            }
            else
            {
                units.Add("");
            }

            int alphabetIndex = 1;

            while (units.Count <= maxUnitIndex)
            {
                units.Add(GetAlphabetUnit(alphabetIndex));
                alphabetIndex++;
            }

            return units.ToArray();
        }
        private static string GetAlphabetUnit(int index)
        {
            if (index <= 0)
                return "";

            StringBuilder sb = new StringBuilder();

            while (index > 0)
            {
                index--;

                sb.Insert(0, (char)('A' + (index % 26)));

                index /= 26;
            }

            return sb.ToString();
        }
        //1,000,000,000,000
        static readonly string[] SI = new string[] { };
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        static GameUtils()
        {
            for (int i = 0; i < rondomCount; ++i)
            {
                m_nArr[i] = UnityEngine.Random.Range(0, 10000);
            }
        }

        private static int nIndex
        {
            get
            {
                int nTemp = m_nIndex++;

                if (m_nIndex >= rondomCount) { m_nIndex = 0; }

                return nTemp;
            }
        }

        static Dictionary<uint, WaitForSeconds> m_dicMapTime = new Dictionary<uint, WaitForSeconds>();
        static Dictionary<uint, WaitForSecondsRealtime> m_dicMapRealTime = new Dictionary<uint, WaitForSecondsRealtime>();

        public static WaitForSeconds WaitForSeconds(float a_fSeconds)
        {
            if (a_fSeconds < 0.0f) { Debug.Log("arg error"); return null; }

            uint nVal = 1;

            if (a_fSeconds >= 0.001f)
            {
                nVal = (uint)(a_fSeconds * 1000);
            }
            return WaitForMilliSeconds(nVal);
        }


        public static WaitForSecondsRealtime WaitForSecondsRealTime(float a_fSecondsR)
        {
            if (a_fSecondsR < 0.0f) { Debug.Log("arg error"); return null; }

            uint nVal = 1;

            if (a_fSecondsR >= 0.001f)
            {
                nVal = (uint)(a_fSecondsR * 1000);
            }
            return WaitForMilliSecondsRealTime(nVal);
        }


        public static WaitForSeconds WaitForMilliSeconds(uint a_nMilliSecond)
        {
            if (m_dicMapTime.TryGetValue(a_nMilliSecond, out WaitForSeconds instance) == false)
            {
                instance = new WaitForSeconds(a_nMilliSecond / (float)1000);
                m_dicMapTime.Add(a_nMilliSecond, instance);
            }
            return instance;
        }

        public static WaitForSecondsRealtime WaitForMilliSecondsRealTime(uint a_nMilliSecondR)
        {
            if (m_dicMapRealTime.TryGetValue(a_nMilliSecondR, out WaitForSecondsRealtime instance) == false)
            {
                instance = new WaitForSecondsRealtime(a_nMilliSecondR / (float)1000);
                m_dicMapRealTime.Add(a_nMilliSecondR, instance);
            }
            return instance;
        }


        public static string ToStringData(this List<int> list, string splitChar)
        {
            string toData = string.Join(splitChar, list.ToArray());
            return toData;
        }

        public static List<int> getParseToIntList(this string dataStr, char splitChar)
        {
            List<int> dataList = new List<int>();
            string[] splitedDatas = dataStr.Split(splitChar);
            for (int i = 0; i < splitedDatas.Length; i++)
            {
                string data = splitedDatas[i];
                dataList.Add(int.Parse(data));
            }
            return dataList;
        }

        //public static List<int> getRandomList(List<int> lipercent, List<string> list,int count)
        //{
        //    int index = 0;
        //    int num = 0;
        //    List<int> templist = new List<int>();
        //    while (count > index)
        //    {
        //        Debug.Log("num == " + num);
        //        bool isbool = getPercent(lipercent[num]);
        //        if (isbool)
        //        {
        //            string[] split = list[num].Split(',');
        //            int random = UnityEngine.Random.Range(0, split.Length);
        //            templist.Add(random);
        //            index++;
        //            num = 0;
        //            continue;
        //        }
        //        num++;
        //        if (num > count)
        //        {
        //            num = 0;
        //        }
        //    }
        //    return templist;
        //}

        //public static int GetRandomIdx(float[] arrPercent)
        //{
        //    float totalPercent = 0;
        //    foreach (var item in arrPercent)
        //    {
        //        // totalPercent = (float)Math.Round(totalPercent + item, 3);
        //        totalPercent += item;
        //    }
        //    totalPercent = (float)Math.Round(totalPercent, 3);
        //    if (totalPercent < 100)
        //        return -1;

        //    float randNum = UnityEngine.Random.Range(0f, 1f) * 100f;
        //    float compNum = 0;
        //    for (int i = arrPercent.Length - 1; i >= 0; i--)
        //    {
        //        compNum += arrPercent[i];
        //        if (randNum < compNum)
        //        {
        //            return i;
        //        }
        //    }

        //    return -1;
        //}
        public static int GetRandomIdx(float[] arrPercent)
        {
            if (arrPercent == null || arrPercent.Length == 0) return -1;

            float total = 0f;
            int maxIdx = 0;
            float maxVal = float.NegativeInfinity;

            // 한 번의 루프로 합계/최대값 인덱스 계산
            for (int i = 0; i < arrPercent.Length; i++)
            {
                float w = arrPercent[i];
                total += w;
                if (w > maxVal) { maxVal = w; maxIdx = i; }
            }

            total = (float)Math.Round(total, 3);
            if (total < 100f)
                return -1;

            float randNum = UnityEngine.Random.value * 100f;
            float compNum = 0f;

            for (int i = arrPercent.Length - 1; i >= 0; i--)
            {
                compNum += arrPercent[i];
                if (randNum < compNum)
                    return i;
            }

            // 부동소수 경계 등으로 여기 오면 최대 확률의 인덱스로 폴백
            return maxIdx;
        }

        public static int getRandom() { return m_nArr[nIndex]; }

        public static float getPercent() { return m_nArr[nIndex] * 0.0001f; }

        public static bool getPercent(float a_nPercent)
        {
            if (a_nPercent == 0) { return false; }

            return m_nArr[nIndex] <= (a_nPercent * 100);
        }

        static StringBuilder sb = new StringBuilder();
        public static string Append(params string[] str)
        {
            sb.Clear();
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }

        public static string setStringBuilder(this string[] str)
        {
            StringBuilder sc = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                sc.Append(str[i]);
            }
            return sc.ToString();
        }

        public static string setCommaNumber(float value, string str = "")
        {
            string[] temp = new string[2];
            temp[0] = string.Format("{0:#,###}", value);
            temp[1] = str;
            string result = setStringBuilder(temp);
            return result;
        }

        //public static string ToCurrencyString(this double number, bool isPercent = false, CurrencyType currencyType = CurrencyType.KR)
        //{
        //    currencyType = StringManager.Instance.GetLanguageType() == SystemLanguage.Korean ? CurrencyType.KR : CurrencyType.EN;

        //    // 1미만일 경우 소수 3째짜리 까지 보여줌  0.005
        //    if (-1d < number && number < 1d)
        //    {
        //        string strValue = number.ToString("0.####");
        //        if (isPercent)
        //            return Append(strValue, "%");
        //        else
        //            return strValue;
        //    }
        //    // 100미만일 경우   소수 2째짜리 까지 보여줌  12.23
        //    else if (-100d < number && number < 100d)
        //    {
        //        string strValue = isPercent ? number.ToString("0.##") : number.ToString("0");
        //        if (isPercent)
        //            return Append(strValue, "%");
        //        else
        //            return strValue;
        //    }
        //    else if (-1000d < number && number < 1000d)
        //    {
        //        string strValue = isPercent ? number.ToString("0.#") : number.ToString("0");
        //        if (isPercent)
        //            return Append(strValue, "%");
        //        else
        //            return strValue;
        //    }
        //    // 100이상~10000이하 소수점을 보여주지 않는다   8465
        //    else if (-10000d < number && number < 10000d)
        //    {
        //        string strValue = number.ToString("0");
        //        if (isPercent)
        //            return Append(strValue, "%");
        //        else
        //            return strValue;
        //    }

        //    // 무한
        //    if (true == double.IsInfinity(number))
        //    {
        //        return "Infinity";
        //    }

        //    //string significant = (number < 0) ? "-" : string.Empty;
        //    string showNumber = string.Empty;
        //    string unitString = string.Empty;
        //    string[] partsSplit = number.ToString("E").Split('+');
        //    if (partsSplit.Length < 2)
        //    {
        //        Debug.LogWarning(string.Format("Failed - ToCurrencyString({0})", number)); return Zero;
        //    }

        //    if (false == int.TryParse(partsSplit[1], out int exponent))
        //    {
        //        Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}", number, partsSplit[1]));
        //        return Zero;
        //    }

        //    int quotient = exponent / 4;
        //    int remainder = exponent % 4;
        //    if (exponent < 4)
        //    {
        //        // showNumber = Math.Truncate(number).ToString();
        //        //if (valueType == StatusValueType.Percent)
        //        //    showNumber = Math.Round(number, 3).ToString();
        //        //else
        //        //    showNumber = Math.Truncate(Math.Round(number, 3)).ToString();
        //        showNumber = Math.Truncate(Math.Round(number, 3)).ToString();
        //    }
        //    else
        //    {
        //        //var temp = double.Parse(partsSplit[0].Replace("E", "")) * Math.Pow(10, remainder);
        //        //showNumber = temp.ToString("F").Replace(".00", "");

        //        string replace = partsSplit[0].Replace("E", "");
        //        double parseValue = double.Parse(replace);
        //        var temp = parseValue * Math.Pow(10, remainder);
        //        showNumber = temp.ToString("0.###0");
        //    }

        //    string unitString2 = string.Empty;
        //    unitString = (currencyType == CurrencyType.KR) ? CurrencyUnits[quotient] : CurrencyUnits2[quotient];
        //    if (quotient > 0)
        //    {
        //        unitString2 = (currencyType == CurrencyType.KR)
        //            ? CurrencyUnits[quotient - 1]
        //            : CurrencyUnits2[quotient - 1];
        //    }
        //    string result = string.Empty;
        //    if (exponent < 4)
        //    {
        //        result = string.Format("{0}{1}", showNumber, unitString);
        //    }
        //    else
        //    {
        //        string[] showNumberSplit = showNumber.Split('.');
        //        if (showNumberSplit.Length > 1)
        //        {
        //            // NNNN억NNNN만
        //            if (quotient > 1)
        //                result = string.Format("{0}{1} {2}{3}", showNumberSplit[0], unitString, showNumberSplit[1], unitString2);// significant,
        //                                                                                                                         // NNNN만NNNN
        //            else
        //                result = string.Format("{0}{1} {2}", showNumberSplit[0], unitString, showNumberSplit[1]);// significant,
        //        }
        //        else
        //        {
        //            result = string.Format("{0}{1}", showNumber, unitString);
        //        }
        //    }

        //    if (isPercent)
        //        return Append(result, "%");
        //    return result;
        //}
        //public static string ToCurrencyStringExpand(this double number, bool isPercent = false,  CurrencyType currencyType = CurrencyType.KR)
        //{
        //    currencyType = StringManager.Instance.GetLanguageType() == SystemLanguage.Korean ? CurrencyType.KR : CurrencyType.EN;

        //    // 1미만일 경우 소수 3째짜리 까지 보여줌  0.005
        //    if (-1d < number && number < 1d)
        //    {
        //        string strValue = number.ToString("0.####");
        //        if (isPercent)
        //            return Append(strValue, "%");
        //        else
        //            return strValue;
        //    }
        //    // 100미만일 경우   소수 2째짜리 까지 보여줌  12.23
        //    else if (-100d < number && number < 100d)
        //    {
        //        string strValue = number.ToString("0.##");
        //        if (isPercent)
        //            return Append(strValue, "%");
        //        else
        //            return strValue;
        //    }
        //    // 100이상~10000이하 소수점을 보여주지 않는다   8465
        //    else if (-10000d < number && number < 10000d)
        //    {
        //        string strValue = number.ToString("0");
        //        if (isPercent)
        //            return Append(strValue, "%");
        //        else
        //            return strValue;
        //    }

        //    // 무한
        //    if (true == double.IsInfinity(number))
        //    {
        //        return "Infinity";
        //    }

        //    //string significant = (number < 0) ? "-" : string.Empty;
        //    string showNumber = string.Empty;
        //    string unitString = string.Empty;
        //    string[] partsSplit = number.ToString("E").Split('+');
        //    if (partsSplit.Length < 2)
        //    {
        //        Debug.LogWarning(string.Format("Failed - ToCurrencyString({0})", number)); return Zero;
        //    }

        //    if (false == int.TryParse(partsSplit[1], out int exponent))
        //    {
        //        Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}", number, partsSplit[1]));
        //        return Zero;
        //    }

        //    int quotient = exponent / 4;
        //    int remainder = exponent % 4;
        //    if (exponent < 4)
        //    {
        //        // showNumber = Math.Truncate(number).ToString();
        //        //if (valueType == StatusValueType.Percent)
        //        //    showNumber = Math.Round(number, 3).ToString();
        //        //else
        //        //    showNumber = Math.Truncate(Math.Round(number, 3)).ToString();
        //        showNumber = Math.Truncate(Math.Round(number, 3)).ToString();
        //    }
        //    else
        //    {
        //        //var temp = double.Parse(partsSplit[0].Replace("E", "")) * Math.Pow(10, remainder);
        //        //showNumber = temp.ToString("F").Replace(".00", "");

        //        string replace = partsSplit[0].Replace("E", "");
        //        double parseValue = double.Parse(replace);
        //        var temp = parseValue * Math.Pow(10, remainder);
        //        showNumber = temp.ToString("0.###0");
        //    }

        //    string unitString2 = string.Empty;
        //    unitString = (currencyType == CurrencyType.KR) ? CurrencyUnits[quotient] : CurrencyUnits2[quotient];

        //    string result = string.Empty;
        //    if (exponent < 4)
        //    {
        //        result = string.Format("{0}{1}", showNumber, unitString);
        //    }
        //    else
        //    {
        //        string[] showNumberSplit = showNumber.Split('.');
        //        if (showNumberSplit.Length > 1)
        //        {
        //            // NNNN억NNNN만
        //            if (quotient > 1)
        //                result = string.Format("{0}{1}", showNumberSplit[0], unitString);// significant,
        //                                                                                                                         // NNNN만
        //            else
        //                result = string.Format("{0}{1}", showNumberSplit[0], unitString);// significant,
        //        }
        //        else
        //        {
        //            result = string.Format("{0}{1}", showNumber, unitString);
        //        }
        //    }

        //    if (isPercent)
        //        return Append(result, "%");
        //    return result;
        //}
        public static string ToCurrencyString(this double number, bool isPercent = false, CurrencyType currencyType = CurrencyType.KR)
        {
            currencyType = StringManager.Instance.GetLanguageType() == SystemLanguage.Korean
                ? CurrencyType.KR
                : CurrencyType.EN;

            if (double.IsNaN(number))
                return "NaN";

            if (double.IsInfinity(number))
                return "Infinity";

            bool isNegative = number < 0;
            double absNumber = Math.Abs(number);

            string result;

            // 1 미만
            if (absNumber < 1d)
            {
                result = absNumber.ToString("0.####", CultureInfo.InvariantCulture);
            }
            // 100 미만
            else if (absNumber < 100d)
            {
                result = isPercent
                    ? absNumber.ToString("0.##", CultureInfo.InvariantCulture)
                    : Math.Round(absNumber).ToString("0", CultureInfo.InvariantCulture);
            }
            // 1000 미만
            else if (absNumber < 1000d)
            {
                result = isPercent
                    ? absNumber.ToString("0.#", CultureInfo.InvariantCulture)
                    : Math.Round(absNumber).ToString("0", CultureInfo.InvariantCulture);
            }
            // 10000 미만
            else if (absNumber < 10000d)
            {
                result = Math.Round(absNumber).ToString("#,0", CultureInfo.InvariantCulture);
            }
            else
            {
                result = FormatLargeNumber(absNumber, currencyType);
            }

            if (isNegative && result != "0")
                result = "-" + result;

            if (isPercent)
                result += "%";

            return result;
        }
        public static string ToCurrencyString(this BigDouble number, bool isPercent = false, CurrencyType currencyType = CurrencyType.KR)
        {
            currencyType = StringManager.Instance.GetLanguageType() == SystemLanguage.Korean
                ? CurrencyType.KR
                : CurrencyType.EN;

            if (double.IsNaN(number.Mantissa))
                return "NaN";

            if (double.IsInfinity(number.Mantissa))
                return "Infinity";

            // BigDouble 부호는 Mantissa 기준으로 판정
            bool isNegative = number.Mantissa < 0;

            BigDouble absNumber = isNegative ? -number : number;

            string result;

            if (absNumber == BigDouble.Zero)
            {
                result = "0";
            }
            else if (absNumber < new BigDouble(1d))
            {
                double value = Math.Abs(absNumber.Mantissa) * Math.Pow(10, absNumber.Exponent);
                result = value.ToString("0.####", CultureInfo.InvariantCulture);
            }
            else if (absNumber < new BigDouble(100d))
            {
                double value = Math.Abs(absNumber.Mantissa) * Math.Pow(10, absNumber.Exponent);

                result = isPercent
                    ? value.ToString("0.##", CultureInfo.InvariantCulture)
                    : Math.Round(value).ToString("0", CultureInfo.InvariantCulture);
            }
            else if (absNumber < new BigDouble(1000d))
            {
                double value = Math.Abs(absNumber.Mantissa) * Math.Pow(10, absNumber.Exponent);

                result = isPercent
                    ? value.ToString("0.#", CultureInfo.InvariantCulture)
                    : Math.Round(value).ToString("0", CultureInfo.InvariantCulture);
            }
            else if (absNumber < new BigDouble(10000d))
            {
                double value = Math.Abs(absNumber.Mantissa) * Math.Pow(10, absNumber.Exponent);
                result = Math.Round(value).ToString("#,0", CultureInfo.InvariantCulture);
            }
            else
            {
                result = FormatLargeNumber(absNumber, currencyType);
            }

            // 부호는 최종 단계에서 한 번만 추가
            if (isNegative && result != "0")
                result = "-" + result;

            if (isPercent)
                result += "%";

            return result;
        }
        //public static string ToCurrencyMultipleString(this double number, CurrencyType currencyType = CurrencyType.KR)
        //{
        //    currencyType = StringManager.Instance.GetLanguageType() == SystemLanguage.Korean
        //        ? CurrencyType.KR
        //        : CurrencyType.EN;

        //    if (double.IsNaN(number))
        //        return "NaN";

        //    if (double.IsInfinity(number))
        //        return "Infinity";

        //    bool isNegative = number < 0;
        //    double absNumber = Math.Abs(number);

        //    string result;

        //    // 1 미만
        //    if (absNumber < 1d)
        //    {
        //        result = absNumber.ToString("0.####", CultureInfo.InvariantCulture);
        //    }
        //    // 100 미만
        //    else if (absNumber < 100d)
        //    {
        //        result =
        //             absNumber.ToString("0.##", CultureInfo.InvariantCulture);

        //    }
        //    // 1000 미만
        //    else if (absNumber < 1000d)
        //    {
        //        result = absNumber.ToString("0.#", CultureInfo.InvariantCulture);
        //    }
        //    // 10000 미만
        //    else if (absNumber < 10000d)
        //    {
        //        result = Math.Round(absNumber).ToString("0", CultureInfo.InvariantCulture);
        //    }
        //    else
        //    {
        //        result = FormatLargeNumber(absNumber, currencyType);
        //    }

        //    if (isNegative && result != "0")
        //        result = "-" + result;

        //    return result;
        //}
        private static string FormatLargeNumber(BigDouble number, CurrencyType currencyType)
        {
            string[] units = currencyType == CurrencyType.KR ? CurrencyUnits : CurrencyUnits2;

            if (number <= BigDouble.Zero)
                return "0";

            // 4자리 단위이므로 지수 4마다 단위가 하나 증가
            int unitIndex = number.Exponent / 4;

            if (unitIndex <= 0)
            {
                double normalValue = number.Mantissa * Math.Pow(10, number.Exponent);
                return Math.Round(normalValue).ToString("0", CultureInfo.InvariantCulture);
            }

            // 현재 배열의 마지막 단위까지만 사용
            if (unitIndex >= units.Length)
                unitIndex = units.Length - 1;

            int unitExponent = unitIndex * 4;
            int remainExponent = number.Exponent - unitExponent;

            // 선택된 단위로 나눈 값
            // 예: 1.2345E12 / 10^12 = 1.2345
            double temp = number.Mantissa * Math.Pow(10, remainExponent);

            // 유효 숫자 4자리, 소수 최대 2자리: 1.23만, 12.34만, 246.6만, 1,234만 (kit_cocoa LAYOUT_battle '재화 숫자 표시')
            return temp.ToString(temp < 100d ? "0.##" : temp < 1000d ? "0.#" : "#,0", CultureInfo.InvariantCulture) + units[unitIndex];
        }
        private static string FormatLargeNumber(double number, CurrencyType currencyType)
        {
            string[] units = currencyType == CurrencyType.KR ? CurrencyUnits : CurrencyUnits2;

            // 4자리 단위 index 계산
            int unitIndex = 0;
            double temp = number;

            while (temp >= 10000d && unitIndex < units.Length - 1)
            {
                temp /= 10000d;
                unitIndex++;
            }

            // 마지막 단위까지만 지원
            if (unitIndex <= 0)
                return Math.Round(number).ToString("0", CultureInfo.InvariantCulture);

            // 유효 숫자 4자리, 소수 최대 2자리: 1.23만, 12.34만, 246.6만, 1,234만 (kit_cocoa LAYOUT_battle '재화 숫자 표시')
            return temp.ToString(temp < 100d ? "0.##" : temp < 1000d ? "0.#" : "#,0", CultureInfo.InvariantCulture) + units[unitIndex];
        }
        public static double ToCurrencyDouble(this string currencyString, CurrencyType stringType = CurrencyType.KR)
        {
            double result = 0;
            bool isNumber = double.TryParse(currencyString, out result);
            if (true == isNumber)
            {
                return result;
            }
            else
            {
                int length = currencyString.Length; int lastNumberIndex = -1;
                for (int i = length - 1; 0 <= i; --i)
                {
                    if (true == char.IsNumber(currencyString, i))
                    {
                        lastNumberIndex = i;
                        break;
                    }
                }
                if (lastNumberIndex < 0)
                {
                    throw new Exception("Failed currency string");
                }
                string number = currencyString.Substring(0, lastNumberIndex + 1);
                string unit = currencyString.Substring(lastNumberIndex + 1);
                int index = (CurrencyType.KR == stringType) ? Array.FindIndex(CurrencyUnits, p => p == unit) : Array.FindIndex(SI, p => p == unit);
                if (-1 == index)
                {
                    throw new Exception("Failed currency string");
                }
                string exponentNumber = string.Format("{0}E+{1}", number, index * 3);
                return double.Parse(exponentNumber);
            }
        }

        public static string prettySecondsTimeFormat(int seconds)
        {
            TimeSpan timespan = TimeSpan.FromSeconds(seconds);
            string[] strtime = timespan.ToString().Split('.');
            return strtime[0];
        }

        public static double getSliceDouble(this double self, int n)
        {
            int sliceNumber = (int)Math.Pow(10, n);
            return (Math.Truncate(self * sliceNumber) / sliceNumber);
        }


        public static Color copy(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
        }



        public static double getRemainTime(string key)
        {
            double result;

            string datetime = PlayerPrefs.GetString(key);
            DateTime goaltime = Convert.ToDateTime(datetime);

            DateTime currenttime = DateTime.Parse(DateTime.Now.ToString());//CGameManager.Instance.getServerDateTime();
            TimeSpan resulttime = goaltime - currenttime;

            result = resulttime.TotalSeconds;
            return result;
        }

        public static DateTime getCalDayDateTime(int resettime, DateTime currentdate)
        {
            DateTime date;
            int hour = currentdate.Hour;
            int min = currentdate.Minute;
            int sec = currentdate.Second;

            int latehour = 0;
            int latemin = 0;
            int latesec = 0;
            int resultsec = 0;

            if (resettime > hour)
            {
                latehour = resettime - (hour + 1);
                latemin = 60 - min;
            }
            else
            {
                latehour = resettime + (23 - hour);
                latemin = 60 - min;
            }

            latesec = 60 - sec;
            resultsec = ((latehour * 3600) + (latemin * 60) + latesec) - 60;
            date = currentdate.AddSeconds(resultsec);
            return date;
        }


        public static int getCalSecond(int resettime, DateTime currentdate)
        {
            int hour = currentdate.Hour;
            int min = currentdate.Minute;
            int sec = currentdate.Second;

            int latehour = 0;
            int latemin = 0;
            int latesec = 0;
            int resultsec = 0;

            if (resettime > hour)
            {
                latehour = resettime - (hour + 1);
                latemin = 60 - min;
            }
            else
            {
                latehour = resettime + (23 - hour);
                latemin = 60 - min;
            }

            latesec = 60 - sec;
            resultsec = ((latehour * 3600) + (latemin * 60) + latesec) - 60;
            return resultsec;
        }

        public static bool checkNickName(string _name)
        {
            return Regex.IsMatch(_name, "^[0-9a-zA-Z가-힣]*$");
        }


        //펫 레벨업 공격력 공식 
        public static float getCalPetStatLevel(float start, float increase, int level)
        {
            if (level == 0)
                return 0;
            float result = start + (increase * level);
            //int temp = 0;
            //if (level > 500)
            //{
            //    result += 1;
            //    temp = level - 500;
            //    result += temp * increase;

            //    if (level > 1000)
            //    {
            //        result += 3;
            //        temp = level - 1000;
            //        result += temp * increase;

            //        if (level > 1500)
            //        {
            //            result += 5;
            //            temp = level - 1500;
            //            result += temp * increase;
            //        }
            //    }
            //}
            //result += (increase * level);
            return result;
        }

        //펫 스킬 레벨시 능력 공식
        public static float getCalPetSkillLevel(float start, float increase, int level)
        {
            if (level == 0)
                return 0;
            float result = start + (increase * level);
            return result;
        }

        public static double getCalPetGoldSum(int level)
        {
            double result = (((double.Parse(level.ToString()) - 1) / 2) * 10 + 10) * (double.Parse(level.ToString()) * 2000);

            if (level >= 500)
            {
                result = (((double.Parse(level.ToString()) - 1) / 2) * 20 + 10) * (double.Parse((level * level).ToString()) * 20);
            }

            result = Math.Round(result);
            return result;
        }

        public static double getCalPetSkillSum(int level)
        {
            double result = (((level - 1) / 10) * 50 + 100) * level;
            result = Math.Round(result);
            return result;
        }

        //펫 레벨업 비용 공식 || 펫 스킬 레벨업 비용 공식 
        public static double getCalPetStatLevelSum(int startvalue, float value, int level)
        {
            double result = startvalue + (startvalue * (value * Math.Pow(level, 2)));
            result = Math.Round(result);
            return result;
        }

        //캐릭터 스텟 비용 공식 
        public static float getCalCharStatLevel(double start, float increase, int level)
        {
            double result = start + (increase * level);
            result = Math.Round(result, 4);//Math.Round(result, 4);
            return float.Parse(result.ToString());
        }


        //캐릭터 스텟 비용 공식 
        public static double getCalCharStatLevel(double start, float increase, float A, float B, float C, int level, int round = 2)
        {
            if (A == 0 && B == 0 && C == 0)
            {
                return start + (increase * level);
            }

            double result = start + ((A * Mathf.Pow(level, 3)) - (B * Mathf.Pow(level, 2)) + (C * level));
            result = Math.Round(result, round); //Math.Round(result, round);
            return result;
        }

        //캐릭터 스텟 비용 공식 
        public static double getCalCharStatLevelSum(double startgold, float goldA, float goldB, float goldC, int level, int round)
        {
            //double temp = 1;
            //if (level > 1)
            //{
            //    temp = Math.Pow(goldC, (level - 1));
            //}

            double result = startgold + ((goldA * Math.Pow(level, 3)) - (goldB * Math.Pow(level, 2)) + (goldC * level));
            result = Math.Round(result, round);
            return result;
        }


        //악세사리
        public static float getCalAccessory(float value, float rate, int level)
        {
            if (level == 0)
                level = 1;

            double result = Math.Round(value + ((level - 1) * value * rate), 4);
            return (float)result;
        }

        public static float getCalAccessoryHaveAbility(float attack, float attackrate, int level)
        {
            if (level == 0)
                level = 1;

            double result = Math.Round(attack + ((level - 1) * attackrate), 4);
            return (float)result;
        }

        //장비 공격력
        public static float getCalWeaponAttack(float attack, float attackrate, int level)
        {
            if (level == 0)
                level = 1;

            float result = attack + ((level - 1) * attack * attackrate);
            return result;
        }

        //장비 공격속도
        public static float getCalCostumeSpeed(float speed, float speedrate, int level)
        {
            if (level == 0)
                level = 1;
            double result = Math.Round(speed + ((level - 1) * speedrate), 4);
            return (float)result;
        }

        //장비 공격속도
        public static float getCalCostumeAttackSpeedPercent(float attack, float attackrate, int level)
        {
            if (level == 0)
                level = 1;

            double result = Math.Round(attack + ((level - 1) * attackrate), 4);
            return (float)result;
        }

        //장비 업그레이드 비용
        public static double getCalUpgradeGoldPrice(double price, float rate, int level)
        {
            double result = Math.Round(price + ((level - 1) * price * rate));
            return result;
        }

        //장비 업그레이드 비용
        public static double getCalUpgradeGemPrice(double price, int level, double increase)
        {
            double result = Math.Round(price + ((level - 1) * increase));
            return result;
        }

        //장비 업그레이드 확률
        public static float getCalUpgradePercent2(float limit, float decreaserate, int level)
        {
            float result = 10f;
            for (int i = 0; i < level; i++)
            {
                result -= decreaserate;
            }

            result = float.Parse(Math.Truncate(result).ToString());
            if (limit > result)
            {
                result = limit;
            }
            return result;
        }

        //장비 업그레이드 확률
        public static float getCalUpgradePercent(float limit, float decreaserate, int level)
        {
            float result = 100f;
            for (int i = 0; i < level; i++)
            {
                result -= decreaserate;
            }

            result = float.Parse(Math.Truncate(result).ToString());
            if (limit > result)
            {
                result = limit;
            }
            return result;
        }


        public static double getCalDoubleRelicPassive(float defaultvalue, float increase, int level)
        {
            if (level == 0)
                return 0;
            double result = double.Parse(Math.Round((defaultvalue + (increase * (level))), 5).ToString());

            return result;
        }

        public static float getCalRelicPassive(float defaultvalue, float increase, int level)
        {
            if (level == 0)
                return 0;
            float result = float.Parse(Math.Round((defaultvalue + (increase * (level))), 5).ToString());

            return result;
        }

        public static T TryParse<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default(T);

            string s = data.ToString();

            if (typeof(T) == typeof(int))
            {
                int i = 0;
                int.TryParse(s, out i);
                return (T)Convert.ChangeType(i, typeof(int));
            }
            else if (typeof(T) == typeof(float))
            {
                float f = 0.0f;
                float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out f);
                return (T)Convert.ChangeType(f, typeof(float));
            }
            else if (typeof(T) == typeof(long))
            {
                long l = 0L;
                long.TryParse(s, out l);
                return (T)Convert.ChangeType(l, typeof(long));
            }
            else if (typeof(T) == typeof(ushort))
            {
                ushort us = 0;
                ushort.TryParse(s, out us);
                return (T)Convert.ChangeType(us, typeof(ushort));
            }
            else if (typeof(T) == typeof(double))
            {
                double d = 0.0;
                double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d);
                return (T)Convert.ChangeType(d, typeof(double));
            }
            else if (typeof(T) == typeof(BigDouble))
            {
                try
                {
                    BigDouble value = BigDouble.Parse(s);
                    return (T)(object)value;
                }
                catch
                {
                    return (T)(object)BigDouble.Zero;
                }
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(s, typeof(string));
            }
            else if (typeof(T).IsEnum)
            {
                int i = 0;
                int.TryParse(s, out i);
                return (T)Enum.Parse(typeof(T), i.ToString());
            }
            else if (typeof(T) == typeof(bool))
            {
                bool b = false;
                bool.TryParse(s, out b);
                return (T)Convert.ChangeType(b, typeof(bool));
            }

            return default(T);
        }

        public static T TryParse<T>(string key, LitJson.JsonData jsonData)
        {
            if (jsonData == null || !jsonData.ContainsKey(key))
                return default(T);

            JsonData data = jsonData[key];

            if (data == null)
                return default(T);

            if (typeof(T) == typeof(int))
            {
                int i = 0;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        int.TryParse(data["S"].ToString(), out i);
                    else if (data.ContainsKey("N"))
                        int.TryParse(data["N"].ToString(), out i);
                    else
                        int.TryParse(data.ToString(), out i);
                }
                else
                {
                    int.TryParse(data.ToString(), out i);
                }

                return (T)Convert.ChangeType(i, typeof(int));
            }
            else if (typeof(T) == typeof(float))
            {
                float f = 0.0f;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        float.TryParse(data["S"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out f);
                    else if (data.ContainsKey("N"))
                        float.TryParse(data["N"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out f);
                    else
                        float.TryParse(data.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out f);
                }
                else
                {
                    float.TryParse(data.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out f);
                }

                return (T)Convert.ChangeType(f, typeof(float));
            }
            else if (typeof(T) == typeof(long))
            {
                long l = 0L;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        long.TryParse(data["S"].ToString(), out l);
                    else if (data.ContainsKey("N"))
                        long.TryParse(data["N"].ToString(), out l);
                    else
                        long.TryParse(data.ToString(), out l);
                }
                else
                {
                    long.TryParse(data.ToString(), out l);
                }

                return (T)Convert.ChangeType(l, typeof(long));
            }
            else if (typeof(T) == typeof(ushort))
            {
                ushort us = 0;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        ushort.TryParse(data["S"].ToString(), out us);
                    else if (data.ContainsKey("N"))
                        ushort.TryParse(data["N"].ToString(), out us);
                    else
                        ushort.TryParse(data.ToString(), out us);
                }
                else
                {
                    ushort.TryParse(data.ToString(), out us);
                }

                return (T)Convert.ChangeType(us, typeof(ushort));
            }
            else if (typeof(T) == typeof(double))
            {
                double d = 0.0;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        double.TryParse(data["S"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d);
                    else if (data.ContainsKey("N"))
                        double.TryParse(data["N"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d);
                    else
                        double.TryParse(data.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d);
                }
                else
                {
                    double.TryParse(data.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d);
                }

                return (T)Convert.ChangeType(d, typeof(double));
            }
            else if (typeof(T) == typeof(BigDouble))
            {
                string s = string.Empty;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        s = data["S"].ToString();
                    else if (data.ContainsKey("N"))
                        s = data["N"].ToString();
                    else
                        s = data.ToString();
                }
                else
                {
                    s = data.ToString();
                }

                try
                {
                    BigDouble value = BigDouble.Parse(s);
                    return (T)(object)value;
                }
                catch
                {
                    return (T)(object)BigDouble.Zero;
                }
            }
            else if (typeof(T) == typeof(string))
            {
                string s = string.Empty;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        s = data["S"].ToString();
                    else if (data.ContainsKey("N"))
                        s = data["N"].ToString();
                    else
                        s = data.ToString();
                }
                else
                {
                    s = data.ToString();
                }

                return (T)Convert.ChangeType(s, typeof(string));
            }
            else if (typeof(T).IsEnum)
            {
                int i = 0;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        int.TryParse(data["S"].ToString(), out i);
                    else if (data.ContainsKey("N"))
                        int.TryParse(data["N"].ToString(), out i);
                    else
                        int.TryParse(data.ToString(), out i);
                }
                else
                {
                    int.TryParse(data.ToString(), out i);
                }

                return (T)Enum.Parse(typeof(T), i.ToString());
            }
            else if (typeof(T) == typeof(bool))
            {
                bool b = false;

                if (data.IsObject)
                {
                    if (data.ContainsKey("S"))
                        bool.TryParse(data["S"].ToString(), out b);
                    else if (data.ContainsKey("BOOL"))
                        bool.TryParse(data["BOOL"].ToString(), out b);
                    else
                        bool.TryParse(data.ToString(), out b);
                }
                else
                {
                    bool.TryParse(data.ToString(), out b);
                }

                return (T)Convert.ChangeType(b, typeof(bool));
            }

            return default(T);
        }

        static public Color ConvertHexcodeToColor(string hexCode)
        {
            string[] checkSharps = hexCode.Split('#');
            if (checkSharps.Length < 2) hexCode = Append("#", hexCode);

            for (int i = hexCode.Length; i < 9; i++)
            {
                hexCode = Append(hexCode, "F");
            }

            Color color;
            ColorUtility.TryParseHtmlString(hexCode, out color);
            return color;
        }
        static public string ConvertColorToHexCode(Color color)
        {
            // Color blend = Color.white;
            int r = (int)(color.r * 255);
            int g = (int)(color.g * 255);
            int b = (int)(color.b * 255);
            int a = (int)(color.a * 255);

            string hexCC = "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + a.ToString("X2");
            return hexCC;
        }
        static public string ConvertTimeToString(float fTotalSeconds, int _unit = 2)
        {
            var totalSeconds = fTotalSeconds;
            string strTime = string.Empty;
            if (_unit == 4)
            {
                var dd = Math.Floor(totalSeconds / 60 / 60 / 24).ToString("00");
                var hh = (Math.Floor(totalSeconds / 60 / 60) % 24).ToString("00");
                var mm = (Math.Floor(totalSeconds / 60) % 60).ToString("00");
                var ss = Convert.ToInt32(totalSeconds % 60).ToString("00");
                strTime = string.Format("{0}:{1}:{2}:{3}", dd, hh, mm, ss);
            }
            else if (_unit == 3)
            {
                var hh = Math.Floor(totalSeconds / 60 / 60).ToString("00");
                var mm = (Math.Floor(totalSeconds / 60) % 60).ToString("00");
                var ss = Convert.ToInt32(totalSeconds % 60).ToString("00");
                strTime = string.Format("{0}:{1}:{2}", hh, mm, ss);
            }
            else if (_unit == 2)
            {
                var mm = Math.Floor(totalSeconds / 60).ToString("00");
                var ss = Convert.ToInt32(totalSeconds % 60).ToString("00");
                strTime = string.Format("{0}:{1}", mm, ss);
            }
            else
            {
                var ss = Convert.ToInt32(totalSeconds % 60).ToString("00");
                strTime = string.Format("{0}", ss);
            }

            return strTime;
        }
        static public string ConvertDetailTimeToString(float fTotalSeconds, int _unit = 3)
        {
            var totalSeconds = fTotalSeconds;
            string strTime = string.Empty;
            if (_unit == 4)
            {
                var hh = Math.Truncate(totalSeconds / 60 / 60).ToString("00");
                var mm = Math.Truncate(totalSeconds / 60 % 60).ToString("00");
                var ss = Math.Floor(totalSeconds % 60).ToString("00");
                var milisec = (totalSeconds % 1).ToString(".000");
                strTime = string.Format("{0}:{1}:{2}{3}", hh, mm, ss, milisec);
            }
            else if (_unit == 3)
            {
                var mm = Math.Floor(totalSeconds / 60).ToString("00");
                var ss = Math.Floor(totalSeconds % 60).ToString("00");
                var milisec = (totalSeconds % 1).ToString(".000");
                strTime = string.Format("{0}:{1}{2}", mm, ss, milisec);
            }
            else
            {
                var ss = Math.Floor(totalSeconds % 60).ToString("00");
                var milisec = (totalSeconds % 1).ToString(".000");
                strTime = string.Format("{0}{1}", ss, milisec);
            }

            return strTime;
        }
        static public void Debug_Log(object message)
        {
            if (Debug.isDebugBuild)
                Debug.Log(message);
        }
        static public void Debug_Log(object message, UnityEngine.Object context)
        {
            if (Debug.isDebugBuild)
                Debug.Log(message, context);
        }

        static public void Debug_LogError(object message)
        {
            if (Debug.isDebugBuild)
                Debug.LogError(message);
        }

        static public T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            if (obj == null)
                return default(T);

            T t = obj.GetComponent<T>();
            if (t == null)
                t = obj.AddComponent<T>();

            return t;
        }

        public static Vector3 GetRatioScale(RectTransform canvasRect)
        {
            Vector3 v3DefaultScale = Vector3.one;
            // GameObject obj = GameObject.Find("Canvas");
            // Canvas canvas = obj.GetComponent<Canvas>();
            // RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            float defaultRatio = 1920f / 1080f;
            float canvasRatio = canvasRect.rect.width / canvasRect.rect.height;
            float ratio = canvasRatio / defaultRatio;

            if (ratio < 1)
            {
                // ratio = defaultRatio / canvasRatio;
            }

            return (v3DefaultScale * ratio);
        }

        public static bool IsNull(this UnityEngine.Object obj)
        {
            return ReferenceEquals(obj, null);
        }

        public static bool IsFakeNull(this UnityEngine.Object obj)
        {
            return !ReferenceEquals(obj, null) && obj;
        }

        public static bool IsAssigned(this UnityEngine.Object obj)
        {
            return obj;
        }

        public static Vector2 GetInsideUnitCircle(float radius)
        {
            return UnityEngine.Random.insideUnitCircle * radius;
        }
        public static Vector2 GetInsideUnitSphere(float radius)
        {
            return UnityEngine.Random.insideUnitSphere * radius;
        }

        public static int GetRandomRange(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }
        public static float GetRandomRange(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
        public static bool GetRandomResult(float fPercent)
        {
            if (fPercent <= 0)
                return false;
            else if (fPercent >= 100)
                return true;

            float fRandNum = GetRandomRange(0f, 100f);
            return fRandNum < fPercent;
        }

        public static CountryCode StrCountryCodeToCountryCode(string countryCode)
        {
            if (countryCode == "KR") return CountryCode.SouthKorea;
            else if (countryCode == "JP") return CountryCode.Japan;
            else if (countryCode == "ID") return CountryCode.Indonesia;
            else if (countryCode == "AU") return CountryCode.Australia;
            else if (countryCode == "US") return CountryCode.UnitedStates;
            else if (countryCode == "TW") return CountryCode.Taiwan;
            else return CountryCode.UnitedStates;
        }

        public static bool CheckNumberAndSpecialChar(string str)
        {
            Regex numRegex = new Regex(@"[0-9]");
            bool isMatchNum = false; // numRegex.IsMatch(str);
            Regex specialRegex = new Regex(@"[~!@\#$%^&*\()\=+|\\/:;?""<>']");
            bool isMatchSpecial = specialRegex.IsMatch(str);

            if (isMatchNum || isMatchSpecial)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string SmartEscapeURL(string url)
        {
            string escapedArgs = "";
            string[] parts = url.Split('?');
            if (parts.Length == 1) return url;
            string[] args = parts[1].Split('&');
            for (int i = 0; i < args.Length; i++)
            {
                string[] kvp = args[i].Split('=');
                if (kvp.Length == 2)
                {
                    escapedArgs += string.Format("{0}={1}", UnityWebRequest.EscapeURL(kvp[0]).Replace("+", "%20"), UnityWebRequest.EscapeURL(kvp[1]).Replace("+", "%20"));

                    if (i < args.Length - 1) escapedArgs += "&";

                }
            }
            string eUrl = parts[0] + "?" + escapedArgs;
            return eUrl;
        }

        public static int Sigma(int startNum, int endNum)
        {
            int iTotalNum = 0;
            for (int i = startNum; i <= endNum; i++)
                iTotalNum += i;

            return iTotalNum;
        }

        public static string[] SplitCSVFile(string line)
        {
            List<string> values = new List<string>();
            bool inQuotes = false;
            string currentValue = "";
            foreach (var c in line)
            {
                if (c == ',')
                {
                    if (inQuotes)
                    {
                        currentValue += c;
                    }
                    else
                    {
                        values.Add(currentValue);
                        currentValue = string.Empty;
                    }
                }
                else if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else
                {
                    currentValue += c;
                }
            }

            values.Add(currentValue);
            return values.ToArray();
        }
        public static bool CheckTargetInEllipseSector(Vector2 targetWorld, Transform frame, float a, float b,Vector2 forwardWorld, float sightAngleDeg)
        {
            // 1) 월드→로컬, 타원→단위원 정규화
            Vector2 pLocal = frame.InverseTransformPoint(targetWorld);
            float x = pLocal.x / a;
            float y = pLocal.y / b;

            // 타원 내부(단위원) 여부
            float r2 = x * x + y * y;
            if (r2 > 1f) return false; // 타원 밖이면 탈락

            // 2) forward도 같은 방식으로 정규화한 좌표계에서 각도 비교
            Vector2 fLocal = frame.InverseTransformDirection(forwardWorld);
            Vector2 pDir = new Vector2(x, y).normalized;
            Vector2 fDir = new Vector2(fLocal.x / a, fLocal.y / b).normalized;

            // acos 대신 dot >= cos(threshold)로 비교 (빠름)
            float cosThresh = Mathf.Cos(0.5f * sightAngleDeg * Mathf.Deg2Rad);
            float dot = Mathf.Clamp(Vector2.Dot(pDir, fDir), -1f, 1f);
            return dot >= cosThresh || float.IsNaN(dot);
        }
        public static bool CheckTargetInSight(Vector2 player, Vector2 target, Vector3 forward, float sightAngle)
        {
            bool isbool = false;

            Vector2 interV = (target - player).normalized;
            // '타겟-나 벡터'와 '내 정면 벡터'를 내적
            float dot = Vector3.Dot(interV, forward); // transform.up
            // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
            float theta = Mathf.Acos(dot);
            // angleRange와 비교하기 위해 degree로 변환
            float degree = Mathf.Rad2Deg * theta;

            // 시야각 판별
            if (degree <= (sightAngle / 2f) || float.IsNaN(degree))
            {
                isbool = true;
            }

            return isbool;
        }

        public static float DirToAngle(Vector3 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // if (angle < 0) angle += 360;
            return angle;
        }
        public static Quaternion AngleToQuarternion(float angle)
        {
            return Quaternion.AngleAxis(angle + 180, Vector3.forward);
        }
        public static Vector3 AngleToDirection(float degree)
        {
            float radians = degree * (Mathf.PI / 180);
            Vector3 degreeVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            return degreeVector;
        }
        public static Quaternion DirToQuarternion(Vector3 dir)
        {
            return Quaternion.AngleAxis(DirToAngle(dir), Vector3.forward);
        }

        public static bool IsClientVersionLow(string serverVer, string clientVer)
        {
            if (clientVer.Equals(serverVer))
                return false;

            bool isLowVersion = true;
            string[] serverVerCuts = serverVer.Split('.');
            string[] clientVerCuts = clientVer.Split('.');
            for (int i = 0; i < clientVerCuts.Length; i++)
            {
                int serverCut = TryParse<int>(serverVerCuts[i]);
                int clientCut = TryParse<int>(clientVerCuts[i]);
                if (serverCut == clientCut)
                    continue;

                if (serverCut < clientCut)
                {
                    isLowVersion = false;
                    break;
                }
                else
                {
                    isLowVersion = true;
                    break;
                }
            }
            return isLowVersion;
        }
        public static bool IsClientVersionLowOrEqual(string serverVer, string clientVer)
        {
            if (clientVer.Equals(serverVer))
                return true;

            bool isLowVersion = true;
            string[] serverVerCuts = serverVer.Split('.');
            string[] clientVerCuts = clientVer.Split('.');
            for (int i = 0; i < clientVerCuts.Length; i++)
            {
                int serverCut = TryParse<int>(serverVerCuts[i]);
                int clientCut = TryParse<int>(clientVerCuts[i]);
                if (serverCut == clientCut)
                    continue;

                if (serverCut < clientCut)
                {
                    isLowVersion = false;
                    break;
                }
                else
                {
                    isLowVersion = true;
                    break;
                }
            }
            return isLowVersion;
        }

        public static string GetGradeTypeName(GradeType _gradeType)
        {
            SystemLanguage systemLanguage = StringManager.Instance.GetLanguageType();
            if (systemLanguage == SystemLanguage.Korean)
            {
                switch (_gradeType)
                {
                    //case GradeType.None:
                    //    return string.Empty;
                    //case GradeType.Common:
                    //    return "일반";
                    //case GradeType.Uncommon:
                    //    return "고급";
                    //case GradeType.Rare:
                    //    return "희귀";
                    //case GradeType.Unique:
                    //    return "유일";
                    //case GradeType.Epic:
                    //    return "영웅";
                    //case GradeType.Legendary:
                    //    return "전설";
                    //case GradeType.Mythical:
                    //    return "신화";
                    default:
                        break;
                }
            }
            return _gradeType.ToString();
        }
        public static Color GetGradeColor(GradeType _gradeType)
        {
            Color color = Color.white;
            switch (_gradeType)
            {
                case GradeType.None:
                    return Color.white;
                case GradeType.D:
                    return ConvertHexcodeToColor("D9D9D9");
                case GradeType.C:
                    return ConvertHexcodeToColor("00D90E");
                case GradeType.B:
                    return ConvertHexcodeToColor("007ED9");
                case GradeType.A:
                    return ConvertHexcodeToColor("BC00D9");
                case GradeType.S:
                    return ConvertHexcodeToColor("FF7F00");
                case GradeType.SS:
                    return ConvertHexcodeToColor("FF1200");
                case GradeType.SSS:
                    return ConvertHexcodeToColor("FFDD00");
                case GradeType.US:
                    return ConvertHexcodeToColor("00FFF4");
                case GradeType.USS:
                    return ConvertHexcodeToColor("D16988");
                default:
                    return ConvertHexcodeToColor("D16988");
                    break;
            }
            //switch (_gradeType)
            //{
            //    case GradeType.None:
            //        return Color.white;
            //    case GradeType.Common:
            //        return ConvertHexcodeToColor("6EE8B8");
            //    case GradeType.Uncommon:
            //        return ConvertHexcodeToColor("FCE047");
            //    case GradeType.Rare:
            //        return ConvertHexcodeToColor("F69F0C");
            //    case GradeType.Unique:
            //        return ConvertHexcodeToColor("F4405F"); 
            //    case GradeType.Epic:
            //        return ConvertHexcodeToColor("21D2EE");
            //    case GradeType.Legendary:
            //        return ConvertHexcodeToColor("674DA8");
            //    case GradeType.Mythical:
            //        return ConvertHexcodeToColor("1254CB");
            //    default:
            //        break;
            //}
            return color;
        }
        public static string GetGradeColorCode(GradeType _gradeType)
        {
            switch (_gradeType)
            {
                case GradeType.None:
                    return "000000";
                case GradeType.D:
                    return "D9D9D9";
                case GradeType.C:
                    return "00D90E";
                case GradeType.B:
                    return "007ED9";
                case GradeType.A:
                    return "BC00D9";
                case GradeType.S:
                    return "FF7F00";
                case GradeType.SS:
                    return "FF1200";
                case GradeType.SSS:
                    return "FFDD00";
                case GradeType.US:
                    return "00FFF4";
                case GradeType.USS:
                    return "D16988";
                default:
                    return "D16988";
                    break;
            }
            //switch (_gradeType)
            //{
            //    case GradeType.None:
            //        return Color.white;
            //    case GradeType.Common:
            //        return ConvertHexcodeToColor("6EE8B8");
            //    case GradeType.Uncommon:
            //        return ConvertHexcodeToColor("FCE047");
            //    case GradeType.Rare:
            //        return ConvertHexcodeToColor("F69F0C");
            //    case GradeType.Unique:
            //        return ConvertHexcodeToColor("F4405F"); 
            //    case GradeType.Epic:
            //        return ConvertHexcodeToColor("21D2EE");
            //    case GradeType.Legendary:
            //        return ConvertHexcodeToColor("674DA8");
            //    case GradeType.Mythical:
            //        return ConvertHexcodeToColor("1254CB");
            //    default:
            //        break;
            //}
            return "000000";
        }
        public static string GetGradeBgName(GradeType _gradeType)
        {
            switch (_gradeType)
            {
                //case GradeType.None:
                //    return "ui_grade_slot_01";
                //case GradeType.Common:
                //    return "ui_grade_slot_01";
                //case GradeType.Uncommon:
                //    return "ui_grade_slot_02";
                //case GradeType.Rare:
                //    return "ui_grade_slot_03";
                //case GradeType.Unique:
                //    return "ui_grade_slot_04";
                //case GradeType.Epic:
                //    return "ui_grade_slot_05";
                //case GradeType.Legendary:
                //    return "ui_grade_slot_06";
                //case GradeType.Mythical:
                //    return "ui_grade_slot_07";
                default:
                    break;
            }
            return "ui_grade_slot_01";
        }
        public static string GetGuildMarkSpriteName(int mark)
        {
            string iconName = Append("icon_guild_", mark.ToString("000"));
            return iconName;
        }
        public static string GetGuildMarkBgSpriteName(int mark)
        {
            string iconName = Append("guild_bg_", mark.ToString("000"));
            return iconName;
        }
        public static string GetStageStringText(int stageLevel)
        {
            int worldId = ((stageLevel - 1) / 100) + 1;
            int stageNumber = ((stageLevel - 1) % 100) + 1;
            return string.Format("{0}-{1}", worldId, stageNumber);
        }
        public static string GetRankRewardStringText(string _startRank, string _endRank, SystemLanguage systemLanguage)
        {
            if (systemLanguage == SystemLanguage.Korean)
            {
                int startRank = TryParse<int>(_startRank);
                int endRank = TryParse<int>(_endRank);
                if (startRank == 1)
                    return string.Format("{0}등", startRank);
                else if (startRank == 2)
                    return string.Format("{0}등", startRank);
                else if (startRank == 3)
                    return string.Format("{0}등", startRank);
                else if (startRank == 0)
                    return StringManager.Instance.GetStringText("Name_RankRewardAll");
                else
                {
                    return string.Format("{0}~{1}등", startRank, endRank);
                }
            }
            else
            {
                int startRank = TryParse<int>(_startRank);
                int endRank = TryParse<int>(_endRank);
                if (startRank == 1)
                    return string.Format("{0}st", startRank);
                else if (startRank == 2)
                    return string.Format("{0}nd", startRank);
                else if (startRank == 3)
                    return string.Format("{0}rd", startRank);
                else if (startRank == 0)
                    return StringManager.Instance.GetStringText("Name_RankRewardAll");
                else
                {
                    return string.Format("{0}~{1}th", startRank, endRank);
                }
            }
        }
        public static string GetRankStringText(string rank, SystemLanguage systemLanguage)
        {
            if (systemLanguage == SystemLanguage.Korean)
            {
                int iRank = TryParse<int>(rank);
                if (iRank == 1)
                    return string.Format("{0}등", rank);
                else if (iRank == 2)
                    return string.Format("{0}등", rank);
                else if (iRank >= 3 && iRank <= 5)
                    return string.Format("{0}등", rank);
                else if (iRank >= 6 && iRank <= 10)
                    return string.Format("{0}등", rank);
                else if (iRank >= 11 && iRank <= 50)
                    return string.Format("{0}등", rank);
                else if (iRank >= 51 && iRank <= 100)
                    return string.Format("{0}등", rank);
                else
                    return string.Format("{0}위", rank);
            }
            else
            {
                int iRank = TryParse<int>(rank);
                if (iRank == 1)
                    return string.Format("{0}st", rank);
                else if (iRank == 2)
                    return string.Format("{0}nd", rank);
                else if (iRank >= 3 && iRank <= 5)
                    return string.Format("{0}rd", rank);
                else if (iRank >= 6 && iRank <= 10)
                    return string.Format("{0}th", rank);
                else if (iRank >= 11 && iRank <= 50)
                    return string.Format("{0}th", rank);
                else if (iRank >= 51 && iRank <= 100)
                    return string.Format("{0}th", rank);
                else
                    return string.Format("{0}th", rank);
            }
        }
        public static string GetMapObjectName(int mapObjectId)
        {
            if (mapObjectId == 1) return "tree_1";
            else if (mapObjectId == 2) return "tree_2";
            else if (mapObjectId == 101) return "weed_2";
            else if (mapObjectId == 102) return "weed_2";
            else if (mapObjectId == 201) return "rock";
            else if (mapObjectId == 301) return "tomb_1";

            return string.Empty;
        }

        //public static double GetDefPower(Status status)
        //{
        //    double cp = ((status.TotalHp * 1) *
        //        status.TotalDefence);

        //    return cp;
        //}
        public static double Lerp(double a, double b, float t)
        {
            return a + (b - a) * Mathf.Clamp01(t);
        }

        public static Vector3 GetCirclePoint(Vector3 centerPos, float dist, float angle)
        {
            Vector3 pos = new Vector3();
            pos.x = centerPos.x + (dist * Mathf.Cos(angle / (180f / Mathf.PI)));
            pos.y = centerPos.y + (dist * Mathf.Sin(angle / (180f / Mathf.PI)));
            pos.z = 0;// centerPos.z + (dist * Mathf.Sin(angle / (180f / Mathf.PI)));

            return pos;
        }
        public static Vector2 GetSafeRandomPos(RectTransform canvasRect, float margin)
        {
            Vector2 size = canvasRect.rect.size;

            float halfX = size.x * 0.5f - margin;
            float halfY = size.y * 0.5f - margin;

            float x = UnityEngine.Random.Range(-halfX, halfX);
            float y = UnityEngine.Random.Range(-halfY, halfY);

            return new Vector2(x, y);
        }
        public static bool IsInsideLocalEllipse(Vector2 worldPoint, Transform frame, float a, float b)
        {
            Vector2 p = frame.InverseTransformPoint(worldPoint);
            return (p.x * p.x) / (a * a) + (p.y * p.y) / (b * b) <= 1f;
        }
        public static Vector3 WorldPointToCanvasPoint(Vector3 worldPoint, Canvas canvas, Camera camera)
        {
            //first you need the RectTransform component of your canvas
            RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

            Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPoint);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //now you can set the position of the ui element
            return WorldObject_ScreenPosition;
        }
        public static Vector3 WorldPointToCanvasPoint(Vector3 worldPoint, RectTransform canvasRect, Camera camera)
        {
            //first you need the RectTransform component of your canvas
            RectTransform CanvasRect = canvasRect;

            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

            Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPoint);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //now you can set the position of the ui element
            return WorldObject_ScreenPosition;
        }
        public static string GetRuneColorName(int _color)
        {
            // 1 : 최종 데미지 증가
            // 2 : 머리 치명타 확률
            // 3 : 검 치명타 확률
            // 4 : 스킬 치명타 확률
            if (_color == 1) return "분노의 기운";
            else if (_color == 2) return "질투의 기운";
            else if (_color == 3) return "탐욕의 기운";
            else if (_color == 4) return "폭식의 기운";

            return string.Empty;
        }
        public static Color GetRuneColor(int _color)
        {
            if (_color == 1) return new Color32(251, 236, 93, 255);
            else if (_color == 2) return new Color32(244, 164, 96, 255);
            else if (_color == 3) return new Color32(241, 91, 91, 255);
            else if (_color == 4) return new Color32(91, 198, 208, 255);
            return Color.white;
        }
        public static string GetRuneColorCode(int _color)
        {
            if (_color == 1) return "FBEC5D";
            else if (_color == 2) return "FF7FFC";
            else if (_color == 3) return "F15B5B";
            else if (_color == 4) return "5BC6D0";
            return "FBEC5D";
        }
        public static double ConvertBigDoubleRankScore(BigDouble originValue)
        {
            if (originValue <= BigDouble.Zero)
                return 0;

            if (originValue.Exponent < 15)
            {
                double value = originValue.Mantissa * Math.Pow(10, originValue.Exponent);
                return Math.Log(value + 1);
            }

            return Math.Log(originValue.Mantissa) + originValue.Exponent * Math.Log(10);
        }

        public static BigDouble RestoreBigDoubleRankScore(double rankScore)
        {
            if (rankScore <= 0)
                return BigDouble.Zero;

            if (rankScore < 700)
                return new BigDouble(Math.Exp(rankScore) - 1);

            double base10Value = rankScore / Math.Log(10);
            int exponent = (int)Math.Floor(base10Value);
            double mantissa = Math.Pow(10, base10Value - exponent);

            return new BigDouble(mantissa, exponent);
        }
        public static float ComputeFillByMilestones(float cur, float[] milestones)
        {
            if (milestones == null || milestones.Length == 0)
                return 0f;

            // bounds = [0, milestones...]
            int segments = milestones.Length; // 5등분
            float[] bounds = new float[segments + 1];
            bounds[0] = 0f;
            for (int i = 0; i < milestones.Length; i++)
                bounds[i + 1] = milestones[i];

            // 범위 밖 클램프
            if (cur <= bounds[0]) return 0f;
            if (cur >= bounds[bounds.Length - 1]) return 1f;

            // 속한 구간 찾기
            int idx = 0;
            for (; idx < segments; idx++)
            {
                if (cur <= bounds[idx + 1])
                    break;
            }

            // 구간 내 진행도(0~1)
            float t = Mathf.InverseLerp(bounds[idx], bounds[idx + 1], cur);

            // 등분 매핑: 구간당 폭 = 1/segments (여기선 0.2)
            return (idx + t) / segments;
        }
        public static void FitToParent(Spine.Unity.SkeletonGraphic sg, RectTransform parent, Vector2 referenceSize)
        {
            if (!sg || !parent) return;
            var rt = (RectTransform)sg.transform;

            // 부모 전체에 스트레치
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            if (!sg.IsValid) sg.Initialize(true);

            // 부모 실제 픽셀 크기
            var ps = parent.rect.size;

            // 레퍼런스(디자인 기준) 대비로 XY 스케일 각각 계산
            float sx = ps.x / Mathf.Max(1f, referenceSize.x);
            float sy = ps.y / Mathf.Max(1f, referenceSize.y);

            rt.localScale = new Vector3(sx, sy, 1f); // ★ 비율 무시하고 늘리기/줄이기
            rt.anchoredPosition = Vector2.zero;
        }
        public static void EnterPip(int w = 16, int h = 9)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        // [체크 1] 기기 자체가 하드웨어/OS 레벨에서 PiP를 지원하는지 검증
        using var packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
        bool hasPipSupport = packageManager.Call<bool>("hasSystemFeature", "android.software.picture_in_picture");

        if (!hasPipSupport)
        {
            Debug.LogWarning("이 기기는 OS 또는 하드웨어 사양(RAM 등) 제한으로 PiP를 지원하지 않습니다.");
                    GameManager.Instance.UISystemScene.ShowToastMessagePanel(StringManager.Instance.GetStringText("Msg_Unsupported device."));
                    return;
        }
        using var version = new AndroidJavaClass("android.os.Build$VERSION");
        int sdkInt = version.GetStatic<int>("SDK_INT");
        // 안드로이드 8.0(SDK 26) 미만은 PiP 지원 불가
        if (sdkInt < 26) 
        {
                    GameManager.Instance.UISystemScene.ShowToastMessagePanel(StringManager.Instance.GetStringText("Msg_Unsupported device."));
                    return;
        }
        using var builder = new AndroidJavaObject("android.app.PictureInPictureParams$Builder");
        using var rational = new AndroidJavaObject("android.util.Rational", w, h);
        builder.Call<AndroidJavaObject>("setAspectRatio", rational);
        // [체크 2] 안드로이드 12(SDK 31) 이상 처리 최적화
        if (sdkInt >= 31)
        {
            // 사용자가 홈 버튼을 누르거나 제스처로 나갈 때 자동으로 PiP가 켜지도록 설정
            builder.Call<AndroidJavaObject>("setAutoEnterEnabled", true);

            using var pipParams = builder.Call<AndroidJavaObject>("build");
            activity.Call("setPictureInPictureParams", pipParams);

            // ★ 중요: Android 12 이상에서 setAutoEnterEnabled를 켰다면, 
            // 버튼을 눌러 "즉시" 진입하는 상황이 아니라면 enterPictureInPictureMode를 중복 호출하지 않는 것이 안전합니다.
            // 만약 버튼 클릭 시 즉시 진입하게 하고 싶다면 아래 enterPictureInPictureMode를 유지하되 제조사 버그를 감안해야 합니다.
            bool result = activity.Call<bool>("enterPictureInPictureMode", pipParams);
            Debug.Log($"Android 12+ 수동 PiP 진입 시도 결과: {result}");
        }
        else
        {
            // 안드로이드 8.0 ~ 11 버전 처리
            using var pipParams = builder.Call<AndroidJavaObject>("build");
            bool result = activity.Call<bool>("enterPictureInPictureMode", pipParams);
            Debug.Log($"Android 8~11 PiP 진입 시도 결과: {result}");
        }
    }
    catch (System.Exception e)
    {
        Debug.LogWarning($"Enter PiP 예외 발생: {e}");
                GameManager.Instance.UISystemScene.ShowToastMessagePanel(StringManager.Instance.GetStringText("Msg_Unsupported device."));
    }
#endif
        }
        public static string GenerateRandomNickname(int length = 16)
        {
            StringBuilder sb = new StringBuilder(length);
            System.Random random = new System.Random();

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }

            return sb.ToString();
        }
        public static double ParsePrice(string priceText)
        {
            if (string.IsNullOrEmpty(priceText))
                return 0;

            priceText = priceText
                .Replace(",", "")
                .Replace("₩", "")
                .Replace("$", "")
                .Replace("원", "")
                .Trim();

            if (double.TryParse(
                priceText,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double price))
            {
                return price;
            }

            Debug.LogWarning($"[Airbridge] Price parse failed: {priceText}");
            return 0;
        }
        public static bool CheckStore()
        {
#if UNITY_EDITOR
            // 에디터 환경에서는 항상 true
            return true;
#elif UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = context.Call<AndroidJavaObject>("getPackageManager");
            string installer = packageManager.Call<string>(
                "getInstallerPackageName",
                context.Call<string>("getPackageName")
            );

            return installer == "com.android.vending" // 구글 플레이스토어
                || installer == "com.skt.skaf.A000Z00040" // 원스토어
                || installer == "com.kt.olleh.storefront"
                || installer == "com.lguplus.appstore";
        }
#else
        // 기타 플랫폼에서는 false 반환
        return false;
#endif
        }

        // 보이는 몸(지금 body 첨부 그림의 불투명 영역, 텍스처 Read/Write 필요)의 높이가 _height(캔버스 px)가 되게 배율을 넣고,
        // 몸 아래 가운데를 목표 점에 두려면 그래픽 위치에 더할 값(캔버스 px)을 돌려줌. _flip은 좌우 반전
        public static Vector2 FitSkeletonBody(SkeletonGraphic _graphic, float _height, bool _flip)
        {
            var attachment = (Spine.RegionAttachment)_graphic.Skeleton.FindSlot("body").Attachment;
            var region = (Spine.AtlasRegion)attachment.Region;
            var texture = (Texture2D)((Material)region.page.rendererObject).mainTexture;
            // 아틀라스 y는 위 기준, GetPixels는 아래 기준
            Color[] pixels = texture.GetPixels(region.x, texture.height - region.y - region.height, region.width, region.height);
            int minX = region.width, maxX = -1, minY = region.height, maxY = -1;
            for (int y = 0; y < region.height; y++)
                for (int x = 0; x < region.width; x++)
                    if (pixels[y * region.width + x].a > 0f)
                    {
                        minX = Mathf.Min(minX, x);
                        maxX = Mathf.Max(maxX, x);
                        minY = Mathf.Min(minY, y);
                        maxY = Mathf.Max(maxY, y);
                    }
            // 스프라이트 1px의 스켈레톤 길이, SkeletonGraphic은 스켈레톤 1을 캔버스 100px로 그림
            float unit = attachment.Width / region.width;
            float scale = _height / ((maxY - minY + 1) * unit * 100f);
            _graphic.rectTransform.localScale = new Vector3(_flip ? -scale : scale, scale, 1f);
            // 몸 아래 가운데의 스켈레톤 좌표 (첨부 그림은 X·Y가 가운데)
            float footX = attachment.X + ((minX + maxX + 1) * 0.5f - region.width * 0.5f) * unit;
            float footY = attachment.Y + (minY - region.height * 0.5f) * unit;
            return new Vector2((_flip ? footX : -footX) * 100f * scale, -footY * 100f * scale);
        }
    }
}
