using System;
using System.Collections.Generic;
using Starcounter;

namespace Playground {
    partial class DynItemsPage : Partial {
        protected override void OnData() {
            base.OnData();

            //Not supported due to https://github.com/Starcounter/Starcounter/issues/3686
            //Dictionary<string, object> item = new Dictionary<string, object>() { { "Name", "Item 3" }, { "Comments", "A dictionary item with comments" } };

            this.Items.Data = new object[] {
                new { Name = "Item 0" },
                new { Name = "Item 1", Color = "black" },
                new { Name = "Item 2", Color = "white", Comments = "An item with comments" },
                //item
            };
        }
    }
}
