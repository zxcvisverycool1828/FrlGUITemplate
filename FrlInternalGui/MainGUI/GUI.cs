using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FrlGUITemplate.MainGUI
{
    [BepInPlugin("frl.internal.template", "frltemplate", "1.0.6")]
    public class GUIMain : BaseUnityPlugin
    {
        public enum Theme { Dark, VeryDark, Light, Space, Purple, Oreo, RGB, Solarized, Neon, Forest }

        public static Theme currentTheme = Theme.Dark;
        public Color windowColor;
        public Color buttonColor;
        public Color textFieldColor;
        public Color labelTextColor;

        private GUIStyle windowStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle textFieldStyle;
        private GUIStyle verticalScrollbarStyle;
        private GUIStyle verticalScrollbarThumbStyle;

        private static float controlSpacing = 10f;
        private static float horizontalPadding = 10f;
        private static float verticalPadding = 10f;
        public static float buttonHeight = 30f;

        public int cornerRadius = 8;

        private Rect mainWindowRect = new Rect(100, 50, 800, 325);
        private int currentTab = 0;
        private Vector2 scrollPosition = Vector2.zero;

        private List<ModCategory> modCategories = new List<ModCategory>();
        private bool showWindow = true;
        private Theme lastAppliedTheme = (Theme)(-1);
        private Texture2D cachedTabButtonActiveTex;
        private Texture2D cachedTabButtonInactiveTex;

        public class ModCategory
        {
            public string Name;
            public Type ClassType;
            public List<ModMethod> Methods = new List<ModMethod>();
        }

        public class ModMethod
        {
            public string MethodName;
            public string DisplayName;
            public bool IsToggle;
            public bool IsEnabled;
            public MethodInfo Method;
            public object TargetInstance;
        }

        void Awake()
        {
            LoadModsFromAssembly();

            Debug.Log($"Loaded {modCategories.Count} mod categories with {modCategories.Sum(c => c.Methods.Count)} total mods.");
        }

        private void LoadModsFromAssembly()
        {
            modCategories.Clear();

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var modTypes = assembly.GetTypes()
                    .Where(t => t.Namespace == "FrlGUITemplate.Mods" && !t.IsAbstract)
                    .ToList();

                foreach (var modType in modTypes)
                {
                    var category = new ModCategory
                    {
                        Name = modType.Name,
                        ClassType = modType
                    };

                    var instance = Activator.CreateInstance(modType);

                    var methods = modType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    foreach (var method in methods)
                    {
                        if (method.IsSpecialName || method.GetParameters().Length > 0)
                            continue;

                        string methodName = method.Name;

                        var nameField = modType.GetField(methodName + "Name");
                        var isToggleField = modType.GetField(methodName + "IsToggle");
                        var isEnabledField = modType.GetField(methodName + "IsEnabled");

                        if (nameField != null)
                        {
                            var modMethod = new ModMethod
                            {
                                MethodName = methodName,
                                DisplayName = nameField.GetValue(instance)?.ToString() ?? methodName,
                                IsToggle = (isToggleField != null) ? (bool)isToggleField.GetValue(instance) : false,
                                IsEnabled = (isEnabledField != null) ? (bool)isEnabledField.GetValue(instance) : false,
                                Method = method,
                                TargetInstance = instance
                            };

                            category.Methods.Add(modMethod);
                        }
                    }

                    if (category.Methods.Count > 0)
                    {
                        modCategories.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading mods: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void ExecuteModMethod(ModMethod modMethod)
        {
            try
            {
                if (modMethod.Method != null && modMethod.TargetInstance != null)
                {
                    if (modMethod.IsToggle)
                    {
                        modMethod.IsEnabled = !modMethod.IsEnabled;

                        var isEnabledField = modMethod.TargetInstance.GetType().GetField(modMethod.MethodName + "IsEnabled");

                        if (isEnabledField != null)
                        {
                            isEnabledField.SetValue(modMethod.TargetInstance, modMethod.IsEnabled);
                        }
                    }

                    modMethod.Method.Invoke(modMethod.TargetInstance, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking mod {modMethod.MethodName}: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void ApplyTheme()
        {
            if (currentTheme != lastAppliedTheme)
            {
                switch (currentTheme)
                {
                    case Theme.Dark:
                        windowColor = new Color(0.05f, 0.05f, 0.05f);
                        buttonColor = new Color(0.1f, 0.1f, 0.1f);
                        textFieldColor = new Color(0.1f, 0.1f, 0.1f);
                        labelTextColor = Color.white;
                        break;
                    case Theme.VeryDark:
                        windowColor = new Color(0.02f, 0.02f, 0.02f);
                        buttonColor = new Color(0.03f, 0.03f, 0.03f);
                        textFieldColor = new Color(0.03f, 0.03f, 0.03f);
                        labelTextColor = Color.white;
                        break;
                    case Theme.Light:
                        windowColor = new Color(0.8f, 0.8f, 0.8f);
                        buttonColor = new Color(0.9f, 0.9f, 0.9f);
                        textFieldColor = new Color(0.9f, 0.9f, 0.9f);
                        labelTextColor = Color.black;
                        break;
                    case Theme.Space:
                        windowColor = new Color(0f, 0f, 0.2f);
                        buttonColor = new Color(0f, 0f, 0.3f);
                        textFieldColor = new Color(0f, 0f, 0.3f);
                        labelTextColor = Color.cyan;
                        break;
                    case Theme.Purple:
                        windowColor = new Color(0.3f, 0f, 0.3f);
                        buttonColor = new Color(0.4f, 0f, 0.4f);
                        textFieldColor = new Color(0.4f, 0f, 0.4f);
                        labelTextColor = Color.white;
                        break;
                    case Theme.Oreo:
                        windowColor = Color.black;
                        buttonColor = new Color(0.2f, 0.2f, 0.2f);
                        textFieldColor = new Color(0.2f, 0.2f, 0.2f);
                        labelTextColor = Color.white;
                        break;
                    case Theme.RGB:
                        windowColor = Color.red;
                        buttonColor = Color.green;
                        textFieldColor = Color.blue;
                        labelTextColor = Color.green;
                        break;
                    case Theme.Solarized:
                        windowColor = new Color(0.0f, 0.168f, 0.211f);
                        buttonColor = new Color(0.282f, 0.337f, 0.396f);
                        textFieldColor = new Color(0.282f, 0.337f, 0.396f);
                        labelTextColor = new Color(0.976f, 0.976f, 0.949f);
                        break;
                    case Theme.Neon:
                        windowColor = Color.black;
                        buttonColor = new Color(0.0f, 1.0f, 1.0f);
                        textFieldColor = new Color(1.0f, 0.0f, 1.0f);
                        labelTextColor = new Color(1.0f, 1.0f, 0.0f);
                        break;
                    case Theme.Forest:
                        windowColor = new Color(0.133f, 0.545f, 0.133f);
                        buttonColor = new Color(0.180f, 0.624f, 0.180f);
                        textFieldColor = new Color(0.180f, 0.624f, 0.180f);
                        labelTextColor = new Color(0.0f, 0.392f, 0.0f);
                        break;
                }

                windowStyle = null;
                buttonStyle = null;
                labelStyle = null;
                textFieldStyle = null;

                if (cachedTabButtonActiveTex != null)
                {
                    Destroy(cachedTabButtonActiveTex);
                    cachedTabButtonActiveTex = null;
                }
                if (cachedTabButtonInactiveTex != null)
                {
                    Destroy(cachedTabButtonInactiveTex);
                    cachedTabButtonInactiveTex = null;
                }
                lastAppliedTheme = currentTheme;
            }
        }

        private void CycleTheme()
        {
            currentTheme = (Theme)(((int)currentTheme + 1) % Enum.GetValues(typeof(Theme)).Length);
            ApplyTheme();
        }

        private Texture2D MakeTex(int width, int height, Color col)
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

        private Texture2D MakeRoundedRectTexture(int width, int height, Color col, int roundPixels)
        {
            return GUIHelper.MakeRoundedRectTexture(width, height, col, roundPixels);
        }


        private void DrawMainWindow(int windowID)
        {
            float sidebarWidth = 150f;
            float sidebarX = horizontalPadding;
            float sidebarY = windowStyle.padding.top;

            if (cachedTabButtonActiveTex == null || cachedTabButtonInactiveTex == null)
            {
                cachedTabButtonActiveTex = GUIHelper.MakeRoundedRectTexture(64, 64, new Color(0.1f, 0.5f, 1f), cornerRadius);
                cachedTabButtonInactiveTex = GUIHelper.MakeRoundedRectTexture(64, 64, buttonColor, cornerRadius);
            }

            for (int i = 0; i < modCategories.Count; i++)
            {
                Rect tabButtonRect = new Rect(sidebarX, sidebarY + i * (buttonHeight + controlSpacing), sidebarWidth - horizontalPadding, buttonHeight);
                GUIStyle tabButtonStyle = new GUIStyle(buttonStyle);
                tabButtonStyle.fontStyle = FontStyle.Bold;
                tabButtonStyle.alignment = TextAnchor.MiddleCenter;

                Texture2D tabTex = i == currentTab ? cachedTabButtonActiveTex : cachedTabButtonInactiveTex;
                tabButtonStyle.normal.background = tabTex;
                tabButtonStyle.hover.background = tabTex;
                tabButtonStyle.normal.textColor = i == currentTab ? Color.white : labelTextColor;
                if (GUI.Button(tabButtonRect, modCategories[i].Name, tabButtonStyle))
                {
                    currentTab = i;
                }
            }

            Rect themeButtonRect = new Rect(sidebarX, sidebarY + modCategories.Count * (buttonHeight + controlSpacing), sidebarWidth - horizontalPadding, buttonHeight);
            if (GUI.Button(themeButtonRect, "Theme: " + currentTheme.ToString(), buttonStyle))
            {
                CycleTheme();
            }

            float contentX = sidebarWidth + controlSpacing + horizontalPadding;
            float contentWidth = mainWindowRect.width - contentX - horizontalPadding;
            float scrollYOffset = 40f;
            float scrollY = windowStyle.padding.top + scrollYOffset;
            float scrollHeight = mainWindowRect.height - scrollY - verticalPadding;
            Rect scrollViewRect = new Rect(contentX, scrollY, contentWidth, scrollHeight);
            Rect viewRect = new Rect(0, 0, contentWidth, 800);
            scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, viewRect, GUIStyle.none, verticalScrollbarStyle);

            float contentY = 0;
            if (currentTab >= 0 && currentTab < modCategories.Count)
            {
                DrawCategoryMethods(modCategories[currentTab], contentWidth, ref contentY);
            }

            GUI.EndScrollView();
            GUI.DragWindow();
        }

        private void DrawCategoryMethods(ModCategory category, float contentWidth, ref float contentY)
        {
            foreach (var method in category.Methods)
            {
                string buttonText = method.DisplayName;

                if (method.IsToggle)
                {
                    buttonText += method.IsEnabled ? " [ON]" : " [OFF]";
                }

                Rect methodRect = new Rect(0, contentY, contentWidth, buttonHeight);
                GUIStyle methodButtonStyle = new GUIStyle(buttonStyle);

                if (method.IsToggle && method.IsEnabled)
                {
                    methodButtonStyle.normal.background = MakeRoundedRectTexture(64, 64, new Color(0.2f, 0.7f, 0.2f), cornerRadius);
                }

                if (GUI.Button(methodRect, buttonText, methodButtonStyle))
                {
                    ExecuteModMethod(method);
                }

                contentY += buttonHeight + controlSpacing;
            }
        }

        void OnGUI()
        {

            ApplyTheme();

            Rect statusRect = new Rect(10, 10, 500, buttonHeight);
            GUI.Button(statusRect,
                $"GUI Template | Ping: {PhotonNetwork.GetPing()} | FPS: {(int)(1.0f / Time.deltaTime)} | {(NetworkSystem.Instance == null ? "None" : NetworkSystem.Instance.netState)}",
                buttonStyle);

            if (verticalScrollbarStyle == null)
            {
                verticalScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
                verticalScrollbarStyle.fixedWidth = 12;
                verticalScrollbarStyle.normal.background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f));
            }
            if (verticalScrollbarThumbStyle == null)
            {
                verticalScrollbarThumbStyle = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                verticalScrollbarThumbStyle.fixedWidth = 12;
                verticalScrollbarThumbStyle.normal.background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f));
            }
            GUI.skin.verticalScrollbar = verticalScrollbarStyle;
            GUI.skin.verticalScrollbarThumb = verticalScrollbarThumbStyle;

            float designWidth = 1920f;
            float designHeight = 1080f;
            float scaleX = Screen.width / designWidth;
            float scaleY = Screen.height / designHeight;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleY, 1));

            if (windowStyle == null)
            {
                windowStyle = new GUIStyle(GUI.skin.window);
                Texture2D windowTex = MakeRoundedRectTexture(64, 64, windowColor, cornerRadius);
                windowStyle.normal.background = windowTex;
                windowStyle.active.background = windowTex;
                windowStyle.hover.background = windowTex;
                windowStyle.focused.background = windowTex;
                windowStyle.onNormal.background = windowTex;
                windowStyle.onActive.background = windowTex;
                windowStyle.onHover.background = windowTex;
                windowStyle.onFocused.background = windowTex;
                windowStyle.normal.textColor = labelTextColor;
                windowStyle.padding.top = 30;
                windowStyle.fontSize = 24;
                windowStyle.fontStyle = FontStyle.Bold;
                windowStyle.border = new RectOffset(cornerRadius, cornerRadius, cornerRadius, cornerRadius);
            }
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                Texture2D btnTex = MakeRoundedRectTexture(64, 64, buttonColor, cornerRadius);
                buttonStyle.normal.background = btnTex;
                buttonStyle.active.background = btnTex;
                buttonStyle.hover.background = btnTex;
                buttonStyle.focused.background = btnTex;
                buttonStyle.onNormal.background = btnTex;
                buttonStyle.onActive.background = btnTex;
                buttonStyle.onHover.background = btnTex;
                buttonStyle.onFocused.background = btnTex;
                buttonStyle.normal.textColor = labelTextColor;
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.border = new RectOffset(cornerRadius, cornerRadius, cornerRadius, cornerRadius);
            }
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.normal.textColor = labelTextColor;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.alignment = TextAnchor.MiddleCenter;
            }
            if (textFieldStyle == null)
            {
                textFieldStyle = new GUIStyle(GUI.skin.textField);
                Texture2D textFieldTex = MakeTex(2, 2, textFieldColor);
                textFieldStyle.normal.background = textFieldTex;
                textFieldStyle.active.background = textFieldTex;
                textFieldStyle.hover.background = textFieldTex;
                textFieldStyle.focused.background = textFieldTex;
                textFieldStyle.onNormal.background = textFieldTex;
                textFieldStyle.onActive.background = textFieldTex;
                textFieldStyle.onHover.background = textFieldTex;
                textFieldStyle.onFocused.background = textFieldTex;
                textFieldStyle.normal.textColor = labelTextColor;
                textFieldStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (showWindow)
            {
                mainWindowRect = GUI.Window(0, mainWindowRect, DrawMainWindow, "<color=white>GUI Template</color>", windowStyle);
            }

            GUIHelper.buttonStyle = buttonStyle;
            GUIHelper.labelStyle = labelStyle;
            GUIHelper.textFieldStyle = textFieldStyle;
        }

        public static float LastPress = 0f;
        void Update()
        {
            try
            {
                float currentTime = Time.time;

                if (Keyboard.current != null && Keyboard.current.deleteKey.wasPressedThisFrame)
                {
                    if (currentTime - LastPress > 0.2f)
                    {
                        showWindow = !showWindow;

                        LastPress = currentTime;
                    }
                }
            }
            catch
            {

            }
        }
    }
}