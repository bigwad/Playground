using System;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Linq;

namespace Playground
{
    partial class IndexPage : Json
    {
        public IEnumerable<Database.Item> Items
        {
            get
            {
                return DbLinq.Objects<Database.Item>().OrderBy(x => x.Index);
            }
        }
    }
}
