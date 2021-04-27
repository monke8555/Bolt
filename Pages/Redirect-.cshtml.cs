using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Bolt.Pages
{
    public class RedirectModel : PageModel
    {
        private readonly ILogger<RedirectModel> _logger;

        public RedirectModel(ILogger<RedirectModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
