using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NanoPingPong.Shared.Config;
using QRCoder;

namespace NanoPingPong.Web.Pages
{
    public class IndexModel : PageModel
    {

        public IContext Context { get; }

        public IndexModel(IContext context)
        {
            Context = context;
        }

        public void OnGet()
        {
            // Nothing additional.
        }

        public IActionResult OnGetAddress()
        {
            // Default to 0.01 XNO or 0.1 BAN for ease of use.
            return GenerateQRCode($"{Context.LinkPrefix}{Context.Address}?amount=10000000000000000000000000000");
        }

        public IActionResult OnGetDonations()
        {
            return GenerateQRCode($"{Context.LinkPrefix}{Context.DonationAddress}");
        }

        private FileContentResult GenerateQRCode(string value)
        {
            var data = QRCodeGenerator.GenerateQrCode(value, QRCodeGenerator.ECCLevel.Q);
            var code = new PngByteQRCode(data);
            var qr = code.GetGraphic(
                (int)Math.Ceiling((decimal)150 / (decimal)data.ModuleMatrix.Count) + 1,
                drawQuietZones: false);
            return File(qr, "image/png");
        }

    }
}
