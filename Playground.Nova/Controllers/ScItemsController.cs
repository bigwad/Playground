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
    public class ScItemsController : Controller
    {
        // GET api/<controller>
        [HttpGet]
        public async Task<IEnumerable<ItemProxy>> Get()
        {
            ItemProxy[] items = null;

            await Db.TransactAsync(() =>
            {
                items = items = Db.SQL<Item>("SELECT i FROM Playground.Nova.Database.Item i ORDER BY i.\"Date\"").Select(x => new ItemProxy(x)).ToArray();
            });

            return items;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<ItemProxy> Get(ulong id)
        {
            ItemProxy proxy = null;

            await Db.TransactAsync(() =>
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
        public async Task<ItemProxy> Post([FromBody]ItemProxy value)
        {
            if (value.Guid == null)
            {
                value.Guid = Guid.NewGuid().ToString();
            }

            value.Date = DateTime.Now;
            await value.InsertAsync();

            return value;
        }

        // PUT api/<controller>
        [HttpPut]
        public async Task<ItemProxy> Put([FromBody]ItemProxy value)
        {
            await value.UpdateAsync();
            return value;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(ulong id)
        {
            bool result = false;

            await Db.TransactAsync(() =>
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
