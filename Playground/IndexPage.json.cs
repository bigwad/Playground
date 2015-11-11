using Starcounter;

namespace Playground {
    partial class IndexPage : Page {
        protected override void OnData() {
            base.OnData();

            this.Items.Add().StringValue = "Value 0";
            this.SubPage = Self.GET("/playground/sub");
        }

        void Handle(Input.Button Action) {
            this.Message = string.Format("Button was clicked {0} time(s)!", Action.Value);
        }

        void Handle(Input.Text Action) {
            this.Message = string.Format("Text was changed to '{0}'!", Action.Value);
        }

        void Handle(Input.AddItem Action) {
            this.Items.Add().StringValue = "Value " + Action.Value;
        }

        void Handle(Input.RemoveItem Action) {
            if (this.Items.Count < 1) {
                return;
            }

            this.Items.RemoveAt(0);
        }
    }
}
