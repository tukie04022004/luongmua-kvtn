using Microsoft.AspNetCore.Mvc;
using KttvKvtnWeb.Data;
using KttvKvtnWeb.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace KttvKvtnWeb.Controllers
{
    public class OverviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OverviewController(ApplicationDbContext context) => _context = context;

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            // Lấy dữ liệu lượng mưa mới nhất từng trạm
            var allRain = _context.CwtDomua.AsEnumerable()
                .GroupBy(x => x.Matram)
                .Select(g => g.OrderByDescending(x => x.DateTime).First())
                .ToList();

            // Biểu đồ cột
            var chartData = allRain.Select(x =>
            {
                var tram = _context.TramKttvtn.FirstOrDefault(t => t.Matram == x.Matram);
                return new
                {
                    matram = x.Matram,
                    tentram = tram?.Tentram ?? x.Matram,
                    luongmua = Math.Round(x.Luongmua, 1),
                    color = GetRainColor(x.Luongmua)
                };
            }).OrderByDescending(x => x.luongmua).ToList();

            ViewBag.ChartData = JsonConvert.SerializeObject(chartData);

            // Bản đồ OpenStreetMap: marker + popup + màu đồng bộ
            var stationsList = _context.TramKttvtn.AsEnumerable()
                .Select(t =>
                {
                    var parts = t.Vitri.Split(',').Select(p => p.Trim()).ToArray();
                    var lat = parts.Length > 0 ? parts[0] : "10.762";
                    var lng = parts.Length > 1 ? parts[1] : "106.660";

                    var rain = allRain.FirstOrDefault(r => r.Matram == t.Matram);

                    return new
                    {
                        tentram = t.Tentram,
                        diachi = t.Diachi,
                        lat,
                        lng,
                        luongmua = rain?.Luongmua ?? 0,
                        thoigian = rain?.Thoigian ?? "Chưa có dữ liệu",
                        color = GetRainColor(rain?.Luongmua ?? 0)
                    };
                })
                .ToList();

            ViewBag.Stations = JsonConvert.SerializeObject(stationsList);

            return View();
        }

        private string GetRainColor(double rain)
        {
            if (rain <= 0) return "#2F2F2F";    // Không mưa - đen khói
            if (rain <= 10) return "#87CEFA";    // Mưa nhỏ - xanh biển nhạt
            if (rain <= 50) return "#32CD32";    // Mưa vừa - xanh lá
            if (rain <= 100) return "#FFA500";   // Mưa to - cam
            return "#FF0000";                     // Mưa rất to - đỏ
        }
    }
}
