using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Larje.Core.Saves.Serialization
{
    public sealed class UnityFieldContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            List<MemberInfo> members = new List<MemberInfo>();
            Type current = objectType;
            while (current != null && current != typeof(object))
            {
                FieldInfo[] fields = current.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach (FieldInfo field in fields)
                {
                    if (IsSerializable(field))
                    {
                        members.Add(field);
                    }
                }

                current = current.BaseType;
            }

            return members;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.Readable = true;
            property.Writable = true;
            return property;
        }

        private bool IsSerializable(FieldInfo field)
        {
            if (field.IsInitOnly || field.IsDefined(typeof(NonSerializedAttribute), false))
            {
                return false;
            }

            return field.IsPublic || field.IsDefined(typeof(SerializeField), false) || field.IsDefined(typeof(JsonPropertyAttribute), false);
        }
    }
}
