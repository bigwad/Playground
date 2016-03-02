using Starcounter;

namespace Playground {
    partial class IndexPage : Page {
        protected override void OnData() {
            base.OnData();

            //this.Items.Add().Name = "Value 0";
            this.SubPage = Self.GET("/playground/sub");
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

        [IndexPage_json.Items]
        partial class IndexPageItem : Json {
        }
    }
}
