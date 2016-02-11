namespace Yasuo.Common
{
    using System;

    using LeagueSharp.Common;

    using Yasuo.Common.Extensions;
    using Yasuo.Common.Utility;

    public abstract class Base
    {
        protected Base()
        {
            Variables.Assembly.OnUnload += OnUnload;
        }

        public abstract bool Enabled { get; }

        public abstract string Name { get; }

        public bool Initialized { get; protected set; }

        public bool Unloaded { get; protected set; }

        public Menu Menu { get; set; }

        public event EventHandler OnInitialized;

        public event EventHandler OnEnabled;

        public event EventHandler OnDisabled;

        protected virtual void OnEnable()
        {
            if (Unloaded)
            {
                return;
            }

            if (!Initialized)
            {
                OnInitialize();
            }

            if (Initialized && !Enabled)
            {
                OnEnabled.RaiseEvent(null, null);
            }
        }

        protected virtual void OnDisable()
        {
            if (Initialized && Enabled && !Unloaded)
            {
                OnDisabled.RaiseEvent(null, null);
            }
        }

        protected virtual void OnInitialize()
        {
            if (Initialized || Unloaded)
            {
                return;
            }

            OnInitialized.RaiseEvent(this, null);

            Initialized = true;
        }

        protected virtual void OnUnload(object sender, UnloadEventArgs args)
        {
            if (Unloaded)
            {
                return;
            }

            OnDisable();

            if (args != null && args.Final)
            {
                Unloaded = true;
            }
        }

        public class UnloadEventArgs : EventArgs
        {
            public bool Final;

            public UnloadEventArgs(bool final = false)
            {
                Final = final;
            }
        }
    }
}