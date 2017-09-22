﻿using System;
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
            BigInteger handSize = 3;
            factLUT = new BigInteger[61];
            factLUT[0] = 1;
            for (int i = 1; i < factLUT.Length; i++)
            {
                factLUT[i] = i * factLUT[i - 1];
            }
            Deck deck = new Deck();
            deck.AddCategory(new Category("Brilliant Fusion", 3, 1));
            deck.AddCategory(new Category("Other Cards", 37, 0));

            Deck success = new Deck();
            Deck other = new Deck();

            foreach (Category c in deck.Categories)
            {
                if (c.desired > 0)
                {
                    success.AddCategory(c);
                }
                else
                {
                    other.AddCategory(c);
                }
            }
            if (success.GetCardCount() > handSize)
            {
                Console.WriteLine("Impossible result: total successes greater than hand size");
            }
            double r = (double)(BiDist(3, 1) * BiDist(37, 4)) / (double) BiDist(40, 5);
            r += (double)(BiDist(3, 2) * BiDist(37, 3)) / (double) BiDist(40, 5);
            r += (double)(BiDist(3, 3) * BiDist(37, 2)) / (double) BiDist(40, 5);
            Console.WriteLine(r);
            Console.WriteLine(CumulHyGeo(40, 5, 3, 1));
            Console.WriteLine(MultHyGeo(40, 5, new int[]{ 3 }, new int[] { 1 }));
            Console.WriteLine(CumulMultHyGeo(40, 5, new int[]{ 3 }, new int[] { 1 }));
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
            double r = (double) BiDist(N - K.Sum(), n - k.Sum()) / (double) BiDist(N, n);
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
            Console.WriteLine("KILL ME NOW");
            int[][] newK = new int[1][];
            newK[0] = new int[0];
            for (int i = 0; i < K.Length; i++)
            {
                newK = HelpMe(newK, n, K[i], k[i]);
            }
            double r = 0;
            for (int i = 0; i < newK.Length; i++)
            {
                r += MultHyGeo(N, n, K, newK[i]);
            }
            foreach (int[] a in newK)
            {
                foreach (int x in a)
                {
                    Console.WriteLine(x);
                }
            }
            return r;
        }

        public static int[][] HelpMe(int[][] c, int n, int K, int k)
        {
            int[,][] t = new int[c.Length, (K - k + 1)][];
            for (int i = 0; i < c.Length; i++)
            {
                for (int j = 0; j + k <= K; j++)
                {
                    t[i, j] = new int[c[i].Length + 1];
                    Array.Copy(c[i], t[i, j], c[i].Length);
                    t[i, j][t[i, j].Length - 1] = k + j;
                }
            }
            return t.Cast<int[]>().ToArray();
        }

        public static BigInteger BiDist(int n, int k)
        {
            return factLUT[n] / (factLUT[k] * factLUT[n - k]);
        }
    }

    class Deck
    {
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
        public override string ToString()
        {
            string str = "";
            foreach (Category c in Categories)
            {
                str += $"{c}\n";
            }
            str += $"Card Count: {GetCardCount()}";
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
