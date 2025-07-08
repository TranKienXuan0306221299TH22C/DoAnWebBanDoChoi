using DoAnWebBanDoChoi.Models.VNpay;

namespace DoAnWebBanDoChoi.Services.VNpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
