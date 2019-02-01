using System;
using Starcounter;

namespace Database
{
    [Database]
    public class Item
    {
        public string Guid { get; set; }
        public DateTime Date { get; set; }
        public int Thread { get; set; }
        public int Index { get; set; }
    }
}
