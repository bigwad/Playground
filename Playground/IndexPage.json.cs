using Starcounter;

namespace Playground {
    partial class IndexPage : Page {
        void Handle(Input.Button Action) {
            this.Message = string.Format("Button was clicked {0} time(s)!", Action.Value);
        }

        void Handle(Input.Text Action) {
            this.Message = string.Format("Text was changed to '{0}'!", Action.Value);
        }
    }
}
