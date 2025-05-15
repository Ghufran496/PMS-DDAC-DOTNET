namespace PMS_DDAC.Models
{
    public class MonthlyBillReportViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");
        public decimal TotalAmount { get; set; }
        public List<BillModel> Bills { get; set; } = new List<BillModel>();
    }
}