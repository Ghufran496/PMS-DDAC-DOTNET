using PMS_DDAC.Models;

namespace PMS_DDAC.Models.Admin
{
    public class AdminDashboardViewModel
    {
        public int PaidBillsCount { get; set; }
        public int UnpaidBillsCount { get; set; }
        public decimal PaidIncome { get; set; }
        public decimal UnpaidIncome { get; set; }
        public decimal TotalWaterBill { get; set; }
        public decimal TotalElectricityBill { get; set; }
        public List<ServiceModel> UpcomingServices { get; set; } = new();
    }
}
