using System;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Linq;

namespace Playground
{
    partial class TableItemsJson : Json
    {
        public TableItemsJson()
        {
            this.Columns = Self.GET("/playground/table-columns-items");
            this.RefreshItems();
        }

        protected void Handle(Input.RefreshItemsTrigger action)
        {
            action.Cancel();
            this.RefreshItems();
        }

        protected void RefreshItems()
        {
            this.Items.Clear();

            Database.Person[] items = DbLinq.Objects<Database.Person>()
                .Where(x => !x.TableHidden)
                .OrderBy(x => x.TableSortIndex).ToArray();

            foreach (Database.Person item in items)
            {
                Json json = Self.GET("/playground/table-row-item/" + item.GetObjectID());
                this.Items.Add(json);
            }
        }
    }
}
