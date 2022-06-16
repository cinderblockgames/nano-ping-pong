using Microsoft.AspNetCore.Mvc.RazorPages;
using NanoPingPong.Shared.Config;

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

    }
}
