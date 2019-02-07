using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Playground.Nova.Models
{
    public class ItemProxy
    {
        public long Id { get; set; }
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
            this.Id = source.Id;
            this.Guid = source.Guid;
            this.Date = source.Date;
            this.Thread = source.Thread;
            this.Index = source.Index;
        }

        public async Task UpdateAsync(PlaygroundContext db)
        {
            Item item = db.Items.FirstOrDefault(x => x.Id == this.Id);

            if (item == null)
            {
                throw new InvalidOperationException($"Item with Id {this.Id} not found.");
            }

            item.Guid = this.Guid;
            item.Date = this.Date;
            item.Thread = this.Thread;
            item.Index = this.Index;

            await db.SaveChangesAsync();
        }

        public async Task InsertAsync(PlaygroundContext db)
        {
            Item item = new Item();

            item.Guid = this.Guid;
            item.Date = this.Date;
            item.Thread = this.Thread;
            item.Index = this.Index;

            db.Items.Add(item);
            await db.SaveChangesAsync();

            this.Id = item.Id;
        }

        public async Task DeleteAsync(PlaygroundContext db)
        {
            Item item = db.Items.FirstOrDefault(x => x.Id == this.Id);

            if (item == null)
            {
                throw new InvalidOperationException($"Item with Id {this.Id} not found.");
            }

            db.Items.Remove(item);
            await db.SaveChangesAsync();
        }
    }
}
