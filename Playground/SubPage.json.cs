using Starcounter;

namespace Playground {
    partial class SubPage : Json {
        protected int counter = 1;

        protected override void OnData() {
            base.OnData();

            this.Items.Add().StringValue = "i7-4790k";
            this.Items.Add().StringValue = "i7-5820k";
            this.Items.Add().StringValue = "i7-6700k";
        }

        protected void SetDetails(string Name) {
            if (string.IsNullOrEmpty(Name)) {
                this.Details = null;
            } else {
                this.Details = Self.GET("/playground/details/" + Name);
            }
        }

        void Handle(Input.SelectedItem Action) {
            this.SetDetails(Action.Value);
        }

        void Handle(Input.Remove Action) {
            this.Items.RemoveAt(this.Items.Count - 1);
            this.SetDetails(null);
        }

        void Handle(Input.Add Action) {
            this.Items.Add().StringValue = "Item-" + counter++;
            this.SetDetails(null);
            this.Message = "An item was added";
        }
    }
}
