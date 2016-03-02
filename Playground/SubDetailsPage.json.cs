using Starcounter;

namespace Playground {
    partial class SubDetailsPage : Page {
        protected override void OnData() {
            base.OnData();

            this.Name = this.Data as string;
        }
    }
}
