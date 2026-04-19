using System.Numerics;
using System.Security.Cryptography;

namespace CryptoSandbox.Engine
{
    public class PrimeMathEngine
    {
        // check primes - ok for demos under 10^6
        public static bool IsPrimeSimple(long number)
        {
            if (number < 2)
                return false;
            if (number == 2 || number == 3)
                return true;
            if (number % 2 == 0 || number % 3 == 0)
                return false;
            for (long i = 5; i * i <= number; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0)
                    return false;
            }
            return true;
        }

        // generate random prime (for brute force)
        public static List<long> GetPrimesInRange(long start, long end)
        {
            var primes = new List<long>();
            for (long i = start; i <= end; i++)
            {
                if (IsPrimeSimple(i))
                    primes.Add(i);
            }
            return primes;
        }

        public static long GetRandomPrime(long min, long max)
        {
            // Random Number Generator
            Random rand = new Random();
            // check if is prime
            while (true)
            {
                long candidate = rand.NextInt64(min, max + 1);
                if (IsPrimeSimple(candidate))
                {
                    return candidate;
                }
            }
        }

        // generate random big integer
        // to show why brute force doesn't always work
        //Miller-Rabin generator
        public static BigInteger GetRandomBigInt(int bits)
        {
            return BigInteger.Abs(new BigInteger(RandomNumberGenerator.GetBytes(bits / 8)));
        }
    }
}
