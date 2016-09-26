using Starcounter;

namespace Playground {
    partial class MatrixPage : Json {
        protected override void OnData() {
            base.OnData();

            for (int x = 0; x < 10; x++) {
                var row = this.Matrix.Add();

                for (int y = 0; y < 10; y++) {
                    row.Add().IntegerValue = y;
                }
            }
        }

        protected void Handle(Input.RevertClick Action) {
            Action.Cancel();

            for (var x = 0; x < this.Matrix.Count; x++) {
                var row = this.Matrix[x];

                for (var y = 0; y < row.Count; y++) {
                    row[y].IntegerValue = 9 - row[y].IntegerValue;
                }
            }
        }

        protected void Handle(Input.MinusClick Action) {
            Action.Cancel();

            for (var x = 0; x < this.Matrix.Count; x++) {
                var row = this.Matrix[x];

                for (var y = 0; y < row.Count; y++) {
                    row[y].IntegerValue--;
                }
            }
        }

        protected void Handle(Input.PlusClick Action) {
            Action.Cancel();

            for (var x = 0; x < this.Matrix.Count; x++) {
                var row = this.Matrix[x];

                for (var y = 0; y < row.Count; y++) {
                    row[y].IntegerValue++;
                }
            }
        }

        protected void Handle(Input.RemoveClick Action) {
            Action.Cancel();
            this.Matrix.Data = null;
        }
    }
}
