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
            IEnumerable<Database.Item> items = DbLinq.Objects<Database.Item>().OrderBy(x => x.Date);

            foreach (Database.Item item in items)
            {
                this.Items.Add().Data = new Database.ItemProxy(item);
            }
        }

        public void ItemInserted(ulong no)
        {
            Database.Item item = Db.FromId<Database.Item>(no);
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
                //this.Items.Remove(row);
                System.Threading.Interlocked.Increment(ref Counter);
                row.Data.Notes += $" Removed: {Counter};";
            }
        }

        public void ItemUpdated(ulong no)
        {
            var row = this.Items.FirstOrDefault(x => x.Data.ObjectNo == no);
            Database.Item item = Db.FromId<Database.Item>(no);

            row.Data = new Database.ItemProxy(item);
        }

        [IndexPage_json.Items]
        partial class IndexPageItems : Json, IBound<Database.ItemProxy>
        {
        }
    }
}
