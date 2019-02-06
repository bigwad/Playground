using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Starcounter.Nova;

namespace Playground.Nova.Database
{
    public class ItemProxy
    {
        public ulong Oid { get; set; }
        public string Guid { get; set; }
        public DateTime Date { get; set; }
        public int Thread { get; set; }
        public int Index { get; set; }
        public string Notes { get; set; }

        public string DateStr
        {
            get
            {
                return this.Date.ToString();
            }
            set
            {
                DateTime date;

                if (DateTime.TryParse(value, out date))
                {
                    this.Date = date;
                }
            }
        }

        public ItemProxy()
        {
        }

        public ItemProxy(Item source)
        {
            this.Oid = Db.GetOid(source);
            this.Guid = source.Guid;
            this.Date = source.Date;
            this.Thread = source.Thread;
            this.Index = source.Index;
        }

        public void Update()
        {
            Db.TransactAsync(() =>
            {
                Item item = Db.Get<Item>(this.Oid);

                item.Guid = this.Guid;
                item.Date = this.Date;
                item.Thread = this.Thread;
                item.Index = this.Index;
            });
        }

        public void Insert()
        {
            Db.TransactAsync(() =>
            {
                Item item = Db.Insert<Item>();

                item.Guid = this.Guid;
                item.Date = this.Date;
                item.Thread = this.Thread;
                item.Index = this.Index;

                this.Oid = Db.GetOid(item);
            });
        }

        public void Delete()
        {
            Db.TransactAsync(() =>
            {
                Item item = Db.Get<Item>(this.Oid);
                Db.Delete(item);
            });
        }
    }
}
