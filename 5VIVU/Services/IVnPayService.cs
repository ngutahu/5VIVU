using _5VIVU.Data;
using _5VIVU.VIewModels;
namespace _5VIVU.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPayMentRequestModel model);        
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}
