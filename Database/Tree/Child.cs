using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace Database.Tree
{
    [Database]
    public class Child : Parent
    {
        public string Value { get; set; }
        public DateTime Date { get; set; }
        public Sibling ChildSibling { get; set; }
    }
}
