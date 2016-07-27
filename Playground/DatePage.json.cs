using System;
using Starcounter;

namespace Playground
{
    partial class DatePage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            this.Date = DateTime.Now.ToString();
        }
    }
}
