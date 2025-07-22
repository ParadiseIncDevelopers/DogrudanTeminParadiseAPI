using System.Collections.Generic;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProductItemTypeSpendingReportDto
    {
        public List<ProductItemTypeSpendingDto> Weekly { get; set; }
        public List<ProductItemTypeSpendingDto> Monthly { get; set; }
        public List<ProductItemTypeSpendingDto> Quarterly { get; set; }
        public List<ProductItemTypeSpendingDto> Yearly { get; set; }
    }
}
