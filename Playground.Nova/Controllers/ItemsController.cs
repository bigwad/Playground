using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starcounter.Nova;
using Playground.Nova.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Playground.Nova.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        // GET api/<controller>
        [HttpGet]
        public IEnumerable<ItemProxy> Get()
        {
            ItemProxy[] items = null;

            Db.TransactAsync(() =>
            {
                items = items = Db.SQL<Item>("SELECT i FROM Playground.Nova.Database.Item i ORDER BY i.\"Date\"").Select(x => new ItemProxy(x)).ToArray();
            });

            return items;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public ItemProxy Get(ulong id)
        {
            ItemProxy proxy = null;

            Db.TransactAsync(() =>
            {
                Item item = Db.Get<Item>(id);

                if (item == null)
                {
                    proxy = new ItemProxy();
                }
                else
                {
                    proxy = new ItemProxy(item);
                }
            });

            return proxy;
        }

        // POST api/<controller>
        [HttpPost]
        public ItemProxy Post([FromBody]ItemProxy value)
        {
            if (value.Guid == null)
            {
                value.Guid = Guid.NewGuid().ToString();
            }

            value.Date = DateTime.Now;
            value.Insert();

            return value;
        }

        // PUT api/<controller>
        [HttpPut]
        public ItemProxy Put([FromBody]ItemProxy value)
        {
            value.Update();
            return value;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public bool Delete(ulong id)
        {
            bool result = false;

            Db.TransactAsync(() =>
            {
                Item item = Db.Get<Item>(id);

                if (item != null)
                {
                    result = true;
                    Db.Delete(item);
                }
            });

            return result;
        }
    }
}
