using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace SimpleTweaksPlugin.Tweaks.UiAdjustment
{
    internal class HideCursorInCutscene : UiAdjustments.SubTweak
    {
        public override string Name => "Hide Cursor In Cutscene";
        public override string Description => "Auto hide mouse cursor in cutscenes.";
        protected override string Author => "akira0245";
        [DllImport("user32.dll")] static extern int ShowCursor(bool bShow);
        private bool CutsceneActive => Service.Condition[ConditionFlag.OccupiedInCutSceneEvent] || Service.Condition[ConditionFlag.WatchingCutscene78];
        unsafe bool IsAddonVisible(string name) => Service.GameGui.GetAddonByName(name, 1) is var a && a != IntPtr.Zero && ((AtkUnitBase*)a)->IsVisible;

        private int cursorCount;
        void HideCursor()
        {
            while (cursorCount >= 0)
            {
                cursorCount = ShowCursor(false);
            }
        }
        void ShowCursor()
        {
            while (cursorCount < 0)
            {
                cursorCount = ShowCursor(true);
            }
        }
        public override void Enable()
        {
            cursorCount = ShowCursor(true);
            Service.Framework.Update += Framework_Update; ;
            base.Enable();
        }

        private void Framework_Update(Dalamud.Game.Framework framework)
        {
            if (CutsceneActive)
            {
                //PluginLog.Verbose($"{cursorCount}");
                if (!IsAddonVisible("SelectYesno") &&
                    !IsAddonVisible("CutSceneSelectString") &&
                    !IsAddonVisible("JournalResult") &&
                    !IsAddonVisible("SelectString") &&
                    !IsAddonVisible("SelectIconString"))
                {
                    HideCursor();
                    return;
                }
            }
            ShowCursor();
        }

        public override void Disable()
        {
            Service.Framework.Update -= Framework_Update;
            ShowCursor();
            base.Disable();
        }
    }
}
