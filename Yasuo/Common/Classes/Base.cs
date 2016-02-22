namespace Yasuo.Common.Classes
{
    using System;

    using LeagueSharp.Common;

    using Yasuo.Common.Extensions;

    public abstract class Base
    {
        protected Base()
        {
            Variables.Assembly.OnUnload += this.OnUnload;
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
            if (this.Unloaded)
            {
                return;
            }

            if (!this.Initialized)
            {
                this.OnInitialize();
            }

            if (this.Initialized && !this.Enabled)
            {
                this.OnEnabled.RaiseEvent(null, null);
            }
        }

        protected virtual void OnDisable()
        {
            if (this.Initialized && this.Enabled && !this.Unloaded)
            {
                this.OnDisabled.RaiseEvent(null, null);
            }
        }

        protected virtual void OnInitialize()
        {
            if (this.Initialized || this.Unloaded)
            {
                return;
            }

            this.OnInitialized.RaiseEvent(this, null);

            this.Initialized = true;
        }

        protected virtual void OnUnload(object sender, UnloadEventArgs args)
        {
            if (this.Unloaded)
            {
                return;
            }

            this.OnDisable();

            if (args != null && args.Final)
            {
                this.Unloaded = true;
            }
        }

        public class UnloadEventArgs : EventArgs
        {
            public bool Final;

            public UnloadEventArgs(bool final = false)
            {
                this.Final = final;
            }
        }
    }
}