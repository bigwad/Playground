using System;
using Starcounter;

namespace Subground {
    partial class SubPage : Page {
        protected override void OnData() {
            base.OnData();

            this.Date = DateTime.Now.ToString();
        }
    }
}
