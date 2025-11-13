using Microsoft.AspNetCore.Mvc;
using KttvKvtnWeb.Data;
using KttvKvtnWeb.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Globalization;

namespace KttvKvtnWeb.Controllers
{
    public class DetailController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DetailController(ApplicationDbContext context) => _context = context;

        public IActionResult Index(string matram, string from, string to)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            // Lấy danh sách trạm
            var trams = _context.TramKttvtn.OrderBy(t => t.Matram).ToList();
            ViewBag.Trams = trams;

            // Nếu chưa chọn trạm thì chọn trạm đầu tiên
            if (string.IsNullOrEmpty(matram) && trams.Any())
                matram = trams.First().Matram;

            ViewBag.MaTram = matram;

            // Lấy tên trạm
            var tentram = _context.TramKttvtn.FirstOrDefault(t => t.Matram == matram)?.Tentram ?? matram;
            ViewBag.TenTram = tentram;

            // Xử lý ngày
            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (!string.IsNullOrEmpty(from) && DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime f))
                fromDate = f;
            if (!string.IsNullOrEmpty(to) && DateTime.TryParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                toDate = t;

            // Mặc định 30 ngày gần nhất
            if (!fromDate.HasValue) fromDate = DateTime.Today.AddDays(-30);
            if (!toDate.HasValue) toDate = DateTime.Today;

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            // Lấy dữ liệu theo trạm + ngày
            if (!string.IsNullOrEmpty(matram))
            {
                var data = _context.CwtDomua
                    .Where(x => x.Matram == matram)
                    .AsEnumerable() // tránh lỗi khi x.Thoigian là string
                    .Where(x => DateTime.TryParse(x.Thoigian, out DateTime tg)
                                && tg >= fromDate && tg <= toDate)
                    .OrderByDescending(x => DateTime.Parse(x.Thoigian))
                    .ToList();

                ViewBag.Data = data;
            }

            return View();
        }

        public IActionResult ExportExcel(string matram, string from, string to)
        {
            if (string.IsNullOrEmpty(matram)) return NotFound();

            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (!string.IsNullOrEmpty(from) && DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime f))
                fromDate = f;
            if (!string.IsNullOrEmpty(to) && DateTime.TryParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                toDate = t;

            if (!fromDate.HasValue) fromDate = DateTime.Today.AddDays(-30);
            if (!toDate.HasValue) toDate = DateTime.Today;

            var data = _context.CwtDomua
                .Where(x => x.Matram == matram)
                .AsEnumerable()
                .Where(x => DateTime.TryParse(x.Thoigian, out DateTime tg)
                            && tg >= fromDate && tg <= toDate)
                .OrderByDescending(x => DateTime.Parse(x.Thoigian))
                .ToList();

            var tram = _context.TramKttvtn.FirstOrDefault(t => t.Matram == matram);
            string tenTram = tram?.Tentram ?? matram;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("LuongMua");

            // Tiêu đề báo cáo
            ws.Cells[1, 1].Value = "BÁO CÁO LƯỢNG MƯA CHI TIẾT";
            ws.Cells[1, 1, 1, 7].Merge = true;
            ws.Cells[1, 1].Style.Font.Size = 16;
            ws.Cells[1, 1].Style.Font.Bold = true;

            ws.Cells[2, 1].Value = $"Trạm: {matram} - {tenTram}";
            ws.Cells[2, 1, 2, 7].Merge = true;

            ws.Cells[3, 1].Value = $"Thời gian: {fromDate:dd/MM/yyyy} → {toDate:dd/MM/yyyy}";
            ws.Cells[3, 1, 3, 7].Merge = true;

            ws.Cells[4, 1].Value = $"In lúc: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cells[4, 1, 4, 7].Merge = true;

            // Header bảng
            string[] headers = { "STT", "Thời gian", "Mã trạm", "Lượng mưa (mm)", "PE", "Lưu lượng", "Cấp mưa" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[6, i + 1].Value = headers[i];
                ws.Cells[6, i + 1].Style.Font.Bold = true;
                ws.Cells[6, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[6, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            // Dữ liệu
            int row = 7;
            int stt = 1;
            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = stt++;
                ws.Cells[row, 2].Value = item.Thoigian;
                ws.Cells[row, 3].Value = item.Matram;
                ws.Cells[row, 4].Value = item.Luongmua;
                ws.Cells[row, 5].Value = item.Pe;
                ws.Cells[row, 6].Value = item.Luuluong;

                string capMua = item.Luongmua > 200 ? "Mưa bão" :
                                item.Luongmua > 100 ? "Mưa to" :
                                item.Luongmua > 50 ? "Mưa vừa" : "Mưa nhỏ";
                ws.Cells[row, 7].Value = capMua;

                row++;
            }

            // AutoFit + Border
            ws.Cells[1, 1, row - 1, 7].AutoFitColumns();
            ws.Cells[6, 1, row - 1, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            var fileBytes = package.GetAsByteArray();
            string fileName = $"LuongMua_{matram}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
