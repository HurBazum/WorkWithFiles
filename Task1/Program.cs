using System.Security.AccessControl;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string path = @"C:\Games\srlzbl\testFolderNew";
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists)
                { 
                    DeleteOldFoldersAndFiles(directoryInfo, 1);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="lastAccesAt">решил сделать его с возможностью измения</param>
        static void DeleteOldFoldersAndFiles(DirectoryInfo directoryInfo, int lastAccesAt = 30)
        {
            //удаляются объекты в папке
            var directoryFiles = directoryInfo.GetFiles();
            var directoryFolders = directoryInfo.GetDirectories();
            try
            {
                //сканирует файлы в папке, удаляет те, к которым обращались более 30 минут назад
                foreach (var file in directoryFiles)
                {
                    if (DateTime.Now - file.LastAccessTime > TimeSpan.FromMinutes(lastAccesAt))
                    {
                        file.Delete();
                    }
                }
                //сканирует папки, удаляет те, к которым обращались более 30 минут назад, со всем содержимым
                //если это не так переходит к предыдущему блоку и сканирует все файлы в текущей директории
                foreach (var directory in directoryFolders)
                {
                    DeleteOldFoldersAndFiles(directory);
                    if (DateTime.Now - Directory.GetLastAccessTime(directory.FullName) > TimeSpan.FromMinutes(lastAccesAt))
                    {
                        directory.Delete(true);
                    }
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