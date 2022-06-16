using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NanoPingPong.Shared.Config;
using QRCoder;

namespace NanoPingPong.Web.Pages
{
    public class QRModel : PageModel
    {

        private IContext Context { get; }

        public QRModel(IContext context)
        {
            Context = context;
        }
        public IActionResult OnGetAddress()
        {
            return GenerateQRCode(Context.Link);
        }

        public IActionResult OnGetDonations()
        {
            return GenerateQRCode(Context.DonationLink, true);
        }

        private FileContentResult GenerateQRCode(string value, bool small = false)
        {
            var data = QRCodeGenerator.GenerateQrCode(value, QRCodeGenerator.ECCLevel.Q);
            var code = new PngByteQRCode(data);
            var qr = code.GetGraphic(small ? 2 : 3);
            return File(qr, "image/png");
        }

    }
}
