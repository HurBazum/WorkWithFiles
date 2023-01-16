using System.Security.AccessControl;

namespace WorkWithFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string path = @"C:\\testFolders";
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists)
                { 
                    DeleteOldFoldersAndFiles(directoryInfo);
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
        static void DeleteOldFoldersAndFiles(DirectoryInfo directoryInfo)
        {
            DateTime lastAccessAt;
            TimeSpan deleteTime = TimeSpan.FromMinutes(30);
            //удаляются объекты в папке
            var directoryFiles = directoryInfo.GetFiles();
            var directoryFolders = directoryInfo.GetDirectories();
            try
            {
                //сканирует файлы в папке, удаляет те, к которым обращались более 30 минут назад
                foreach (var file in directoryFiles)
                {
                    lastAccessAt = file.LastAccessTime;
                    if (DateTime.Now - lastAccessAt > deleteTime)
                    {
                        Console.WriteLine($"файл {file} время последнего доступа {lastAccessAt}");
                        Console.WriteLine($"прошло больше {deleteTime.Minutes} минут c последнего доступа, - {file} надо удалять");
                        file.Delete();
                    }
                    else
                    {
                        Console.WriteLine($"файл {file} время последнего доступа {lastAccessAt}");
                        Console.WriteLine($"прошло меньше {deleteTime.Minutes} минут c последнего доступа," +
                            $" - {file} не надо удалять");
                    }
                }
                //сканирует папки, удаляет те, к которым обращались более 30 минут назад, со всем содержимым
                //если это не так переходит к предыдущему блоку и сканирует все файлы в текущей директории
                foreach (var directory in directoryFolders)
                {
                    lastAccessAt = directory.LastAccessTime;
                    if (DateTime.Now - lastAccessAt > deleteTime)
                    {
                        Console.WriteLine($"Папка {directory} время последнего доступа {lastAccessAt}");
                        Console.WriteLine($"прошло больше {deleteTime.Minutes} минут c последнего доступа, - {directory} надо удалять");
                        directory.Delete(true);
                        continue;//для перехода к следующей директории после удаления данной, т.к. DeleteOldFoldersAndFiles(directory) расположен ниже
                    }
                    else
                    {
                        Console.WriteLine($"Папка {directory} время последнего доступа {lastAccessAt}");
                        Console.WriteLine($"прошло меньше {deleteTime.Minutes} минут c последнего доступа," +
                            $" - {directory} не надо удалять");
                    }
                    DeleteOldFoldersAndFiles(directory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.HelpLink);
            }
        }
    }
}