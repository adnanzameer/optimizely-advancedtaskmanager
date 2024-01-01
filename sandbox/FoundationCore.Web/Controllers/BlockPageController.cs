using FoundationCore.Web.Models.Pages;
using FoundationCore.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoundationCore.Web.Controllers;

public class BlockPageController : PageControllerBase<BlockPage>
{
    public IActionResult Index(BlockPage currentPage)
    {
        var model = PageViewModel.Create(currentPage);

        return View(model);
    }
}