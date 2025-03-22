using Domain.Interfaces;
using Domain.Models;
using LogWorker.Helpers;
using System.Globalization;
using System.Text.Json;

namespace LogWorker.Services.CoreServices
{
    public class EventService : IEventService
    {
        public EventService()
        {

        }

        public string FetchEventJoin(string line, StreamReader sr, string delimeter, LogAuthenticationState state)
        {
            if(line.Contains("[UnityCrossThreadLogger]==> Event_Join"))
            {
                var json = line.Replace("[UnityCrossThreadLogger]==> Event_Join", "");
                var timeStampString = sr.ReadLine().Replace("[UnityCrossThreadLogger]", "");
                var timeStamp = DateTime.ParseExact(timeStampString, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);

                var rawEvent = JsonSerializer.Deserialize<EventJoinDto>(json);
                var eventRequest = JsonSerializer.Deserialize<EventRequest>(rawEvent.EventRequest);
                eventRequest.TimeStamp = timeStamp;
                eventRequest.MtgArenaId = state.MtgArenaId;
                eventRequest.UserName = state.UserName;

                var options = new JsonSerializerOptions();
                options.Converters.Add(new CustomDateConverter());

                var result = JsonSerializer.Serialize(eventRequest, options);

                return result + delimeter;
            }

            return string.Empty;
        }
    }
}
