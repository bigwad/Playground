using Starcounter;

namespace Subground
{
    partial class TableRowItemJson : Json, IBound<Database.Person>
    {
        public TableRowItemJson(Database.Person item)
        {
            this.Data = item;
        }

        public TableRowItemJson(string objectID)
        {
            this.Data = Db.FromId<Database.Person>(objectID);
        }

        public string ObjectId => this.Data?.GetObjectID();
        public string Date => this.Data?.Date.ToString();
    }
}
