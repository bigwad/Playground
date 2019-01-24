using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Starcounter.Linq;

namespace Playground
{
    partial class IndexPage : Json
    {
        private static int Counter = 0;

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
            Database.Person item = Db.FromId<Database.Person>(no);
            Database.ItemProxy proxy = new Database.ItemProxy(item);

            System.Threading.Interlocked.Increment(ref Counter);
            proxy.Notes = $"Inserted: {Counter};";
            this.Items.Add().Data = proxy;
        }

        public void ItemDeleted(ulong no)
        {
            var row = this.Items.FirstOrDefault(x => x.Data.ObjectNo == no);

            if (row != null)
            {
                this.Items.Remove(row);
                System.Threading.Interlocked.Increment(ref Counter);
                //row.Data.Notes += $" Removed: {Counter};";
            }
        }

        public void ItemUpdated(ulong no)
        {
            var row = this.Items.FirstOrDefault(x => x.Data.ObjectNo == no);
            Database.Person item = Db.FromId<Database.Person>(no);

            row.Data = new Database.ItemProxy(item);
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

            this.Items.Add().Data = proxy;
        }

        [IndexPage_json.Items]
        partial class IndexPageItems : Json, IBound<Database.ItemProxy>
        {
            protected void Handle(Input.DeleteTrigger action)
            {
                this.Data.Delete();
            }

            protected void Handle(Input.UpdateTrigger action)
            {
                action.Cancel();
                this.Data.Update();
            }
        }
    }
}
