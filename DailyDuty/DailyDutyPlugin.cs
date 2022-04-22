﻿using CheapLoc;
using DailyDuty.Enums;
using DailyDuty.System;
using DailyDuty.Utilities;
using DailyDuty.Windows.DailyDutyWindow;
using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace DailyDuty
{
    public sealed class DailyDutyPlugin : IDalamudPlugin
    {
        public string Name => "DailyDuty";
        private const string SettingsCommand = "/dailyduty";
        private const string ShorthandCommand = "/dd";

        public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();
            Service.Chat.Enable();

            Configuration.Startup();

            Loc.SetupWithFallbacks();

            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(ShorthandCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "shorthand command to open configuration window"
            });

            // Create Custom Services
            Service.System = new DailyDutySystem();
            Service.WindowManager = new WindowManager();

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            Service.ClientState.Login += Configuration.Login;
            Service.ClientState.Logout += Configuration.Logout;
        }

        private void OnCommand(string command, string arguments)
        {
            Service.System.ExecuteCommand(command, arguments);
            Service.WindowManager.ExecuteCommand(command, arguments);
        }

        private void DrawUI() => Service.WindowSystem.Draw();

        private void DrawConfigUI()
        {
            var window = Service.WindowManager.GetWindowOfType<DailyDutyWindow>(WindowName.Main);

            if (window != null)
            {
                window.IsOpen = true;
            }
        }

        public void Dispose()
        {
            Service.System.Dispose();
            Service.WindowManager.Dispose();

            Service.ClientState.Login -= Configuration.Login;
            Service.ClientState.Logout -= Configuration.Logout;

            Service.PluginInterface.UiBuilder.Draw -= DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            Service.Commands.RemoveHandler(SettingsCommand);
            Service.Commands.RemoveHandler(ShorthandCommand);

            Configuration.Cleanup();
        }
    }
}