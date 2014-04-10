using System;
using UnityEngine;

namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AGExt : PartModule
    {
        public void Update()
        {
            foreach (string str in Enum.GetNames(typeof(KSPActionGroup)))
            {
                print(str);
            }
        }
    }

}
