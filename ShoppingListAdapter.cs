using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KitchenStatusClient.Models;

namespace KitchenStatusClient
{
    class ShoppingListAdapter : BaseAdapter<ShoppingItem>
    {
        ShoppingItem[] items;
        Activity context;

        public ShoppingListAdapter(Activity context, ShoppingItem[] items) : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override ShoppingItem this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Length; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.shopping_list_item, null);

            view.FindViewById<TextView>(Resource.Id.name_shopping_textview).Text = MainActivity.CapitalizeString(item.Name);
            view.FindViewById<TextView>(Resource.Id.quantity_shopping_textview).Text = item.Amount.ToString();
            return view;
        }
    }
}