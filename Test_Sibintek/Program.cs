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
            

            //string path1 = @"C:\Users\Рустем\Desktop\Test_Sibintek\file_for_test_sibintek.txt";
            //string path2 = @"C:\Users\Рустем\Desktop\Test_Sibintek\file_for_test_sibintek2.png";
            //string path3 = @"C:\Users\Рустем\Desktop\Test_Sibintek";
            string path = @"C:\Users\Рустем\Desktop\ASUS";
            

            //using (StreamReader sr = new StreamReader(path1))
            //    {
                    
            //        Console.WriteLine($"\n Hello, {MD5Hash(sr.ReadToEnd())}");
            //    }

            //using (StreamReader sr = new StreamReader(path2))
            //{
                
            //    Console.WriteLine($"\n Hello, {MD5Hash(sr.ReadToEnd())}");
            //}
            

            if (Directory.Exists(path))
            {
               

                List<string> filesList = GetFilesList(path);
                for (int i = 0; i < filesList.Count; i++)
                {
                    using (StreamReader sr = new StreamReader(filesList[i]))
                    {
                        try
                        {
                            Console.WriteLine(filesList[i]);
                            Console.WriteLine( MD5Hash(sr.ReadToEnd()) + "\n");
                        }
                        catch (IOException)
                        {
                            sr.Close();
                            continue;
                        }
                    }

                }
            }
            else
            {
                Console.WriteLine("Такого пути нет");
            }
            


        }


        private static List<string> GetFilesList(string path) //список путей ко всем файлам
        {
          
            List<string> filesList = new List<string>();
            string[] dirs = Directory.GetDirectories(path);
            filesList.AddRange(Directory.GetFiles(path));
            foreach (string subdirectory in dirs)
            {
                try
                {
                    filesList.AddRange(GetFilesList(subdirectory));
                }
                catch { }
            }
            
            return filesList;
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
