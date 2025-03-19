using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FrlGUITemplate.Mods
{
    public class Misc
    {
        public string Cool2Name = "Cool 257 Mod";
        public bool Cool2IsToggle = true;
        public bool Cool2IsEnabled = false;
        public async void Cool2()
        {
            while (Cool2IsEnabled)
            {
                Debug.Log("cool");

                await Task.Delay(1);
            }

            Debug.Log("disabled");
        }
    }
}