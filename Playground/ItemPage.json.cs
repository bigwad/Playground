using Starcounter;

namespace Playground
{
    partial class ItemPage : Json
    {
        protected void Handle(Input.SaveTrigger action)
        {
            action.Cancel();
            this.AttachedScope.Commit();
        }

        [ItemPage_json.Item]
        partial class ItemPageItem : Json, IBound<Database.Item>
        {
            public string ObjectId
            {
                get
                {
                    return this.Data?.GetObjectID();
                }
            }
        }
    }
}
