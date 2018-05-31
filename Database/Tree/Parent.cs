using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace Database.Tree
{
    [Database]
    public class Parent
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public Sibling ParentSibling { get; set; }
    }
}
