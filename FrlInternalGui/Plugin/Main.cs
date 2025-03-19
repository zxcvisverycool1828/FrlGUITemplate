using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using ExitGames.Client.Photon;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Voice.PUN.UtilityScripts;
using UnityEngine;

namespace FrlGUITemplate.Plugin
{
    [Description("template")]
    [BepInPlugin("frl.template.gui", "frltemplate", "1.0.0")]
    public class FrlInternalTemplateMain : BaseUnityPlugin
    {


        void OnEnable()
        {
            ApplyHarmonyPatches();
        }


        void OnDisable()
        {
            RemoveHarmonyPatches();
        }

        public static bool IsPatched { get; private set; }

        internal static void ApplyHarmonyPatches()
        {
            if (!IsPatched)
            {
                if (instance == null)
                {
                    instance = new Harmony("frl.template.gui");
                }
                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            if (instance != null && IsPatched)
            {
                IsPatched = false;
            }
        }

        private static Harmony instance;
    }
}
