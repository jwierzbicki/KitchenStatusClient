namespace KitchenStatusServer.Models
{
    public class Product
    {
        public Product()
        {
        }
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public double StateCurrent { get; set; }
        
        public double StateMinimal { get; set; }
        
        public double UnitQuantity { get; set; }
        
        public string UnitType { get; set; }
    }
}
