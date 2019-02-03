using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Playground.Mvc.Models;

namespace Playground.Mvc.Controllers
{
    public class ItemsController : ApiController
    {
        protected PlaygroundEntities db = new PlaygroundEntities();

        // GET api/<controller>
        public IEnumerable<ItemProxy> Get()
        {
            IEnumerable<ItemProxy> items = db.Items.OrderBy(x => x.Date).ToArray().Select(x => new ItemProxy(x));
            return items;
        }

        // GET api/<controller>/5
        public ItemProxy Get(int id)
        {
            Item item = db.Items.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return new ItemProxy();
            }

            ItemProxy proxy = new ItemProxy(item);

            return proxy;
        }

        // POST api/<controller>
        public ItemProxy Post([FromBody]ItemProxy value)
        {
            if (value.Guid == null)
            {
                value.Guid = Guid.NewGuid();
            }

            value.Date = DateTime.Now;
            value.Insert(db);

            return value;
        }

        // PUT api/<controller>
        public ItemProxy Put([FromBody]ItemProxy value)
        {
            value.Update(db);
            return value;
        }

        // DELETE api/<controller>/5
        public bool Delete(int id)
        {
            Item item = db.Items.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return false;
            }

            ItemProxy proxy = new ItemProxy(item);
            proxy.Delete(db);

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}