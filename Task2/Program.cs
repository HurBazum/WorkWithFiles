using System.Net;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace Task2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                /// file:///C:/Games/srlzbl/testFolderNew
                // пример ссылки - file:///C:/Games/Gothic%202%20NB%20Mod%20Installer
                string s = string.Empty;
                Console.WriteLine("Введите URL папки(вместо пробелов вводится - %20): ");
                s = Console.ReadLine();

                ///<summary> 
                ///Проверка верна ли введена ссылка e.g. "file:///C:/" - ок, "file:///C:" - не ок,
                ///при неверном вводе выбрасывается исключение
                ///</summary>
                
                if(!Regex.IsMatch(s, @"file:\/\/\/\w{1}\:[\/\w\/]+"))
                {
                    throw new Exception("Неверно указана ссылка!\nНеобходимо начинать с file:// и название диска заключать в - /.../\n" +
                        "пример правильного ввода - file:///C:/");
                }

                ///<summary>
                /// проверка наличия директории по заданному пути,
                /// чтоб не выводилось "0 - размер данной директории"
                /// после выброшенной методом GetSize(s) ошибки, 
                /// а просто выводилось сообщение об ошибке
                ///</summary>

                if (Directory.Exists(s.Replace("%20", " ").Substring(8)))
                {
                    Console.WriteLine((GetSize(s) + " - размер данной директории."));
                }
                else
                {
                    throw new DirectoryNotFoundException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.HelpLink);
            }
        }
        static long GetSize(string s)
        {
            Uri uri = new Uri(s);
            DirectoryInfo di = new DirectoryInfo(uri.LocalPath);
            long size = 0;
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
                        size += GetSize(directoryInfo.FullName);
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
            catch(AccessViolationException ex)
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

