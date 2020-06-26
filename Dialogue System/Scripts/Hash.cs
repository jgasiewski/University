using System;

class Hash
{
    const int defaultVal = 7;
    const int seed = 31;

    int value;

    public Hash(string str = null)
    {
        value = defaultVal;

        if (str != null)
        {
            foreach (char c in str)
            {
                Append(c);
            }
        }
    }

    public int Append(char c)
    {
        value = value * seed + c;
        return value;
    }

    public static implicit operator int(Hash v)
    {
        if (v == null)
        {
            return -1;
        }
        return v.value;
    }
}
