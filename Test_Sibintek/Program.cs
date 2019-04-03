using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using System.Data.SqlClient;

namespace Test_Sibintek
{
    public class Program
    {
        public static List<string> ErrorList = new List<string>();
        public static List<string> HashList = new List<string>();
        static object locker = new object();
        static void Main(string[] args)
        {
 

            // string path = @"I:\Sibintek_test-master";
            string path = @"C:\Users\Рустем\Desktop\Test_Sibintek";
            if (Directory.Exists(path))
            {

                Queue<string> numbers = new Queue<string>();
                GetFilesQueue(path, numbers);



                int counte = numbers.Count;
                for (int i = 0; i < counte; i++)
                {
                    Thread myThread = new Thread(new ParameterizedThreadStart(MD5Hash));


                    Bytes bytes = new Bytes();
                    try
                    {

                        bytes.name = numbers.Peek();
                        bytes.bytes2 = File.ReadAllBytes(numbers.Dequeue()); //побайтовое чтение файла 

                        myThread.Start(bytes); //вызов потоков
                    }
                    catch (Exception e)
                    {
                        bytes.exception = e.Message;
                        myThread.Start(bytes);
                        continue;
                    }
                }


            }
            else
            {
                Console.WriteLine("Такого пути нет");
            }
        }


        private static void GetFilesQueue(string path, Queue<string> numbers) //список путей ко всем файлам
        {
            string[] dirs = Directory.GetDirectories(path);

            foreach (string f in Directory.GetFiles(path))
            {
                try
                {
                    numbers.Enqueue(f);
                }
                catch { }
            }

            foreach (string subdirectory in dirs)
            {
                try
                {
                    GetFilesQueue(subdirectory, numbers);
                }
                catch { }
            }

        }


        public static void MD5Hash(object Bytes2) //функция хеширования файла
        {

            Bytes by = (Bytes)Bytes2;
            StringBuilder hash = new StringBuilder();
            if (by.bytes2 != null)
            {            
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                byte[] bytes = md5provider.ComputeHash(by.bytes2);

                 for (int i = 0; i < bytes.Length; i++)
                 {
                    hash.Append(bytes[i].ToString("x2"));
                 }
            }
            lock (locker)
            {
                HashList.Add(hash.ToString());
                if(by.exception == null)
                {
                    ErrorList.Add(""); //ошибка
                }
                else
                {
                    ErrorList.Add(by.exception); //ошибка
                }

                Console.WriteLine(by.name);
                Console.WriteLine(HashList[HashList.Count - 1]); //хеширование                
                Console.WriteLine(ErrorList[ErrorList.Count - 1]); //ошибка

                string name = by.name;
                string connectionString = @"Data Source=DESKTOP-TS65TJB;Initial Catalog=data_sibintek;Integrated Security=True";
                string sqlExpression = "INSERT INTO file_hash_table (name, path, hash, eror) VALUES ('"+name+"', '"+name+"', '"+HashList[HashList.Count - 1]+"', '"+ErrorList[ErrorList.Count - 1]+"')";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    int number = command.ExecuteNonQuery();
                    Console.WriteLine("Добавлено объектов: {0}", number);
                }

            }
            
        }

    }

    public class Bytes
    {
        public byte[] bytes2;
        public string hesh;
        public string exception;
        public string name;
    }


}