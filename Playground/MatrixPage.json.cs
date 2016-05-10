using Starcounter;

namespace Playground {
    partial class MatrixPage : Page {
        protected override void OnData() {
            base.OnData();

            for (int x = 0; x < 10; x++) {
                var row = this.Matrix.Add();

                for (int y = 0; y < 10; y++) {
                    row.Add().IntegerValue = y;
                }
            }
        }
    }
}
