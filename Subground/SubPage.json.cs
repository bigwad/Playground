using System;
using Starcounter;

namespace Subground {
    partial class SubPage : Json {
        protected override void OnData() {
            base.OnData();

            this.Date = DateTime.Now.ToString();
        }
    }
}
