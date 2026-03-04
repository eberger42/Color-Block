using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public static class EditorUtility
    {
        public static void DrawOutline(Rect rect, Color color, float thickness)
        {
            // Top
            EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y - thickness, rect.width + thickness * 2, thickness), Color.white);
            // Bottom
            EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y + rect.height, rect.width + thickness * 2, thickness), Color.white);
            // Left
            EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y, thickness, rect.height), Color.white);
            // Right
            EditorGUI.DrawRect(new Rect(rect.x + rect.width, rect.y, thickness, rect.height), Color.white);
        }

    }
}
