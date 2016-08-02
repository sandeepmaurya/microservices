using System.Collections.Generic;
using Newtonsoft.Json;

namespace Order.Service
{
    public class LogMessage
    {
        public string CorrelationId { get; set; }
        public int EventId { get; set; }
        public Dictionary<string, object> Payload { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}
