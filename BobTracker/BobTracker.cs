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
        private static ApplicationLauncherButton trButton;
        private Texture2D buttonTexture;
        private Texture2D onActive;
        private Texture2D onInactive;

        /** Called once at beginning */
        public void Awake()
        {
            debugPrint("Awake()");

            /** Load from config */
            PluginConfiguration config = PluginConfiguration.CreateForType<BobTracker>();
            config.load();
            isActive = config.GetValue<bool>("isActive");

            /** Don't auto-destroy when loading new scene */
            DontDestroyOnLoad(this);

            /** Load textures */
            onActive = loadTexture("BobTracker/icons/BT_active");
            onInactive = loadTexture("BobTracker/icons/BT_inactive");

            /** Register for events */
            GameEvents.onGUIApplicationLauncherReady.Add(setupButton);
            GameEvents.onVesselGoOffRails.Add(onVesselLaunch);
        }

        private void onVesselLaunch(Vessel data)
        {
            if (isActive)
            {
                debugPrint("checking crew");
                if (data.loaded)
                {
                    if (data.GetCrewCount() > 0)
                    {
                        List<ProtoCrewMember> members = data.GetVesselCrew();
                        bool hasScientist = false;
                        foreach (ProtoCrewMember m in members)
                        {
                            if (m.trait == Trait.SCIENTIST)
                            {
                                hasScientist = true;
                            }
                        }
                        if (!hasScientist)
                        {
                            ScreenMessages.PostScreenMessage("No scientist on vessel!", 10f);
                            debugPrint("no scientist on vessel");
                        }
                        else
                        {
                            ScreenMessages.PostScreenMessage("Scientist already on vessel!", 10f);
                            debugPrint("Scientist already on vessel");
                        }
                    }
                    else
                    {
                        debugPrint("no crew on vessel");
                        return;
                    }
                }
                else
                {
                    debugPrint("vessel not fully loaded");
                    return;
                }
            }

            /** Destroy object */
            Destroy(this);
        }

        public void OnDestroy()
        {
            debugPrint("OnDestroy()");

            /** Save to config */
            PluginConfiguration config = PluginConfiguration.CreateForType<BobTracker>();
            config.SetValue("isActive", isActive);
            config.save();

            /** Unregister for events */
            GameEvents.onGUIApplicationLauncherReady.Remove(setupButton);
            GameEvents.onVesselGoOffRails.Remove(onVesselLaunch);
        }

        private static Texture2D loadTexture(string path)
        {
            debugPrint("Loading texture: " + path);
            return GameDatabase.Instance.GetTexture(path, false);
        }

        public void setupButton()
        {
            if(ApplicationLauncher.Ready)
            {
                if (trButton == null)
                {
                    debugPrint("Setting up button");
                    ApplicationLauncher instance = ApplicationLauncher.Instance;
                    buttonTexture = isActive ? onActive : onInactive;
                    trButton = instance.AddModApplication(toggleActive, toggleActive, null, null, null, null, ApplicationLauncher.AppScenes.VAB, buttonTexture);
                } else
                {
                    trButton.onFalse = toggleActive;
                    trButton.onTrue = toggleActive;
                }
            }
        }

        private void toggleActive()
        {
            debugPrint("toggleAction()");
            isActive = !isActive;
            trButton.SetTexture(isActive ? onActive : onInactive);
        }

        private static void debugPrint(string s)
        {
            print(modTag + s);
        }
    }
}
