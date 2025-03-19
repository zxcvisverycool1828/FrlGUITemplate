using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FrlGUITemplate.MainGUI
{
    public class GUIHelper
    {
        public static GUIStyle buttonStyle;
        public static GUIStyle labelStyle;
        public static GUIStyle textFieldStyle;

        private static float controlSpacing = 10f;
        private static float horizontalPadding = 10f;
        private static float verticalPadding = 10f;
        public static float buttonHeight = 30f;

        public static int cornerRadius = 8;

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            result.filterMode = FilterMode.Bilinear;
            result.wrapMode = TextureWrapMode.Clamp;
            return result;
        }

        public static Texture2D MakeRoundedRectTexture(int width, int height, Color col, int roundPixels)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Color clear = new Color(0, 0, 0, 0);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inside = true;
                    if (x < roundPixels && y < roundPixels)
                    {
                        float dx = roundPixels - x;
                        float dy = roundPixels - y;
                        inside = (dx * dx + dy * dy) <= (roundPixels * roundPixels);
                    }
                    else if (x >= width - roundPixels && y < roundPixels)
                    {
                        float dx = x - (width - roundPixels);
                        float dy = roundPixels - y;
                        inside = (dx * dx + dy * dy) <= (roundPixels * roundPixels);
                    }
                    else if (x < roundPixels && y >= height - roundPixels)
                    {
                        float dx = roundPixels - x;
                        float dy = y - (height - roundPixels);
                        inside = (dx * dx + dy * dy) <= (roundPixels * roundPixels);
                    }
                    else if (x >= width - roundPixels && y >= height - roundPixels)
                    {
                        float dx = x - (width - roundPixels);
                        float dy = y - (height - roundPixels);
                        inside = (dx * dx + dy * dy) <= (roundPixels * roundPixels);
                    }
                    tex.SetPixel(x, y, inside ? col : clear);
                }
            }
            tex.Apply();

            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }


        public static bool DrawButton(float windowWidth, ref float currentY, string text, Action onClick)
        {
            float buttonWidth = windowWidth - 2 * horizontalPadding;
            Rect rect = new Rect(horizontalPadding, currentY, buttonWidth, buttonHeight);
            bool clicked = GUI.Button(rect, text, buttonStyle);
            if (clicked && onClick != null)
                onClick();
            currentY += buttonHeight + controlSpacing;
            return clicked;
        }

        public static void DrawLabel(float windowWidth, ref float currentY, string text)
        {
            float labelWidth = windowWidth - 2 * horizontalPadding;
            Rect rect = new Rect(horizontalPadding, currentY, labelWidth, buttonHeight);
            GUI.Label(rect, text, labelStyle);
            currentY += buttonHeight + controlSpacing;
        }

        public static void DrawLabelTextField(float windowWidth, ref float currentY, string labelText, ref string text, int maxLength, float labelWidthFactor = 0.4f)
        {
            float totalWidth = windowWidth - 2 * horizontalPadding;
            float labelWidth = totalWidth * labelWidthFactor;
            float textFieldWidth = totalWidth - labelWidth - controlSpacing;
            Rect labelRect = new Rect(horizontalPadding, currentY, labelWidth, buttonHeight);
            GUI.Label(labelRect, labelText, labelStyle);
            Rect textFieldRect = new Rect(horizontalPadding + labelWidth + controlSpacing, currentY, textFieldWidth, buttonHeight);
            text = GUI.TextField(textFieldRect, text, maxLength, textFieldStyle);
            currentY += buttonHeight + controlSpacing;
        }

        public static void DrawToggle(float windowWidth, ref float currentY, string label, ref bool value, Action<bool> onToggle)
        {
            float buttonWidth = windowWidth - 2 * horizontalPadding;
            Rect rect = new Rect(horizontalPadding, currentY, buttonWidth, buttonHeight);

            GUIStyle toggleStyle = new GUIStyle(buttonStyle);
            if (value)
            {
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                bool clicked = GUI.Button(rect, label + " [ON]", toggleStyle);
                GUI.backgroundColor = originalColor;

                if (clicked)
                {
                    value = !value;
                    onToggle?.Invoke(value);
                }
            }
            else
            {
                bool clicked = GUI.Button(rect, label + " [OFF]", toggleStyle);
                if (clicked)
                {
                    value = !value;
                    onToggle?.Invoke(value);
                }
            }

            currentY += buttonHeight + controlSpacing;
        }

        public static void DrawSeparator(float windowWidth, ref float currentY, float height = 2f)
        {
            Rect rect = new Rect(horizontalPadding, currentY, windowWidth - 2 * horizontalPadding, height);
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            GUI.Box(rect, "");
            GUI.backgroundColor = originalColor;
            currentY += height + controlSpacing;
        }

        public static bool DrawColoredButton(float windowWidth, ref float currentY, string text, Color color, Action onClick)
        {
            float buttonWidth = windowWidth - 2 * horizontalPadding;
            Rect rect = new Rect(horizontalPadding, currentY, buttonWidth, buttonHeight);

            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            bool clicked = GUI.Button(rect, text, buttonStyle);
            GUI.backgroundColor = originalColor;

            if (clicked && onClick != null)
                onClick();

            currentY += buttonHeight + controlSpacing;
            return clicked;
        }
    }
}