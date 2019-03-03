using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using KitchenStatusServer.Models;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Android.Content;
using Newtonsoft.Json;

namespace KitchenStatusClient
{
    [Activity(Theme = "@style/MyTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        static ListView listView;
        static ProductListAdapter adapter;
        static Product[] items;
        public static HttpClient httpClient;

        public enum RequestType { GetAllProducts, PostNewStatusUpdate };

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            SupportActionBar.Title = "Kitchen Status";

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://192.168.0.110/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Get all products from /api/products URI
            await RunAsync(RequestType.GetAllProducts);
            
            adapter = new ProductListAdapter(this, items);
            listView = FindViewById<ListView>(Resource.Id.listView);
            listView.Adapter = adapter;
            listView.ItemClick += OnListItemClick;
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var product = items[e.Position];
            Toast.MakeText(this, $"Editing: {product.Name}", ToastLength.Short).Show();

            Intent intent = new Intent(this, typeof(EditStatusActivity));
            intent.PutExtra("product", JsonConvert.SerializeObject(product));
            //StartActivity(intent);
            StartActivityForResult(intent, 100);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted, ToastLength.Short).Show();

            return base.OnOptionsItemSelected(item);
        }

        public static async Task RunAsync(RequestType requestType, StatusUpdate statusUpdate = null)
        {
            switch(requestType)
            {
                case RequestType.GetAllProducts:
                    items = await GetProductsAsync("api/products");
                    break;
                case RequestType.PostNewStatusUpdate:
                    await CreateStatusUpdate("api/statusupdates", statusUpdate);
                    break;
            }
        }

        static async Task<Product[]> GetProductsAsync(string path)
        {
            Product[] products = null;
            // HTTP GET whole product list
            HttpResponseMessage response = await httpClient.GetAsync(path).ConfigureAwait(false);
            if(response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<Product[]>();
            }

            return products;
        }

        static async Task<Uri> CreateStatusUpdate(string path, StatusUpdate statusUpdate)
        {
            // HTTP POST the product status update
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(path, statusUpdate);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource
            return response.Headers.Location;
        }

        public static string CapitalizeString(string s)
        {
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Toast.MakeText(this, "Returned from editing", ToastLength.Short).Show();

            if(requestCode == 100 && resultCode == Result.Ok)
            {
                // Refresh the list
                await RunAsync(RequestType.GetAllProducts);

                Toast.MakeText(this, "List fetched", ToastLength.Short).Show();

                adapter = new ProductListAdapter(this, items);
                listView.Adapter = adapter;
            }
        }
	}
}

