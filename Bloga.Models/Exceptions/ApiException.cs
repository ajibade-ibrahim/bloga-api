using Newtonsoft.Json;

namespace Bloga.Models.Exceptions
{
    public class ApiException
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}