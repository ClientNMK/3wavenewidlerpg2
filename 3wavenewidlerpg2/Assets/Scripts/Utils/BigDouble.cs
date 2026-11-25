using System;
using System.Globalization;

[Serializable]
public struct BigDouble
{
    public double Mantissa;
    public int Exponent;

    public static BigDouble Zero => new BigDouble(0);

    public BigDouble(double value)
    {
        Mantissa = value;
        Exponent = 0;
        Normalize();
    }

    public BigDouble(double mantissa, int exponent)
    {
        Mantissa = mantissa;
        Exponent = exponent;
        Normalize();
    }

    void Normalize()
    {
        if (Mantissa == 0)
        {
            Mantissa = 0;
            Exponent = 0;
            return;
        }

        if (double.IsNaN(Mantissa) || double.IsInfinity(Mantissa))
            return;

        int moveExponent = (int)Math.Floor(Math.Log10(Math.Abs(Mantissa)));

        Mantissa /= Math.Pow(10, moveExponent);
        Exponent += moveExponent;
    }

    public static BigDouble operator *(BigDouble value, double multiplier)
    {
        if (value.Mantissa == 0 || multiplier == 0)
            return Zero;

        int multiplierExponent = (int)Math.Floor(Math.Log10(Math.Abs(multiplier)));
        double multiplierMantissa = multiplier / Math.Pow(10, multiplierExponent);

        return new BigDouble(
            value.Mantissa * multiplierMantissa,
            value.Exponent + multiplierExponent
        );
    }

    public static BigDouble operator +(BigDouble left, BigDouble right)
    {
        if (left.Mantissa == 0)
            return right;

        if (right.Mantissa == 0)
            return left;

        int exponentDifference = left.Exponent - right.Exponent;

        if (exponentDifference >= 16)
            return left;

        if (exponentDifference <= -16)
            return right;

        if (left.Exponent >= right.Exponent)
        {
            double adjustedRight = right.Mantissa * Math.Pow(10, -exponentDifference);
            return new BigDouble(left.Mantissa + adjustedRight, left.Exponent);
        }

        double adjustedLeft = left.Mantissa * Math.Pow(10, exponentDifference);
        return new BigDouble(adjustedLeft + right.Mantissa, right.Exponent);
    }

    public static BigDouble operator -(BigDouble left, BigDouble right)
    {
        return left + new BigDouble(-right.Mantissa, right.Exponent);
    }
    public static BigDouble operator -(BigDouble value)
    {
        return new BigDouble(-value.Mantissa, value.Exponent);
    }
    public static bool operator >(BigDouble left, BigDouble right)
    {
        return Compare(left, right) > 0;
    }

    public static bool operator <(BigDouble left, BigDouble right)
    {
        return Compare(left, right) < 0;
    }

    public static bool operator >=(BigDouble left, BigDouble right)
    {
        return Compare(left, right) >= 0;
    }

    public static bool operator <=(BigDouble left, BigDouble right)
    {
        return Compare(left, right) <= 0;
    }

    public static bool operator ==(BigDouble left, BigDouble right)
    {
        return left.Mantissa == right.Mantissa && left.Exponent == right.Exponent;
    }

    public static bool operator !=(BigDouble left, BigDouble right)
    {
        return !(left == right);
    }
    public static BigDouble Truncate(BigDouble value)
    {
        if (value.Mantissa == 0)
            return Zero;

        return new BigDouble(Math.Truncate(value.Mantissa), value.Exponent);
    }
    static int Compare(BigDouble left, BigDouble right)
    {
        if (left.Mantissa >= 0 && right.Mantissa < 0)
            return 1;

        if (left.Mantissa < 0 && right.Mantissa >= 0)
            return -1;

        if (left.Mantissa == 0 && right.Mantissa == 0)
            return 0;

        bool isNegative = left.Mantissa < 0;

        if (left.Exponent != right.Exponent)
        {
            int result = left.Exponent.CompareTo(right.Exponent);
            return isNegative ? -result : result;
        }

        return left.Mantissa.CompareTo(right.Mantissa);
    }
    public double ToDouble()
    {
        return Mantissa * Math.Pow(10, Exponent);
    }
    public override bool Equals(object obj)
    {
        return obj is BigDouble value && this == value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Mantissa, Exponent);
    }

    public override string ToString()
    {
        if (Mantissa == 0)
            return "0";

        return $"{Mantissa:0.###}e{Exponent}";
    }
    public static double DivideToDouble(BigDouble left, BigDouble right)
    {
        if (right.Mantissa == 0)
            return 0;

        if (left.Mantissa == 0)
            return 0;

        int exp = left.Exponent - right.Exponent;

        // 현재 HP가 최대 HP보다 훨씬 크면
        if (exp > 15)
            return 1;

        // 현재 HP가 최대 HP보다 훨씬 작으면
        if (exp < -15)
            return 0;

        return (left.Mantissa / right.Mantissa) * Math.Pow(10, exp);
    }
    public static BigDouble Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Zero;

        value = value.Trim();

        // e 표기 먼저 처리
        int eIndex = value.IndexOfAny(new[] { 'e', 'E' });

        if (eIndex >= 0)
        {
            double mantissa = double.Parse(
                value.Substring(0, eIndex),
                CultureInfo.InvariantCulture);

            int exponent = int.Parse(
                value.Substring(eIndex + 1),
                CultureInfo.InvariantCulture);

            return new BigDouble(mantissa, exponent);
        }

        // 일반 숫자
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double normal))
            return new BigDouble(normal);

        throw new FormatException($"Invalid BigDouble : {value}");
    }
}
