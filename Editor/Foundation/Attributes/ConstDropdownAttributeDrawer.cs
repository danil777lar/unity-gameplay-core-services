using System;
using System.Collections.Generic;
using System.Reflection;
using Larje.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(ConstDropdownAttribute))]
    public sealed class ConstDropdownAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type source = ((ConstDropdownAttribute)attribute).Source;
            List<FieldInfo> constants = GetConstants(source, GetValueType(property));
            if (constants.Count == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            string[] names = new string[constants.Count];
            int index = 0;
            for (int i = 0; i < constants.Count; i++)
            {
                names[i] = constants[i].Name;
                if (Equals(constants[i].GetValue(null), GetValue(property)))
                {
                    index = i;
                }
            }

            EditorGUI.BeginProperty(position, label, property);
            int selected = EditorGUI.Popup(position, label.text, index, names);
            SetValue(property, constants[selected].GetValue(null));
            EditorGUI.EndProperty();
        }

        private List<FieldInfo> GetConstants(Type source, Type valueType)
        {
            List<FieldInfo> constants = new List<FieldInfo>();
            if (valueType == null)
            {
                return constants;
            }

            foreach (FieldInfo field in source.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == valueType)
                {
                    constants.Add(field);
                }
            }

            return constants;
        }

        private Type GetValueType(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    return typeof(string);
                case SerializedPropertyType.Integer:
                    return typeof(int);
                case SerializedPropertyType.Float:
                    return typeof(float);
                default:
                    return null;
            }
        }

        private object GetValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Integer:
                    return property.intValue;
                default:
                    return property.floatValue;
            }
        }

        private void SetValue(SerializedProperty property, object constant)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    property.stringValue = (string)constant;
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = (int)constant;
                    break;
                default:
                    property.floatValue = (float)constant;
                    break;
            }
        }
    }
}
