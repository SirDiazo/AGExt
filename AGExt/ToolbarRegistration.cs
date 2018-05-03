using UnityEngine;
using ToolbarControl_NS;

namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(AGXFlight.MODID, AGXFlight.MODNAME);
        }
    }
}