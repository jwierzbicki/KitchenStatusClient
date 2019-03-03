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
    [Activity(Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ListView listView;
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
            
            listView = FindViewById<ListView>(Resource.Id.listView);
            listView.Adapter = new ProductListAdapter(this, items);
            listView.ItemClick += OnListItemClick;
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var product = items[e.Position];
            Toast.MakeText(this, $"Editing: {product.Name}", ToastLength.Short).Show();

            Intent intent = new Intent(this, typeof(EditStatusActivity));
            intent.PutExtra("product", JsonConvert.SerializeObject(product));
            StartActivity(intent);
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
            HttpResponseMessage response = await httpClient.GetAsync(path).ConfigureAwait(false);
            if(response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<Product[]>();
            }

            return products;
        }

        static async Task<Uri> CreateStatusUpdate(string path, StatusUpdate statusUpdate)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(path, statusUpdate);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource
            return response.Headers.Location;
        }
	}
}

