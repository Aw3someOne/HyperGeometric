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
            factLUT = new BigInteger[61];
            factLUT[0] = 1;
            for (int i = 1; i < factLUT.Length; i++)
            {
                factLUT[i] = i * factLUT[i - 1];
            }
            Deck d = new Deck();
            d.AddCategory(new Category("Brilliant Fusion", 1, 3));
            d.AddCategory(new Category("Other Cards", 0, 37));
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
    }

    class Category
    {
        public String CategoryName { get; set; }
        public int desired { get; set; }
        public int count { get; set; }
        public Category(String cat, int d, int c)
        {
            CategoryName = cat;
            desired = d;
            count = c;
        }
    }

}
