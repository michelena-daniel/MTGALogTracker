using System.Text.Json;
using System.Text.RegularExpressions;

namespace LogWorker.Helpers
{
    public class JsonHelper
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters = { new CustomDateConverter() }
        };

        public static List<T> DeserializeJsonObjects<T>(string logs, string jsonPattern)
        {
            var regex = new Regex(jsonPattern, RegexOptions.Singleline);
            var matches = regex.Matches(logs);
            var resultList = new List<T>();

            foreach (Match match in matches)
            {
                try
                {
                    var deserializedObject = JsonSerializer.Deserialize<T>(match.Value, _options);
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
