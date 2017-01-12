using System;
using System.IO;
using System.Windows.Forms;

namespace FileObfuscator
{
    class Program
    {
        private const int EXIT_ERROR = -1;
        private const int EXIT_SUCCESS = 0;
        private const int BUFFER_SIZE = 1024;
        private static byte DEFAULT_KEY = 107;

        static int Main(string[] args)
        {
            if (args.Length == 0)
                Fail("No file specified.");

            String fileName = args[0];
            if (!File.Exists(fileName))
                Fail("Unable to find file specified.");

            String newFileName = fileName.EndsWith(".obf") ? fileName.Substring(0, fileName.Length - 4) : fileName + ".obf";
            if (File.Exists(newFileName))
            {
                DialogResult result = MessageBox.Show(String.Format("The file \"{0}\" already exists. Would you like to overwrite it?", Path.GetFileName(newFileName)), "File already exists", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    Fail("File already exists.");
            }
            File.Copy(fileName, newFileName, true);

            using (FileStream writer = File.Open(newFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            using (FileStream reader = File.Open(newFileName,FileMode.Open, FileAccess.Read, FileShare.Write))
                Obfuscate(writer, reader, DEFAULT_KEY);

            return EXIT_SUCCESS;
        }

        private static void Fail(string reason)
        {
            Console.Error.WriteLine("ERROR: " + reason);
            Environment.Exit(EXIT_ERROR);
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
