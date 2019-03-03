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
    [Activity(Theme = "@style/MyTheme")]
    public class EditStatusActivity : Activity
    {
        Product product;
        TextView statusCurrentTextView;
        TextView productNameTextView;
        EditText statusChangeEditText;
        Button saveChangesButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.edit_status_layout);

            product = JsonConvert.DeserializeObject<Product>(Intent.GetStringExtra("product"));

            productNameTextView = FindViewById<TextView>(Resource.Id.name_textview);
            statusCurrentTextView = FindViewById<TextView>(Resource.Id.statuscurrent_textview);
            statusChangeEditText = FindViewById<EditText>(Resource.Id.statuschange_edittext);
            saveChangesButton = FindViewById<Button>(Resource.Id.savechanges_button);

            // Set textview fields
            productNameTextView.Text = MainActivity.CapitalizeString(product.Name);
            statusCurrentTextView.Text = product.StateCurrent.ToString();
            statusChangeEditText.Text = (0.0).ToString();

            saveChangesButton.Click += SaveChangesActionAsync;
        }

        private async void SaveChangesActionAsync(object sender, EventArgs e)
        {
            var newValue = double.Parse(statusChangeEditText.Text);
            var changes = new StatusChange[] { new StatusChange { Name = product.Name, Quantity = newValue } };
            var statusUpdate = new StatusUpdate { User = "anon", Changes = changes };

            Toast.MakeText(this, $"Saving changes with value: {newValue}", ToastLength.Short).Show();
            // Call API
            await MainActivity.RunAsync(MainActivity.RequestType.PostNewStatusUpdate, statusUpdate);
            Toast.MakeText(this, "Changes saved", ToastLength.Short).Show();

            var intent = new Intent();
            SetResult(Result.Ok, intent);
            // Close activity - return to main list
            Finish();
        }


    }
}