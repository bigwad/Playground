using System;
using Starcounter;

namespace Database
{
    [Database]
    public class PersonsTable
    {
        public string SessionId { get; set; }
        public Person Item { get; set; }
        public int TableSortIndex { get; set; }
        public bool TableHidden { get; set; }
        public bool TableHiddenFromAllAppa { get; set; }
    }

    [Database]
    public class Person
    {
        public string Guid { get; set; }
        public DateTime Date { get; set; }
        public int Thread { get; set; }
        public int Index { get; set; }

        public int TableSortIndex { get; set; }
        public bool TableHidden { get; set; }
    }
}
