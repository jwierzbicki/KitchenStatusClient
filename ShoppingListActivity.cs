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
using KitchenStatusServer.Models;

namespace KitchenStatusClient
{
    [Activity(Theme = "@style/MyTheme")]
    public class ShoppingListActivity : Activity
    {
        ShoppingItem[] items;
        ListView listView;
        ShoppingListAdapter adapter;
        Button button;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.shopping_list_layout);

            button = FindViewById<Button>(Resource.Id.end_shopping_button);
            listView = FindViewById<ListView>(Resource.Id.shopping_list_view);
            
            items = await MainActivity.RunAsync<ShoppingItem[]>(MainActivity.RequestType.GenerateShoppingList);

            adapter = new ShoppingListAdapter(this, items);
            listView.Adapter = adapter;

            button.Click += EndShoppingAsync;
        }

        private async void EndShoppingAsync(object sender, EventArgs e)
        {
            var changes = new List<StatusChange>();
            foreach (var item in items)
            {
                var invertedAmount = -1 * item.Amount;
                changes.Add(new StatusChange { Name = item.Name, Quantity = invertedAmount });
            }
            var statusUpdate = new StatusUpdate { User = "anon_shopper", Changes = changes.ToArray() };

            await MainActivity.RunAsync<Uri>(MainActivity.RequestType.PostNewStatusUpdate, statusUpdate);

            Toast.MakeText(this, "Send update after shopping", ToastLength.Short).Show();

            var intent = new Intent();
            SetResult(Result.Ok, intent);
            // Close activity - return to main list
            Finish();
        }
    }
}