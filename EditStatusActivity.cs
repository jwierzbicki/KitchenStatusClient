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
using KitchenStatusServer.Models;
using Newtonsoft.Json;

namespace KitchenStatusClient
{
    [Activity(Label = "EditStatusActivity")]
    public class EditStatusActivity : Activity
    {
        Product product;
        TextView productNameTextView;
        EditText statusChangeEditText;
        Button saveChangesButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.edit_status_layout);

            product = JsonConvert.DeserializeObject<Product>(Intent.GetStringExtra("product"));

            productNameTextView = FindViewById<TextView>(Resource.Id.name_textview);
            statusChangeEditText = FindViewById<EditText>(Resource.Id.statuschange_edittext);
            saveChangesButton = FindViewById<Button>(Resource.Id.savechanges_button);

            productNameTextView.Text = product.Name;
            statusChangeEditText.Text = product.StateCurrent.ToString();

            saveChangesButton.Click += SaveChangesActionAsync;
        }

        private async void SaveChangesActionAsync(object sender, EventArgs e)
        {
            var newValue = double.Parse(statusChangeEditText.Text);
            
            var changes = new StatusChange[] { new StatusChange { Name = product.Name, Quantity = newValue } };
            var statusUpdate = new StatusUpdate { User = "anon", Changes = changes };

            Toast.MakeText(this, $"Saving changes with value: {newValue}", ToastLength.Short).Show();
            await MainActivity.RunAsync(MainActivity.RequestType.PostNewStatusUpdate, statusUpdate);
            Toast.MakeText(this, "Changes saved", ToastLength.Short).Show();
        }
    }
}