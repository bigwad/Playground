using Starcounter;
using Playground.Database;

namespace Playground {
    partial class IndexPage : Page {
        static IndexPage() {
            DefaultTemplate.Items.Bind = "ItemsBind";
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

        public QueryResultRows<Item> ItemsBind {
            get {
                return Db.SQL<Item>("SELECT i FROM Playground.Database.Item i");
            }
        }

        void Handle(Input.AddItem Action) {
            new Item() {
                Name = "Value " + Action.Value
            };

            this.Transaction.Commit();
        }

        void Handle(Input.RemoveItem Action) {
            if (this.Items.Count < 1) {
                return;
            }

            this.Items[0].Data.Delete();
            this.Transaction.Commit();
        }

        void Handle(Input.AddAndRemoveItem Action) {
            if (this.Items.Count > 0) {
                this.Items[0].Data.Delete();
            }

            new Item() {
                Name = "Value " + Action.Value
            };

            this.Transaction.Commit();
        }

        void Handle(Input.ResetItems Action) {
            Db.SlowSQL("DELETE FROM Playground.Database.Item");

            new Item() { Name = "Reset item 0" };
            new Item() { Name = "Reset item 1" };

            this.Transaction.Commit();
        }

        void Handle(Input.Button Action) {
            this.MessageButton = string.Format("Button was clicked {0} time(s)!", Action.Value);
        }

        void Handle(Input.Text Action) {
            this.MessageText = string.Format("Text was changed to '{0}'!", Action.Value);
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
