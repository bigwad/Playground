using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Starcounter.Linq;

namespace Playground
{
    partial class IndexPage : Json
    {
        public void Init()
        {
            this.RefreshItems();
        }

        public void RefreshItems()
        {
            this.Items.Clear();

            IEnumerable<Database.Person> items = DbLinq.Objects<Database.Person>().OrderBy(x => x.Date);

            foreach (Database.Person item in items)
            {
                this.Items.Add().Data = new Database.ItemProxy(item);
            }
        }

        public void ItemInserted(ulong no)
        {
            var row = this.Items.FirstOrDefault(x => x.Data.ObjectNo == no);
            Database.Person item = Db.FromId<Database.Person>(no);

            if (item == null || row != null)
            {
                return;
            }

            Database.ItemProxy proxy = new Database.ItemProxy(item);

            this.Items.Add().Data = proxy;
        }

        public void ItemDeleted(ulong no)
        {
            var row = this.Items.FirstOrDefault(x => x.Data.ObjectNo == no);

            if (row != null)
            {
                this.Items.Remove(row);
            }
        }

        public void ItemUpdated(ulong no)
        {
            var row = this.Items.FirstOrDefault(x => x.Data.ObjectNo == no);
            Database.Person item = Db.FromId<Database.Person>(no);

            if (item != null)
            {
                row.Data = new Database.ItemProxy(item);
            }
        }

        protected void Handle(Input.InsertTrigger action)
        {
            action.Cancel();

            Database.ItemProxy proxy = null;

            Db.Transact(() =>
            {
                Database.Person item = new Database.Person()
                {
                    Guid = Guid.NewGuid().ToString(),
                    Date = DateTime.Now
                };

                proxy = new Database.ItemProxy(item);
            });

            this.InsertedObjectNo = (long)proxy.ObjectNo;
            this.Items.Add().Data = proxy;
        }

        [IndexPage_json.Items]
        partial class IndexPageItems : Json, IBound<Database.ItemProxy>
        {
            public IndexPage ParentPage => this.Parent.Parent as IndexPage;

            protected void Handle(Input.DeleteTrigger action)
            {
                this.Data.Delete();
                this.ParentPage.Items.Remove(this);
            }

            protected void Handle(Input.UpdateTrigger action)
            {
                action.Cancel();
                this.Data.Update();
            }
        }
    }
}
