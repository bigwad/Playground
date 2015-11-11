using Starcounter;

namespace Playground {
    partial class SubPage : Page {
        protected override void OnData() {
            base.OnData();

            this.Items.Add().StringValue = "i7 - 4790k";
            this.Items.Add().StringValue = "i7 - 5820k";
            this.Items.Add().StringValue = "i7 - 6700k";
        }

        void Handle(Input.Remove Action) {
            this.Items.RemoveAt(this.Items.Count - 1);
        }
    }
}
