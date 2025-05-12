using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Service.DTOs
{
    public class BasketCalculationResultDTO
    {
        public List<BasketResultItemDTO> Items { get; set; } = new List<BasketResultItemDTO>();
        public decimal TotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
