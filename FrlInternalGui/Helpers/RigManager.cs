using Photon.Realtime;
using Photon.Pun;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace FrlGUITemplate.Helpers
{
    internal class RigStuff : BaseUnityPlugin
    {
        public static VRRig GetRigFromPlayer(Player pbbvisreal)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(pbbvisreal);
        }

        public static PhotonView GetPhotonViewFromVRRig(VRRig pbbvisreal)
        {
            return (PhotonView)Traverse.Create(pbbvisreal).Field("photonView").GetValue();
        }
    }
}