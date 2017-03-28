using System;
using Starcounter;

namespace Playground.Database
{
    [Database]
    public class Person
    {
        public int Priority { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
