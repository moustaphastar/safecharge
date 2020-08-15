using UI.Helpers;

namespace UI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl1 { get; set; }
        public string ImageUrl2 { get; set; }
        //[System.Text.Json.Serialization.JsonConverter(typeof(DecimalToStringJsonConverter))]
        public decimal Price { get; set; }
    }
}