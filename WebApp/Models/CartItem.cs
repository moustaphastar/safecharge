using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    [Serializable]
    public class CartItem : Product
    {
        public string Quantity { get; set; }
        public string Total { get; set; }
    }
}
