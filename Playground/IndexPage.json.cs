using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Starcounter.Advanced;

namespace Playground
{
    partial class IndexPage : Json
    {
        protected List<DataItem> dataItems;

        protected override void OnData()
        {
            base.OnData();

            //Db.Transact(() =>
            //{
            //    Db.SlowSQL("DELETE FROM Playground.Database.Person");

            //    new Database.Person() { Priority = 0, Name = "John", Age = 15 };
            //    new Database.Person() { Priority = 1, Name = "Peter", Age = 20 };
            //    new Database.Person() { Priority = 2, Name = "Susan", Age = 25 };
            //    new Database.Person() { Priority = 3, Name = "Kelly", Age = 30 };
            //    new Database.Person() { Priority = 4, Name = "Sherlock", Age = 35 };
            //});

            this.dataItems = new List<DataItem>()
            {
                new DataItem() { Id = 0, Priority = 0, Name = "John", Age = 15 },
                new DataItem() { Id = 1, Priority = 1, Name = "Peter", Age = 20 },
                new DataItem() { Id = 2, Priority = 2, Name = "Susan", Age = 25 },
                new DataItem() { Id = 3, Priority = 3, Name = "Kelly", Age = 30 },
                new DataItem() { Id = 4, Priority = 4, Name = "Sherlock", Age = 35 }
            };
        }

        public IEnumerable<DataItem> Items
        {
            get
            {
                //return Db.SQL<Database.Person>("SELECT p FROM Playground.Database.Person p ORDER BY p.Priority");
                return this.dataItems;
            }
        }

        protected void Handle(Input.SwipeRowsTrigger action)
        {
            action.Cancel();

            //List<Database.Person> items = this.Items.ToList();
            //int p = items[0].Priority;

            //Db.Transact(() =>
            //{
            //    //items.ForEach(x => x.Age++);

            //    items[0].Age++;
            //    items[1].Age++;
            //    items[0].Name += "!";
            //    items[1].Name += "!";

            //    items[0].Priority = items[1].Priority;
            //    items[1].Priority = p;
            //});

            List<DataItem> newItems = this.Items.ToList();
            DataItem item = newItems.FirstOrDefault(x => x.Id == 0);

            if (newItems.IndexOf(item) == item.Priority)
            {
                item.Name = "John White";
                newItems.Remove(item);
                newItems.Insert(1, item);
            }
            else
            {
                item.Name = "John";
                newItems.Remove(item);
                newItems.Insert(0, item);
            }
            
            this.dataItems = newItems;

            //DataItem item = this.dataItems[1];

            //this.dataItems.Remove(item);
            //this.dataItems.Insert(0, item);

            //this.dataItems.ForEach(x => x.Age++);
        }
    }
}
