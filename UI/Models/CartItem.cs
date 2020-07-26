using System;

namespace UI.Models
{
    [Serializable]
    public class CartItem : Product
    {
        public string Quantity { get; set; }
        public string Total { get; set; }
    }
}