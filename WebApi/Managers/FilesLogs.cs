using System;
using System.IO;

namespace WebApi.Managers;
public class FilesLogs
{
    public string FilePath { get; set; }

    public FilesLogs(string filePath)
    {
        FilePath = filePath;
    }
            
    public void WriteLog(string line)
    {
        try
        {
            File.AppendAllText(FilePath, $"{DateTime.Now} -{line}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.Message}");
        }
    }

    [Obsolete("Use newMethod Instead")]
    public void ReadLog()
    {
        try
        {
            string file = "";
            if (File.Exists(FilePath))
                file = File.ReadAllText(FilePath, System.Text.Encoding.UTF8);
            Console.WriteLine($"{file}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error File Not Found: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
        
    // static void Main(string[] args)
    // {
    //     FilesLogs f = new FilesLogs("Example.txt");
    //     f.WriteLog(" Files Log Is Working :)\n");
    //     f.ReadLog();
    //     Console.ReadLine();
    // }    
    
}