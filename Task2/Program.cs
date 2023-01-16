using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace WorkWithFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // file:///C:/GOG%20Games/Gothic%202%20Gold/
                // %20 - указывается вместо пробела в ссылке
                //"file:///C:/Games/srlzbl/Pharaoh/Audio"
                // file:///C:/GOG%20Games/Gothic%202%20Gold/saves/current
                string s = string.Empty;
                long directorySize = 0;
                Console.WriteLine("Введите урл папки(вместо пробелов вводится - %20): ");
                s = Console.ReadLine();
                ///<summary> 
                ///Проверка верна ли введена ссылка e.g. "file:///C:/" - ок, "file:///C:" - не ок,
                ///при неверном вводе выбрасывается исключение
                ///</summary>
                if(!Regex.IsMatch(s, @"file:\/\/\/\w{1}\:[\/\w\/]+"))
                {
                    throw new Exception("Неверно указана ссылка!\nНеобходимо начинать с file:// и название диска заключать в - /.../");
                }
                string newS = new string(s.Substring(8));
                if(Directory.Exists(newS)) 
                    Console.WriteLine((GetSize(s, ref directorySize) + " - размер данной директории."));
                else
                    throw new DirectoryNotFoundException();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.HelpLink);
            }
        }
        static long GetSize(string s, ref long size)
        {
            Uri uri = new Uri(s);
            DirectoryInfo di = new DirectoryInfo(uri.LocalPath);
            try
            {
                if (di.Exists)
                {
                    //скаладывает длины файлов в основной папке
                    foreach (FileInfo file in di.GetFiles())
                    {
                        size += file.Length;
                    }
                    //складывает длины файлов во вложенных папках, если они есть
                    DirectoryInfo[] directoryInfos = di.GetDirectories();
                    foreach (DirectoryInfo directoryInfo in directoryInfos)
                    {
                        GetSize(directoryInfo.FullName, ref size);
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException();
                }
            }
            catch(DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.HelpLink);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.HelpLink);
            }
            return size;
        }
    }
}

