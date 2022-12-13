namespace MTDecrypt;
public class MT19937
{
    private const int N = 624;
    private const int M = 397;
    private const uint MATRIX_A = 0x9908B0DF;
    private const uint UPPER_MASK = 0x80000000;
    private const uint LOWER_MASK = 0X7FFFFFFF;

    private readonly int[] mt = new int[N + 1];
    private int mti = N + 1;

    public MT19937(int seed)
    {
        Init(seed);
    }
    private void Init(int seed)
    {
        mt[0] = seed;
        for (mti = 1; mti < N; mti++)
        {
            mt[mti] = 1812433253 * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti;
        }
    }

    public int Int32()
    {
        int x;
        uint[] mag01 = new uint[2] { 0x0, MATRIX_A };

        if (mti >= N)
        {
            int kk;
            if (mti == N + 1)
            {
                Init(4357);
            }
            for (kk = 0; kk < (N - M); kk++)
            {
                x = (int)(mt[kk] & UPPER_MASK | mt[kk + 1] & LOWER_MASK);
                mt[kk] = (int)(mt[kk + M] ^ (x >> 1) ^ mag01[x & 0x1]);
            }
            for (; kk < N - 1; kk++)
            {
                x = (int)(mt[kk] & UPPER_MASK | mt[kk + 1] & LOWER_MASK);
                mt[kk] = (int)(mt[kk + (M - N)] ^ (x >> 1) ^ mag01[x & 0x1]);
            }
            x = (int)(mt[N - 1] & UPPER_MASK | mt[0] & LOWER_MASK);
            mt[N - 1] = (int)((uint)mt[M - 1] ^ (x >> 1) ^ mag01[x & 0x1]);

            mti = 0;
        }

        x = mt[mti++];
        x ^= x >> 11;
        x ^= (int)((x << 7) & 0x9D2C5680);
        x ^= (int)((x << 15) & 0xEFC60000);
        x ^= x >> 18;
        return x;
    }
}