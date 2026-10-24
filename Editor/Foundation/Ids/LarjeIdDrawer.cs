using System;
using System.Collections.Generic;
using Larje.Core.Ids;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Ids
{
    [CustomPropertyDrawer(typeof(LarjeId), true)]
    public sealed class LarjeIdDrawer : PropertyDrawer
    {
        private const float CREATE_BUTTON_WIDTH = 22f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            Type idType = fieldInfo.FieldType.IsArray
                ? fieldInfo.FieldType.GetElementType()
                : fieldInfo.FieldType.IsGenericType ? fieldInfo.FieldType.GetGenericArguments()[0] : fieldInfo.FieldType;

            List<LarjeId> ids = LarjeIdAssets.FindAll(idType);
            string[] names = new string[ids.Count + 1];
            names[0] = "None";
            int index = 0;
            for (int i = 0; i < ids.Count; i++)
            {
                names[i + 1] = ids[i].name;
                if (ids[i] == property.objectReferenceValue)
                {
                    index = i + 1;
                }
            }

            Rect popupRect = new Rect(position.x, position.y, position.width - CREATE_BUTTON_WIDTH - 2f, position.height);
            Rect buttonRect = new Rect(position.xMax - CREATE_BUTTON_WIDTH, position.y, CREATE_BUTTON_WIDTH, position.height);

            EditorGUI.BeginProperty(position, label, property);
            int selected = EditorGUI.Popup(popupRect, label.text, index, names);
            if (selected != index)
            {
                property.objectReferenceValue = selected == 0 ? null : ids[selected - 1];
            }

            if (GUI.Button(buttonRect, "+"))
            {
                LarjeId created = LarjeIdAssets.CreateWithDialog(idType);
                if (created != null)
                {
                    property.objectReferenceValue = created;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
