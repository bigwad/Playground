using System;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Linq;

namespace Playground
{
    partial class TableColumnsItemsJson : Json
    {
        public TableColumnsItemsJson()
        {
            var column = this.Columns.Add();
            column.Title = "Identifier";
            column.Value = "Guid";
            column.Type = "text";
        }

        [TableColumnsItemsJson_json.Columns]
        partial class TableColumnsItemsJsonColumns : Json
        {
            protected void Handle(Input.SortTrigger action)
            {
                action.Cancel();

                Db.Transact(() =>
                {
                    int index = 0;
                    DbLinq.Objects<Database.Person>().OrderBy(x => x.Guid).ToList().ForEach(x => x.TableSortIndex = index++);
                });
            }
        }
    }
}
