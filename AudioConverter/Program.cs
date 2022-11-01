using System.Diagnostics;

namespace AudioConverter
{
    internal class Program
    {
        private static string _baseDirectory = string.Empty;
        private static string _sourceExtension = string.Empty;
        private static string _destinationExtension = string.Empty;
        private static string _destination = string.Empty;
        private static void ConvertFile(FileInfo file)
        {
            int timeout = 10000;
            string source = file.FullName;

            Directory.CreateDirectory($@"{file.Directory}\Converted");

            _destination = $@"C:\Music\Converted\{file.Name.Replace(_sourceExtension, _destinationExtension)}";

            if (File.Exists(_destination))
            {
                DisplayMessage($"Unprocessed: {file.FullName}");
                return;
            }

            DisplayMessage($"Converting file: {file.FullName} from {_sourceExtension} to {_destinationExtension}");

            using var ffmpeg = new Process
            {
                StartInfo =
                {
                    FileName = @"C:\Tools\ffmpeg\bin\ffmpeg.exe",
                    Arguments = $"-i {source} {_destination}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };

            ffmpeg.EnableRaisingEvents = false;
            ffmpeg.OutputDataReceived += (s, e) => Debug.WriteLine($"Debug: {e.Data}");
            ffmpeg.ErrorDataReceived += (s, e) => Debug.WriteLine($"Error: {e.Data}");
            ffmpeg.Start();
            ffmpeg.BeginOutputReadLine();
            ffmpeg.BeginErrorReadLine();
            ffmpeg.WaitForExit(timeout);
        }
        private static void ProcessFolder(DirectoryInfo directory)
        {
            Console.WriteLine();
            DisplayMessage($"Processing Directory: {directory.FullName}");
            Console.WriteLine();

            var fileInfo = directory.EnumerateFiles();
            var directoryInfo = directory.EnumerateDirectories();

            foreach (var file in fileInfo)
            {
                if (file.Extension == _sourceExtension)
                {
                    ConvertFile(file);
                }
            }

            foreach (var dirInfo in directoryInfo)
            {
                ProcessFolder(dirInfo);
            }
        }
        private static void DisplayMessage(string message)
        {
            Console.WriteLine();
            Console.Write(message);
        }
        static void Main(string[] args)
        {
            DisplayMessage("Enter Source Directory: ");
            _baseDirectory = Console.ReadLine()!;

            DisplayMessage("Enter Source Extension: ");
            _sourceExtension = Console.ReadLine()!;

            DisplayMessage("Enter Destination Extension: ");
            _destinationExtension = Console.ReadLine()!;

            ProcessFolder(new DirectoryInfo(_baseDirectory));

            DisplayMessage($@"Conversion complete. The converted files are located in {_baseDirectory}\Converted.");
            DisplayMessage("");
        }
    }
}