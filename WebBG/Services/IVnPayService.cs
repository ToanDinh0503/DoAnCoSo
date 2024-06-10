using WebBG.ViewModels;

namespace WebBG.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
