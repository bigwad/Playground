using Starcounter;

namespace Playground {
    partial class SubDetailsPage : Json {
        protected override void OnData() {
            base.OnData();

            this.Name = this.Data as string;
        }
    }
}
