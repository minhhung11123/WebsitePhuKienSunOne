using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Controllers
{
    public class BlogController : Controller
    {
        private readonly dbSunOneContext _context;

        public BlogController(dbSunOneContext context)
        {
            _context = context;
        }
        [Route("news.html", Name="News")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsBlog = _context.News
                        .AsNoTracking()
                        .OrderByDescending(x => x.PostId);

            PagedList<News> models = new PagedList<News>(lsBlog, pageNumber, pageSize);

            var lsNews = _context.News.AsNoTracking().OrderByDescending(x => x.CreateDate).Take(4).ToList();

            ViewBag.LsNews = lsNews;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPage = models.PageCount;
            return View(models);
        }

        [Route("/news/{Alias}-{id}.html", Name = "NewsDetails")]
        public async Task<IActionResult> Details(int id)
        {
            var news = _context.News.AsNoTracking().SingleOrDefault(x => x.PostId == id);
            if (news == null)
            {
                return RedirectToAction("Index");
            }
            var lsNews = _context.News
                .AsNoTracking()
                .Where(x => x.Published == true && x.PostId != id)
                .Take(3)
                .OrderByDescending(x => x.CreateDate)
                .ToList();
            ViewBag.BaiVietLienQuan = lsNews;
            news.Views += 1;
            _context.Update(news);
            await _context.SaveChangesAsync();
            return View(news);
        }
    }
}
