using Newtonsoft.Json;

namespace Larje.Core.Saves.Serialization
{
    public static class SaveJson
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new UnityFieldContractResolver(),
            Converters = { new UnityJsonConverters() },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            NullValueHandling = NullValueHandling.Include
        };

        public static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);
    }
}
