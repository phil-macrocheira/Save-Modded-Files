using System.Collections;

class Program
{
    static string configFile = @"config.txt";

    static void Main()
    {
        if (!File.Exists(configFile)) {
            Console.WriteLine("Config.txt not found! Why did you delete/rename it?!");
            return;
        }
        var config = ReadConfig();

        if (!config.ContainsKey("unmodded_path") || !config.ContainsKey("modded_path")) {
            Console.WriteLine("Error: 'unmodded_path' and/or 'modded_path' missing in the config.txt. Why did you delete/rename them?!");
            return;
        }

        string unmoddedPath = config["unmodded_path"];
        string moddedPath = config["modded_path"];
        bool invalid_path = false;

        if (!Directory.Exists(unmoddedPath))
        {
            Console.WriteLine($"unmodded_path path in config.txt does not exist: {unmoddedPath}");
            invalid_path = true;
        }

        if (!Directory.Exists(moddedPath))
        {
            Console.WriteLine($"modded_path path in config.txt does not exist: {moddedPath}");
            invalid_path = true;
        }

        if (invalid_path) {
            return;
        }

        Console.WriteLine($"Comparing folders:\n{unmoddedPath}\n{moddedPath}\n");

        string moddedFolder = CreateModdedFolder("Modded Files");

        foreach (var file in Directory.GetFiles(unmoddedPath, "*", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileName(file);
            string moddedFile = Path.Combine(moddedPath, fileName);

            if (!File.Exists(moddedFile))
            {
                Console.WriteLine($"File not found in modded path: {fileName}, cannot proceed");
                Console.ReadKey();
                return;
            }

            if (!CompareFiles(file, moddedFile))
            {
                string destination = Path.Combine(moddedFolder, fileName);
                File.Copy(moddedFile, destination, true);
            }
        }

        foreach (var file in Directory.GetFiles(moddedPath, "*", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileName(file);
            string unmoddedFile = Path.Combine(unmoddedPath, fileName);

            if (!File.Exists(unmoddedFile))
            {
                string destination = Path.Combine(moddedFolder, fileName);
                File.Copy(file, destination, true);
            }
        }

        Console.WriteLine("Done!");
    }

    static Dictionary<string, string> ReadConfig()
    {
        var config = new Dictionary<string, string>();
        foreach (var line in File.ReadLines(configFile))
        {
            var parts = line.Split(new[] { " = " }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                config[parts[0]] = parts[1].Trim('"');
            }
        }
        return config;
    }

    static string CreateModdedFolder(string baseFolder)
    {
        string folderName = baseFolder;
        int counter = 2;
        while (Directory.Exists(folderName))
        {
            folderName = $"{baseFolder} ({counter})";
            counter++;
        }
        Directory.CreateDirectory(folderName);
        return folderName;
    }

    static bool CompareFiles(string file1, string file2)
    {
        byte[] file1Bytes = File.ReadAllBytes(file1);
        byte[] file2Bytes = File.ReadAllBytes(file2);
        return StructuralComparisons.StructuralEqualityComparer.Equals(file1Bytes, file2Bytes);
    }
}