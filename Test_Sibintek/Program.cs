using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Test_Sibintek
{
    class Program
    {

        static void Main(string[] args)
        {
            

            string path = @"C:\Users\Рустем\Desktop\Test_Sibintek\file_for_test_sibintek.txt";
            string path2 = @"C:\Users\Рустем\Desktop\Test_Sibintek\file_for_test_sibintek2.png";
            string path3 = @"C:\Users\Рустем\Desktop\Test_Sibintek";
            string path4 = @"C:\Users\Рустем\Desktop\Test_Sibintek";
            

            using (StreamReader sr = new StreamReader(path))
                {
                    
                    Console.WriteLine($"\n Hello, {MD5Hash(sr.ReadToEnd())}");
                }

            using (StreamReader sr = new StreamReader(path2))
            {
                
                Console.WriteLine($"\n Hello, {MD5Hash(sr.ReadToEnd())}");
            }
            

            if (Directory.Exists(path3))
            {
                var directory = Directory.EnumerateFileSystemEntries(path3);
                foreach (string elem in directory)
                {
                    Console.WriteLine(elem);
                }
            }
            else
            {
                Console.WriteLine("Такого пути нет");
            }
            

            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++");

            DirectoryInfo dir = new DirectoryInfo(path4);

            Directories(dir);

            
            
        }


        static void Directories(DirectoryInfo dir)
        {
            foreach (var x in dir.GetDirectories().OrderBy(x => x.Name))
            {
                Console.WriteLine(x);
                if (x.GetDirectories().Count() > 0)
                {
                    
                    Directories(x);
                    
                }
            }
        }
  


        public static string MD5Hash(string input) //функция хеширования файла
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

      
    }
}
