using System;
using Starcounter;

namespace Playground
{
    partial class ItemPage : Json
    {
        public bool ItemExist => this.Item.Data != null;

        protected void Handle(Input.SaveTrigger action)
        {
            action.Cancel();
            this.AttachedScope.Commit();
        }

        protected void Handle(Input.CancelTrigger action)
        {
            action.Cancel();
            this.AttachedScope.Rollback();
        }

        [ItemPage_json.Item]
        partial class ItemPageItem : Json, IBound<Database.Item>
        {
            public string Id => this.Data?.GetObjectID();
            public ulong ObjectNo => this.Data?.GetObjectNo() ?? 0;

            public string DateStr
            {
                get
                {
                    return this.Data?.Date.ToString();
                }
                set
                {
                    if (this.Data == null)
                    {
                        return;
                    }

                    DateTime date;

                    if (DateTime.TryParse(value, out date))
                    {
                        this.Data.Date = date;
                    }
                }
            }
        }
    }
}
