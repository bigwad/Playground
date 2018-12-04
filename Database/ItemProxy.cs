﻿using System;
using Starcounter;

namespace Database
{
    public class ItemProxy
    {
        public string Id { get; set; }
        public ulong ObjectNo { get; set; }
        public string Guid { get; set; }
        public DateTime Date { get; set; }
        public int Thread { get; set; }
        public int Index { get; set; }
        public string Notes { get; set; }

        public ItemProxy()
        {
        }

        public ItemProxy(Person source)
        {
            this.Id = source.GetObjectID();
            this.ObjectNo = source.GetObjectNo();
            this.Guid = source.Guid;
            this.Date = source.Date;
            this.Thread = source.Thread;
            this.Index = source.Index;
        }

        public void Update()
        {
            Db.Transact(() =>
            {
                Person item = Db.FromId<Person>(this.ObjectNo);

                item.Guid = this.Guid;
                item.Date = this.Date;
                item.Thread = this.Thread;
                item.Index = this.Index;
            });
        }

        public void Insert()
        {
            Db.Transact(() =>
            {
                Person item = new Person();

                item.Guid = this.Guid;
                item.Date = this.Date;
                item.Thread = this.Thread;
                item.Index = this.Index;

                this.ObjectNo = item.GetObjectNo();
            });
        }

        public void Delete()
        {
            Db.Transact(() =>
            {
                Person item = Db.FromId<Person>(this.ObjectNo);
                item.Delete();
            });
        }
    }
}
