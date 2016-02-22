namespace Yasuo.Common.Classes
{
    using LeagueSharp.Common;

    public abstract class Child<T> : Base, IChild
        where T : Parent
    {
        protected Child(T parent)
        {
            this.Parent = parent;
        }

        public T Parent { get; set; }

        public bool Handled { get; protected set; }

        public override bool Enabled
        {
            get
            {
                return !this.Unloaded && this.Parent != null && this.Parent.Enabled && this.Menu != null &&
                       this.Menu.Item(this.Menu.Name + "Enabled").GetValue<bool>();
            }
        }

        public void HandleEvents()
        {
            if (this.Parent?.Menu == null || this.Menu == null || this.Handled)
            {
                return;
            }

            this.Parent.Menu.Item(this.Parent.Name + "Enabled").ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs args)
                    {
                        if (!Unloaded && args.GetNewValue<bool>())
                        {
                            if (Menu != null && Menu.Item(Menu.Name + "Enabled").GetValue<bool>())
                            {
                                OnEnable();
                            }
                        }
                        else
                        {
                            OnDisable();
                        }
                    };

            this.Menu.Item(this.Menu.Name + "Enabled").ValueChanged += delegate(object sender, OnValueChangeEventArgs args)
                {
                    if (!Unloaded && args.GetNewValue<bool>())
                    {
                        if (Parent.Menu != null && Parent.Menu.Item(Parent.Name + "Enabled").GetValue<bool>())
                        {
                            OnEnable();
                        }
                    }
                    else
                    {
                        OnDisable();
                    }
                };

            if (this.Enabled)
            {
                this.OnEnable();
            }

            this.Handled = true;
        }

        protected abstract void OnLoad();
    }
}
