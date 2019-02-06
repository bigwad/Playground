using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Starcounter.Nova;

namespace Playground.Nova.Database
{
    [Database]
    public abstract class Item
    {
        public virtual string Guid { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual int Thread { get; set; }
        public virtual int Index { get; set; }
    }
}
