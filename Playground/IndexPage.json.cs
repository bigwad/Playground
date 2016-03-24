using Starcounter;

namespace Playground {
    partial class IndexPage : Page {
        static IndexPage() {
        }

        protected override void OnData() {
            base.OnData();

            this.MessageText = null;
            this.MessageButton = null;
            this.Button = 0;
            this.Text = null;
            this.Items.Clear();
            this.AddItem = 0;
            this.RemoveItem = 0;
            this.AddAndRemoveItem = 0;
            this.SubPage = Self.GET("/playground/sub");
            this.LoadSubPage = 0;
            this.UnloadSubPage = 0;
        }

        void Handle(Input.Button Action) {
            this.MessageButton = string.Format("Button was clicked {0} time(s)!", Action.Value);
        }

        void Handle(Input.Text Action) {
            this.MessageText = string.Format("Text was changed to '{0}'!", Action.Value);
        }

        void Handle(Input.AddItem Action) {
            this.Items.Add().Name = "Value " + Action.Value;
        }

        void Handle(Input.RemoveItem Action) {
            if (this.Items.Count < 1) {
                return;
            }

            this.Items.RemoveAt(0);
        }

        void Handle(Input.AddAndRemoveItem Action) {
            this.Items.Add().Name = "Value " + Action.Value;

            if (this.Items.Count > 0) {
                this.Items.RemoveAt(0);
            }
        }

        void Handle(Input.ResetItems Action) {
            this.Items.Data = new object[] {
                new { Name = "Reset item 0" },
                new { Name = "Reset item 1" }
            };
        }

        void Handle(Input.LoadSubPage Action) {
            Action.Cancel();
            this.SubPage = Self.GET("/playground/sub");
        }

        void Handle(Input.UnloadSubPage Action) {
            Action.Cancel();
            this.SubPage = null;
        }
    }
}
