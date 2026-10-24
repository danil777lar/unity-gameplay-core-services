using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Larje.Core.Saves.Serialization
{
    public sealed class UnityJsonConverters : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2)
                || objectType == typeof(Vector3)
                || objectType == typeof(Vector4)
                || objectType == typeof(Vector2Int)
                || objectType == typeof(Vector3Int)
                || objectType == typeof(Quaternion)
                || objectType == typeof(Color);
        }

        public override void WriteJson(JsonWriter writer, object target, JsonSerializer serializer)
        {
            float[] values = ToArray(target);
            writer.WriteStartArray();
            foreach (float component in values)
            {
                writer.WriteValue(component);
            }

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return Activator.CreateInstance(objectType);
            }

            JArray array = JArray.Load(reader);
            float Get(int index)
            {
                return index < array.Count ? array[index].Value<float>() : 0f;
            }

            if (objectType == typeof(Vector2))
            {
                return new Vector2(Get(0), Get(1));
            }

            if (objectType == typeof(Vector3))
            {
                return new Vector3(Get(0), Get(1), Get(2));
            }

            if (objectType == typeof(Vector4))
            {
                return new Vector4(Get(0), Get(1), Get(2), Get(3));
            }

            if (objectType == typeof(Vector2Int))
            {
                return new Vector2Int((int)Get(0), (int)Get(1));
            }

            if (objectType == typeof(Vector3Int))
            {
                return new Vector3Int((int)Get(0), (int)Get(1), (int)Get(2));
            }

            if (objectType == typeof(Quaternion))
            {
                return new Quaternion(Get(0), Get(1), Get(2), Get(3));
            }

            return new Color(Get(0), Get(1), Get(2), Get(3));
        }

        private float[] ToArray(object target)
        {
            switch (target)
            {
                case Vector2 v:
                    return new[] { v.x, v.y };
                case Vector3 v:
                    return new[] { v.x, v.y, v.z };
                case Vector4 v:
                    return new[] { v.x, v.y, v.z, v.w };
                case Vector2Int v:
                    return new float[] { v.x, v.y };
                case Vector3Int v:
                    return new float[] { v.x, v.y, v.z };
                case Quaternion q:
                    return new[] { q.x, q.y, q.z, q.w };
                case Color c:
                    return new[] { c.r, c.g, c.b, c.a };
                default:
                    return new float[0];
            }
        }
    }
}
