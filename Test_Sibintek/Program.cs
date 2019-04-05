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
        public static List<string> NameList = new List<string>();
        public static List<Thread> ThreadList = new List<Thread>();
        static object locker = new object(); //ограничитель доступа к данным (для работы в потоках)
        static void Main(string[] args)
        {
            Console.WriteLine("Enter directory path");
            string path = Console.ReadLine();           

            if (Directory.Exists(path))
            {
                Queue<string> file_queue = new Queue<string>(); //очередь адресов к файлам
                GetFilesQueue(path, file_queue); // заполнение очереди адресами

                int counte = file_queue.Count; //кол. файлов 
                for (int i = 0; i < counte; i++)
                {
                    Thread myThread = new Thread(new ParameterizedThreadStart(MD5Hash)); //объявление нового потока
                    ThreadList.Add(myThread);
                    Data bytes = new Data(); // экземпляр данных для работы в потоке
                    try
                    {
                        bytes.name = file_queue.Peek(); // взятие адреса к файлу из очереди
                        bytes.bytes2 = File.ReadAllBytes(file_queue.Dequeue()); //побайтовое чтение файла 

                        myThread.Start(bytes); //вызов потока
                    }
                    catch (Exception e)
                    {

                        bytes.exception = e.Message; // вывод ошибки если не удалось считать файл
                        myThread.Start(bytes);    //вызов потока                   
                        continue;
                    }
                }               
                Thread myThread2 = new Thread(new ParameterizedThreadStart(SaveinBD)); // поток отправляющей данные в БД
                myThread2.Start(counte);
            }
            else
            {
                Console.WriteLine("Could not find path"); // вывод сообщения что путь не найден
            }
        }


        private static void GetFilesQueue(string path, Queue<string> queue_fiels) //список путей ко всем файлам
        {
            string[] dirs = Directory.GetDirectories(path); //массив каталогов

            foreach (string f in Directory.GetFiles(path))
            {
                try
                {
                    queue_fiels.Enqueue(f); // добавление пути к файлам
                }
                catch { }
            }

            foreach (string subdirectory in dirs)
            {
                try
                {
                    GetFilesQueue(subdirectory, queue_fiels); // поиск файлов в подкаталогах
                }
                catch { }
            }

        }


        public static void MD5Hash(object Bytes2) //функция хеширования файла
        {
            Data by = (Data)Bytes2;
            StringBuilder hash = new StringBuilder();
            if (by.bytes2 != null) 
            {            
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider(); // MD5 хеширование
                byte[] bytes = md5provider.ComputeHash(by.bytes2);

                 for (int i = 0; i < bytes.Length; i++)
                 {
                    hash.Append(bytes[i].ToString("x2"));
                 }
            }
            lock (locker)
            {
                HashList.Add(hash.ToString()); // добавлене хеша файла в список
                NameList.Add(by.name);
                if(by.exception == null)
                {
                    ErrorList.Add(null); //нет ошибок
                }
                else
                {
                    ErrorList.Add(by.exception); //ошибка
                }
            }           
        }

        public static void SaveinBD(object x) //функция отправляющая данные в БД
        {           
            int counte = (int)x; //кол. записей
            string path = Environment.CurrentDirectory.ToString(); //получение пути до БД
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + path + @"\Server_sibintek.mdf;Integrated Security=True"; // установка соединения с БД
            for (int i = 0; i < counte; i++)
            {
                ThreadList[i].Join(); // синхронизация потоков                             
                string sqlExpression = "INSERT INTO Sibintekdata (name, hash, eror) VALUES (N'" + NameList[i] + "', N'" + HashList[i] + "', N'" + ErrorList[i] + "')";   //SQL запрос              
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection); //отправка данных в БД   
                    command.ExecuteNonQuery();
                }
            }
        }       
    }

    public class Data // класс хронящий данные для работы в потоках
    {
        public byte[] bytes2;
        public string hesh;
        public string exception;
        public string name;
    }


}