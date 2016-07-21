using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using KSP.UI.Screens;
using KSP.IO;

using UnityEngine;

namespace BobTracker
{
    /** Load in any editor once */
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]

    public class BobTracker : MonoBehaviour
    {
        /** modTag to use in every debug message */
        private static string modTag = "[BobTracker]: ";

        private bool isActive;
        private bool setup = false;
        private static ApplicationLauncherButton trButton;
        private Texture2D buttonTexture;
        private Texture2D onActive;
        private Texture2D onInactive;

        /** Called once at beginning */
        public void Awake()
        {
            print(modTag + "Awake()");

            /** Not active in the beginning */
            isActive = false;

            /** Load textures */
            onActive = loadTexture("BobTracker/icons/BT_active");
            onInactive = loadTexture("BobTracker/icons/BT_inactive");

            /** Register for events */
            GameEvents.onGUIApplicationLauncherReady.Add(setupButton);
        }

        public void OnDestroy()
        {
            print(modTag + "OnDestroy()");

            /** Unregister for events */
            GameEvents.onGUIApplicationLauncherReady.Remove(setupButton);
        }

        private static Texture2D loadTexture(string path)
        {
            print(modTag + "Loading texture: " + path);
            return GameDatabase.Instance.GetTexture(path, false);
        }

        public void setupButton()
        {
            print(modTag + "Setting up button");

            if(setup)
            {
                print(modTag + "Button already set up");
            } else if (ApplicationLauncher.Ready)
            {
                setup = true;
                if (trButton == null)
                {
                    ApplicationLauncher instance = ApplicationLauncher.Instance;
                    buttonTexture = onInactive;
                    trButton = instance.AddModApplication(onTrue, onFalse, nothing, nothing, nothing, nothing, ApplicationLauncher.AppScenes.VAB, buttonTexture);
                } else
                {
                    trButton.onFalse = onFalse;
                    trButton.onTrue = onTrue;
                }
            }
        }

        public void nothing()
        {
        }

        public void onTrue()
        {
            print(modTag + "onTrue()");
            isActive = true;
            trButton.SetTexture(onActive);
        }

        public void onFalse()
        {
            print(modTag + "onFalse()");
            isActive = false;
            trButton.SetTexture(onInactive);
        }
    }
}
