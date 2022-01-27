﻿using System.Collections.Generic;
using System.Linq;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayTabs;
using DailyDuty.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class CustomDeliveries : DisplayModule
    {
        private static Weekly.CustomDeliveriesSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;
        protected override GenericSettings GenericSettings => Settings;

        public CustomDeliveries()
        {
            CategoryString = Strings.CustomDelivery.Category.Get();
        }

        protected override void DisplayData()
        {
            ImGui.Text(Strings.CustomDelivery.RemainingAllowances.Get().Format(Settings.AllowancesRemaining));
            ImGui.Spacing();

            foreach (var (npcID, npcCount) in Settings.DeliveryNPC)
            {
                var npcName = GetNameForNPC(npcID);
                ImGui.Text($"{npcName}: {npcCount}");
            }
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            ImGui.Text(Strings.Common.ManuallySetCounts.Get());
            ImGui.Spacing();

            foreach (var key in Settings.DeliveryNPC.Keys.ToList())
            {
                var npcName = GetNameForNPC(key);
                int tempCount = (int)Settings.DeliveryNPC[key];

                ImGui.PushItemWidth(30 * ImGuiHelpers.GlobalScale);
                if (ImGui.InputInt($"##{CategoryString}{key}", ref tempCount, 0, 0))
                {
                    if (Settings.DeliveryNPC[key] != tempCount)
                    {
                        if (tempCount is >= 0 and <= 6)
                        {
                            Settings.DeliveryNPC[key] = (uint)tempCount;
                            Service.Configuration.Save();
                        }
                    }
                }

                ImGui.PopItemWidth();

                ImGui.SameLine();

                ImGui.Text($"{npcName}");
            }
        }

        private string GetNameForNPC(uint id)
        {
            var npcData = Service.DataManager.GetExcelSheet<NotebookDivision>()
                !.GetRow(id);

            return npcData!.Name;
        }

        protected override void NotificationOptions()
        {
            PersistentNotification();
        }

        private void PersistentNotification()
        {
            ImGui.Checkbox(Strings.Common.PersistentReminders.Get(), ref Settings.PersistentReminders);
            ImGuiComponents.HelpMarker(Strings.Common.PersistentReminderDescription.Get());
            ImGui.Spacing();
        }

        public override void Dispose()
        {
        }
    }
}
