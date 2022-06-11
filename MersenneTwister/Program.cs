using System;
namespace MersenneTwister
{

	public class RandomMersenne
	{
		#region definition
		const int MERS_N = 624;
		const int MERS_M = 397;
		const int MERS_U = 11;
		const int MERS_S = 7;
		const int MERS_T = 15;
		const int MERS_L = 18;
		const uint MERS_A = 0x9908B0DF;
		const uint MERS_B = 0x9D2C5680;
		const uint MERS_C = 0xEFC60000;

		uint[] mt = new uint[MERS_N];          // durum vektörü
		uint mti;                            // mt'ye indeksledim

		#endregion

		private RandomMersenne() { }
		public RandomMersenne(uint seed)
		{
			RandomInit(seed);
		}
		public void RandomInit(uint seed)
		{
			mt[0] = seed;
			for (mti = 1; mti < MERS_N; mti++)
				mt[mti] = (1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
		}
		public void RandomInitByArray(uint[] seeds)
		{
			// 32 bitten fazla tohum oluşturmak için
			uint i, j;
			int k;
			int length = seeds.Length;
			RandomInit(19650218U);
			if (length <= 0) return;
			i = 1; j = 0;
			k = (MERS_N > length ? MERS_N : length);
			for (; k != 0; k--)
			{
				mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U)) + seeds[j] + j;
				i++; j++;
				if (i >= MERS_N) { mt[0] = mt[MERS_N - 1]; i = 1; }
				if (j >= length) j = 0;
			}
			for (k = MERS_N - 1; k != 0; k--)
			{
				mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U)) - i;
				if (++i >= MERS_N) { mt[0] = mt[MERS_N - 1]; i = 1; }
			}
			mt[0] = 0x80000000U; // MSB 1'dir; sıfır olmayan ilk diziyi sağlamak
		}
		public int IRandom(int min, int max)
		{
			// min <= x <= max aralığında rastgele tam sayı çıktısı
			int r;
			r = (int)((max - min + 1) * Random()) + min; // aralığı rastgele ile çarpın ve kısaltın
			if (r > max)
				r = max;
			if (max < min)
				return -2147483648;
			return r;
		}

		public double Random()
		{
			// 0 <= x < 1 aralığında rastgele kayan noktalı sayı çıktısı
			uint r = BRandom(); // Rastgele 32 bit al
			if (BitConverter.IsLittleEndian)
			{
				byte[] i0 = BitConverter.GetBytes((r << 20));
				byte[] i1 = BitConverter.GetBytes(((r >> 12) | 0x3FF00000));
				byte[] bytes = { i0[0], i0[1], i0[2], i0[3], i1[0], i1[1], i1[2], i1[3] };
				double f = BitConverter.ToDouble(bytes, 0);
				return f - 1.0;
			}
			return r * (1.0 / (0xFFFFFFFF + 1.0));
		}
		public uint BRandom()
		{
			// Rastgele 32 bit üret
			uint y;

			if (mti >= MERS_N)
			{
				const uint LOWER_MASK = 2147483647;
				const uint UPPER_MASK = 0x80000000;
				uint[] mag01 = { 0, MERS_A };

				int kk;
				for (kk = 0; kk < MERS_N - MERS_M; kk++)
				{
					y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
					mt[kk] = mt[kk + MERS_M] ^ (y >> 1) ^ mag01[y & 1];
				}

				for (; kk < MERS_N - 1; kk++)
				{
					y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
					mt[kk] = mt[kk + (MERS_M - MERS_N)] ^ (y >> 1) ^ mag01[y & 1];
				}

				y = (mt[MERS_N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
				mt[MERS_N - 1] = mt[MERS_M - 1] ^ (y >> 1) ^ mag01[y & 1];
				mti = 0;
			}

			y = mt[mti++];

			// Temperleme (ihmal edilebilir):
			y ^= y >> MERS_U;
			y ^= (y << MERS_S) & MERS_B;
			y ^= (y << MERS_T) & MERS_C;
			y ^= y >> MERS_L;
			return y;
		}
	}
}
