using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace FinalTask
{ 
    class Program
    {
        static void Main(string[] args)
        {
            Student[] students;
            BinaryFormatter formatter = new BinaryFormatter();
            //создаём директорию на рабочем столе
            DirectoryInfo di = new DirectoryInfo($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Students");
            if (!di.Exists)
            {
                di.Create();
            }
            //получаем массив студентов из .dat файла
            using (var fs = new FileStream(@$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\DownLoads\Students.dat", FileMode.OpenOrCreate))
            {
                try
                {
                   students = (Student[])formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
                Console.WriteLine("Объект десериализован");
            }
            //создаём файлы групп при помощи списка:
            //в него добавляется уникальное значение Student.Group
            //и мы создаём файл с таким именем в директории
            List<string> groups = new List<string>();
            foreach (var student in students)
            {
                if (!groups.Contains(student.Group))
                {
                    groups.Add(student.Group);
                    using (var fs = new FileStream(@$"{di.FullName}\{student.Group}.txt", FileMode.Append))
                    {
                        if (!File.Exists(@$"{student.Group}.txt"))
                        {
                            File.Create(@$"{student.Group}.txt");
                        }
                        fs.Close();
                    }
                }
                Console.WriteLine($"Имя: {student.Name}\nГруппа: {student.Group}\nДата рождения:{student.DateOfBirth.ToShortDateString()}");
                Console.WriteLine("-------------------------");
            }
            //заполняем файлы значениями students1 по группам
            for (int i = 0; i < students.Length; i++)
            {
                using (StreamWriter sw = File.AppendText(@$"{di.FullName}\{students[i].Group}.txt"))
                {
                    sw.WriteLine(students[i].ToString());
                    sw.Close();
                }
            }
        }
    }
}