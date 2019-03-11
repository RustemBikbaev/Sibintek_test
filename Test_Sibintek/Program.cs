using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace Test_Sibintek
{
    public class Program
    {
        //object block = new object();
        static void Main(string[] args)
        {
            string path = @"I:\Sibintek_test-master";

            if (Directory.Exists(path))
            {
                //// Создаем 4 потоков
                //Thread[] threads = new Thread[4];
                //threads[0] = new Thread(new ThreadStart());

                //for (int i = 0; i < 4; i++)
                //{
                //    threads[i] = new Thread(new ThreadStart(/*mt.ThreadNumbers*/));
                //    threads[i].Name = string.Format("Работает поток: #{0}", i);
                //}    
                //// Запускаем все потоки
                //foreach (Thread t in threads)
                //    t.Start();

                List<string> ErrorList = new List<string>();
                List<string> filesList = GetFilesList(path);
                List<string> HashList = new List<string>();

                for (int i = 0; i < filesList.Count; i++)
                {
                    try
                    {
                        byte[] buff = File.ReadAllBytes(filesList[i]); //побайтовое чтение файла

                        //вызов 2-3 потока
                        HashList.Add(MD5Hash(buff));//хеширование
                        Console.WriteLine(MD5Hash(buff)); //хеширование

                        Console.WriteLine(filesList[i]); //путь  

                        ErrorList.Add("No Exception"); //ошибка
                        Console.WriteLine(ErrorList[i]); //ошибка
                    }
                    catch (Exception e)
                    {
                        ErrorList.Add(e.Message);
                        Console.WriteLine(ErrorList[i]);
                        continue;
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

        public static string MD5Hash(byte[] bytes2) //функция хеширования файла
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(bytes2);

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
