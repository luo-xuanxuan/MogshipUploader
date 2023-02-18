using Newtonsoft.Json;

namespace MogshipUploader.Util
{
    public class UploaderVersionPacket
    {
        [JsonProperty]
        public string Version;

        public UploaderVersionPacket()
        {
            Version = "0.2-alpha";
        }

        public string getJSONString()
        {
            return $"\"Version\":{Version}";
        }

    }
}
