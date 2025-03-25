using DACS_WebClothes.Models;
namespace DACS_WebClothes.ViewModels
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }

    public class VnPaymentRequestModel
    {
        //ma don hang
        public int OrderID { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        //so tien
        public double Amount { get; set; }
        //ngay tao ra requsest
        public DateTime CreatedDate { get; set; }
    }
}
