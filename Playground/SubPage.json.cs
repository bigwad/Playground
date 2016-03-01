using Starcounter;

namespace Playground {
    partial class SubPage : Page {
        protected int counter = 1;

        protected override void OnData() {
            base.OnData();

            this.Items.Add().StringValue = "i7 - 4790k";
            this.Items.Add().StringValue = "i7 - 5820k";
            this.Items.Add().StringValue = "i7 - 6700k";
        }

        void Handle(Input.Remove Action) {
            this.Items.RemoveAt(this.Items.Count - 1);
        }

        void Handle(Input.Add Action) {
            //this.Items.Add().StringValue = "An Item" + counter++;
            this.Items.Add().StringValue = "An Item";
        }
    }
}
