using Newtonsoft.Json;

namespace Insya.Localization;
//public class Localization
//{
//    private readonly static IWebHostEnvironment _hostEnvironment;
//    public static string Read(string key)
//    {
//        string rootPath = "";
//        var fileName = "file.json";
//        var filePath = Path.Combine(rootPath, "Localization", fileName);
//        var jsonContent = File.ReadAllText(filePath);
//        var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
//        if (jsonObject.TryGetValue(key, out var value))
//        {
//            return value;
//        }

//        return key;
//    }


//    public static string Get(string key)
//    {

//        return Read(key);
//    }

//    //public static string Get(string key)
//    //{
//    //    return key;
//    //}

//    public void Add(string key, string value)
//    {
//        var webRootPath = "_hostEnvironment.WebRootPath";
//        var fileName = "file.json";
//        var filePath = Path.Combine(webRootPath, "Localization", fileName);


//        var jsonContent = File.ReadAllText(filePath);
//        var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
//        jsonObject[key] = value;
//        var updatedJsonContent = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
//        File.WriteAllText(filePath, updatedJsonContent);
//    }



//    public void allfunctions()
//    {
//        // Read JSON from a file
//        var jsonFilePath = "path/to/your/json/file.json";
//        var jsonContent = File.ReadAllText(jsonFilePath);
//        var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

//        // Read a value by key
//        if (jsonObject.TryGetValue("yourKey", out var value))
//        {
//            Console.WriteLine($"Value for 'yourKey': {value}");
//        }

//        // Change a value by key
//        jsonObject["yourKey"] = "newUpdatedValue";

//        // Remove a key-value pair
//        jsonObject.Remove("keyToRemove");

//        // Insert a new key-value pair
//        jsonObject["newKey"] = "newValue";

//        // Serialize the updated object back to JSON
//        var updatedJsonContent = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
//        File.WriteAllText(jsonFilePath, updatedJsonContent);
//    }

//    public class LocalizationData
//    {
//        private readonly IWebHostEnvironment _hostEnvironment;

//        public LocalizationData(IWebHostEnvironment hostingEnvironment)
//        {
//            _hostEnvironment = hostingEnvironment;
//        }

//        public string GetData()
//        {
//            return _hostEnvironment.ContentRootPath;
//        }
//    }


//}

public class Localization
{

    public string GetData(string key)
    {
        string baseDirectory = AppContext.BaseDirectory;
        var wwwRootPath = Path.Combine(baseDirectory, "wwwroot");
        var fileName = "file.json";
        var filePath = Path.Combine(wwwRootPath, "Localization", fileName);
        var jsonContent = File.ReadAllText(filePath);
        var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
        if (jsonObject.TryGetValue(key, out var value))
        {
            return value;
        }
        return key;
    }

    public static string Get(string key)
    {
        return string .Format("<span class='_local' data='{0}'>{1}</span>",key,key);
    }
}


