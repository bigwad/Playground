using System;
using System.Linq;
using System.Collections.Generic;
using Starcounter;

namespace Playground {
    partial class RactivePage : Json {
        protected override void OnData() {
            base.OnData();

            List<object> products = new List<object>() {
                new { Name = "i7-6700k", Price = 320 },
                new { Name = "i7-4770k", Price = 280 }
            };

            this.Products.Data = products;
        }

        void Handle(Input.Click Action) {
            this.Message = string.Format("The button was pressed {0} time(s)", Action.Value);
        }

        void Handle(Input.Add Action) {
            this.Products.Add().Data = new { Name = "", Price = 0 };
        }

        [RactivePage_json.Products]
        partial class RactiveProductPage {
            void Handle(Input.Delete Action) {
                this.ParentPage.Products.Remove(this);
            }

            void Handle(Input.Select Action) {
                //this.ParentPage.Sub.Data = this.Data;
                this.ParentPage.Sub = Self.GET("/playground/ractive/" + string.Format("{0}.{1}", this.Name, this.Price));
            }

            RactivePage ParentPage {
                get {
                    return this.Parent.Parent as RactivePage;
                }
            }
        }
    }
}
