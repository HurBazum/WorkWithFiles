using System.Text.RegularExpressions;
namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            GetSizeBeforeAndAfterClear();
            Console.ReadLine();
        }
        static void GetSizeBeforeAndAfterClear()
        {
            try
            {
                string s = string.Empty;
                Console.WriteLine("Введите URL папки(вместо пробелов вводится - %20): ");
                s = Console.ReadLine();
                ///<summary> 
                ///Проверка верна ли введена ссылка e.g. "file:///C:/" - ок, "file:///C:" - не ок,
                ///при неверном вводе выбрасывается исключение
                ///</summary>
                if (!Regex.IsMatch(s, @"file:\/\/\/\w{1}\:[\/\w\/]+"))
                {
                    throw new Exception("Неверно указана ссылка!\nНеобходимо начинать с file:// и название диска заключать в - /.../\n" +
                        "пример правильного ввода - file:///C:/");
                }
                if (Directory.Exists(s.Replace("%20", " ").Substring(8)))
                {
                    ///<summary>
                    /// получаем значения исходного размера и утраченного,
                    /// присваиваем их кортежу для корректного вывода, через
                    /// айтемы 
                    ///</summary>
                    (long, long) lostSizeStartSize = DeleteOldFoldersAndFiles(s);
                    Console.WriteLine(lostSizeStartSize.Item2 + " - исходный размер папки.\n" + lostSizeStartSize.Item1 + " - освобождено");
                    Console.WriteLine(GetSize(s) + " - текущий размер папки.");
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
            catch (DirectoryNotFoundException ex)
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
        static (long, long) DeleteOldFoldersAndFiles(string s, int lastAccessAt = 30)
        {
            //получение пути директории по заданной ссылке
            Uri uri = new Uri(s);
            DirectoryInfo di = new DirectoryInfo(uri.LocalPath);
            ///<summary>
            ///кортеж для подсчёта:
            ///Item1 - размера удалённых объектов,
            ///Item2 - оставшихся, с помощью метода GetSize()
            ///</summary>
            (long, long) lostSizeAndLastSize = (0, 0);//удалено, осталось
            //удаляются объекты в папке
            try
            {
                if (di.Exists)
                {
                    //сканирует файлы в папке, удаляет те, к которым обращались более 30(может задаваться) минут назад
                    foreach (FileInfo file in di.GetFiles())
                    {
                        if (DateTime.Now - file.LastAccessTime > TimeSpan.FromMinutes(lastAccessAt))
                        {
                            lostSizeAndLastSize.Item1 += file.Length;
                            file.Delete();
                        }
                    }
                    
                    //сканирует папки, удаляет те, к которым обращались более 30 минут назад, со всем содержимым
                    //если это не так переходит к предыдущему блоку и сканирует все файлы в текущей директории
                    foreach (var directory in di.GetDirectories())
                    {
                        if (DateTime.Now - Directory.GetLastAccessTime(directory.FullName) > TimeSpan.FromMinutes(lastAccessAt))
                        {
                            lostSizeAndLastSize.Item1 += DeleteOldFoldersAndFiles(directory.FullName, lastAccessAt).Item1;
                            
                            directory.Delete(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.HelpLink);
            }
            return lostSizeAndLastSize = (lostSizeAndLastSize.Item1, GetSize(s) + lostSizeAndLastSize.Item1);
        }
    }
}