using System;
using Starcounter.Advanced;

namespace Playground
{
    public class DataItem : IBindable
    {
        public ulong Id { get; set; }
        public int Priority { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public DataItem()
        {
        }

        public DataItem(DataItem source)
        {
            this.Id = source.Id;
            this.Priority = source.Priority;
            this.Name = source.Name;
            this.Age = source.Age;
        }

        public ulong Identity => this.Id;

        public IBindableRetriever Retriever => throw new NotImplementedException();
    }
}
