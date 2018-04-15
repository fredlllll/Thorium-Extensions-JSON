using Newtonsoft.Json.Linq;

namespace Thorium.Extensions.JSON
{
    public interface IJSONConvertable
    {
        void FromJSON(JToken json);
        JToken ToJSON();
    }
}
