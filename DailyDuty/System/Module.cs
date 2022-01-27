﻿using System;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;

namespace DailyDuty.System
{
    internal abstract class Module : IDisposable
    {
        protected Module()
        {
            Service.ClientState.Login += OnLogin;
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
        }

        public virtual void UpdateSlow()
        {
        }

        public virtual void Update()
        {
            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % 10 != 0) return;

            UpdateSlow();
        }

        public virtual void Dispose()
        {
            Service.ClientState.Login -= OnLogin;
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }

        protected virtual void OnLogin(object? sender, EventArgs e)
        {
            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(task => OnLoginDelayed());
        }

        protected abstract void OnLoginDelayed();
        protected abstract void OnTerritoryChanged(object? sender, ushort e);

        public abstract bool IsCompleted();

        public abstract void DoDailyReset(Configuration.CharacterSettings settings);

        public abstract void DoWeeklyReset(Configuration.CharacterSettings settings);
    }
}
