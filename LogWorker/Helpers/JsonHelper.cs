using System.Text.Json;

namespace LogWorker.Helpers
{
    public class JsonHelper
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters = { new CustomDateConverter() }
        };

        public static List<T> DeserializeJsonObjects<T>(string logs, string delimeter)
        {
            var jsonList = logs.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
            var resultList = new List<T>();

            foreach (var json in jsonList)
            {
                try
                {
                    var deserializedObject = JsonSerializer.Deserialize<T>(json, _options);
                    if (deserializedObject != null)
                    {
                        resultList.Add(deserializedObject);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON for {typeof(T).Name}: {ex.Message}");
                }
            }

            return resultList;
        }
    }
}
