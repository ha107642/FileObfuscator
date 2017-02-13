using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FileObfuscator
{
    class Program
    {
        private const int EXIT_ERROR = -1;
        private const int EXIT_SUCCESS = 0;
        private const int BUFFER_SIZE = 1024;

        struct CommandLineArguments
        {
            public string FileName;
            public bool Obfuscate;
            public byte Key;
        };

        static CommandLineArguments ParseCommandLine(string[] args)
        {
            const byte DEFAULT_KEY = 107;

            CommandLineArguments cmd = new CommandLineArguments
            {
                FileName = null,
                Obfuscate = false,
                Key = DEFAULT_KEY
            };

            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "-o":
                    case "--obfuscate":
                        cmd.Obfuscate = true;
                        break;
                    case "-k":
                    case "--key":
                        ++i; //Advance i by one. We want the next argument.
                        if (i >= args.Length)
                            Fail("Command line argument -k or --key was specified, but no key was actually specified");
                        byte k;
                        if (byte.TryParse(args[i], out k))
                            cmd.Key = k;
                        break;
                    default:
                        cmd.FileName = args[i];
                        break;
                }
            }
            return cmd;
        }

        static int Main(string[] args)
        {
            CommandLineArguments cmd = ParseCommandLine(args);
            if (cmd.FileName == null)
                Fail("No file specified. Exiting");
            if (!File.Exists(cmd.FileName))
                Fail("Unable to find file specified. Exiting");

            string fileName = cmd.FileName;
            string newFileName;
            if (cmd.Obfuscate)
            {
                newFileName = fileName.EndsWith(".obf") ? fileName.Substring(0, fileName.Length - 4) : fileName + ".obf";

                if (File.Exists(newFileName))
                {
                    DialogResult result = MessageBox.Show(String.Format("The file \"{0}\" already exists. Would you like to overwrite it?", Path.GetFileName(newFileName)), "File already exists", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                        return EXIT_ERROR;
                }
            }
            else
            {
                newFileName = fileName.EndsWith(".obf") ? fileName.Substring(0, fileName.Length - 4) : fileName;

                string tempPath = Path.Combine(Path.GetTempPath(), "FileObfuscator");
                Directory.CreateDirectory(tempPath);
                CleanUpTemp(tempPath);

                newFileName = Path.GetFileNameWithoutExtension(newFileName) + DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(newFileName);
                newFileName = Path.Combine(tempPath, newFileName);
            }

            using (FileStream writer = File.Open(newFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            using (FileStream reader = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Write))
                Obfuscate(writer, reader, cmd.Key);

            if (!cmd.Obfuscate)
                OpenFile(newFileName);

            return EXIT_SUCCESS;
        }

        private static void OpenFile(string newFileName)
        {
            ProcessStartInfo si = new ProcessStartInfo(newFileName);
            si.ErrorDialog = true;
            try
            {
                Process.Start(si);
            }
            catch (Win32Exception ex)
            {
                if (ex.Message != "The operation was canceled by the user")
                    throw;
            }
        }

        private static void Fail(string reason)
        {
            Console.Error.WriteLine(reason);
            Environment.Exit(EXIT_ERROR);
        }

        //Delete any files in the temp path that was accessed no later than a week ago.
        private static void CleanUpTemp(string tempPath)
        {
            DateTime aWeekAgo = DateTime.Now.AddDays(-7);
            string[] files = Directory.GetFiles(tempPath);
            foreach (string fileName in files)
            {
                FileInfo file = new FileInfo(fileName);
                if (file.LastAccessTime < aWeekAgo)
                    File.Delete(fileName);
            }
        }

        private static void Obfuscate(FileStream writer, FileStream reader, byte key)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int read = 0;
            while ((read = reader.Read(buffer, 0, BUFFER_SIZE)) > 0)
            {
                for (int i = 0; i < read; ++i)
                    buffer[i] ^= key;
                writer.Write(buffer, 0, read);
            }
        }
    }
}