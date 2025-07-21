using System.Collections.Generic;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProductQuantityReportDto
    {
        public List<ProductQuantityExtremeDto> Weekly { get; set; }
        public List<ProductQuantityExtremeDto> Monthly { get; set; }
        public List<ProductQuantityExtremeDto> Quarterly { get; set; }
        public List<ProductQuantityExtremeDto> Yearly { get; set; }
    }
}
