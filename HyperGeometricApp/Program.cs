using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HyperGeometricApp
{
    public class Program
    {
        public static BigInteger[] factLUT { get; set; }
        static void Main(string[] args)
        {
            int handSize = 5;
            int deckSize = 40;
            Deck deck = new Deck();
            deck.AddCategory(new Category("A", 3, 1));
            deck.AddCategory(new Category("B", 2, 1));

            if (deck.Count > deckSize)
            {
                Console.WriteLine("too many cards in categories");
            }

            factLUT = new BigInteger[deckSize + 1];
            factLUT[0] = 1;
            for (int i = 1; i < factLUT.Length; i++)
            {
                factLUT[i] = i * factLUT[i - 1];
            }

            Deck success = new Deck();

            foreach (Category c in deck.Categories)
            {
                if (c.desired > 0)
                {
                    success.AddCategory(c);
                }
            }
            if (success.Desired > handSize)
            {
                Console.WriteLine("Impossible result: total successes greater than hand size");
            }

            /*
            double r = (double)(BiDist(3, 1) * BiDist(37, 4)) / (double) BiDist(40, 5);
            r += (double)(BiDist(3, 2) * BiDist(37, 3)) / (double) BiDist(40, 5);
            r += (double)(BiDist(3, 3) * BiDist(37, 2)) / (double) BiDist(40, 5);
            Console.WriteLine(r);
            Console.WriteLine(CumulHyGeo(40, 5, 3, 1));
            Console.WriteLine(MultHyGeo(40, 5, new int[]{ 3 }, new int[] { 1 }));
            Console.WriteLine(CumulMultHyGeo(40, 5, new int[]{ 3 }, new int[] { 1 }));
            */

            int[] K = new int[success.Categories.Count()];
            int[] k = new int[K.Length];
            for (int i = 0; i < K.Length; i++)
            {
                K[i] = success.Categories[i].count;
                k[i] = success.Categories[i].desired;
            }
            Console.WriteLine(CumulMultHyGeo(deckSize, (int) handSize, K, k));
        }

        /**
         * Hypergeometric Distribution
         * N = population, n = sample size, K = successes in population, k = successes in sample
         */
        public static double HyGeo(int N, int n, int K, int k)
        {
            return (double) (BiDist(K, k) * BiDist(N - K, n - k)) / (double) BiDist(N, n);
        }

        
        // Cumulative Hypergeometric Distribution
        public static double CumulHyGeo(int N, int n, int K, int k)
        {
            double r = 0;
            for (int i = k; i <= K; i++)
            {
                r += HyGeo(N, n, K, i);
            }
            return r;
        }

        // Multivariate Hypergeometric Distribution
        public static double MultHyGeo(int N, int n, int[] K, int[] k)
        {
            double r = (k.Sum() < N ? (double) BiDist(N - K.Sum(), n - k.Sum()) : 1) / (double) BiDist(N, n);
            for (int i = 0; i < K.Length; i++)
            {
                r *= (double) BiDist(K[i], k[i]);
            }
            return r;
        }

        // Cumulative Multivariate Hypergeometric Distribution
        public static double CumulMultHyGeo(int N, int n, int[] K, int[] k)
        {
            // K[] can stay constant, 'k' will have to become int[i,j] where i is set that we are adding to the result
            // Need to create k[i,j] such that sum of k[i,j] is <= n
            // Then need to sum and return results of Multivariate Hypergeometric Distributions
            List<List<int>> newK = new List<List<int>>();
            newK.Add(new List<int>());
            for (int i = 0; i < K.Length; i++)
            {
                newK = HelpMe(newK, n, K[i], k[i]);
            }
            double r = 0;
            for (int i = 0; i < newK.Count; i++)
            {
                r += MultHyGeo(N, n, K, newK[i].ToArray());
            }
            return r;
        }

        public static List<List<int>> HelpMe(List<List<int>> c, int n, int K, int k)
        {
            List<List<int>> t = new List<List<int>>();
            for (int i = 0; i < c.Count; i++)
            {
                for (int j = 0; j + k <= K; j++)
                {
                    if (j + k + c[i].Sum() <= n)
                    {
                        List<int> tt = new List<int>(c[i]);
                        tt.Add(k + j);
                        t.Add(tt);
                    }
                }
            }
            return t;
        }

        public static BigInteger BiDist(int n, int k)
        {
            return factLUT[n] / (factLUT[k] * factLUT[n - k]);
        }
    }

    class Deck
    {
        public int Count { get { return GetCardCount(); } }
        public int Desired { get { return GetDesired(); } }
        public List<Category> Categories { get; set; }
        public Deck()
        {
            Categories = new List<Category>();
        }
        public void AddCategory(Category cat)
        {
            Categories.Add(cat);
        }
        public int GetCardCount()
        {
            int count = 0;
            foreach (Category c in Categories)
            {
                count += c.count;
            }
            return count;
        }
        public int GetDesired()
        {
            int desired = 0;
            foreach (Category c in Categories)
            {
                desired += c.desired;
            }
            return desired;
        }
        public override string ToString()
        {
            string str = "";
            foreach (Category c in Categories)
            {
                str += $"{c}\n";
            }
            str += $"Card Count: {Count}";
            return str;
        }
    }

    class Category
    {
        public string CategoryName { get; set; }
        public int desired { get; set; }
        public int count { get; set; }
        public Category(string cat, int c, int d)
        {
            CategoryName = cat;
            count = c;
            desired = d;
        }
        public override string ToString()
        {
            return $"{CategoryName}, {count}, {desired}";
        }
    }

}
