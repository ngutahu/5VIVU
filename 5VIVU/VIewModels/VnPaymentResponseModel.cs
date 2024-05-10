namespace _5VIVU.VIewModels
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
    public class VnPayMentRequestModel
    {
        public string fullName { get; set; }
        public string description {  get; set; }    
        public decimal amount {  get; set; }
        public DateTime createDate { get; set; }
        public int ticketID {  get; set; }
    }
}
