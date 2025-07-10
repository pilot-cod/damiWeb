// Pages/BillPage.cshtml.cs
using damiWeb.Data;
using damiWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace damiWeb.Pages;

public class BillPageModel : PageModel
{
    private readonly AppDbContext _db;
    public BillPageModel(AppDbContext db) => _db = db;

    // ==== Thuộc tính bind cho Razor ====
    [BindProperty] public string OrderNo { get; set; } = string.Empty;
    [BindProperty] public DateTime OrderDate { get; set; } = DateTime.Today;

    [BindProperty] public string? CustomerID { get; set; }         // gõ mã KH
    [BindProperty] public string? CustomerName { get; set; }   // tự điền
    [BindProperty] public string? Address { get; set; }        // tự điền

    // ==== 1. Lấy OrderNo mới khi mở trang ====
    public async Task OnGetAsync()
    {
        var last = await _db.OrderMasters
                            .OrderByDescending(o => o.OrderNo)
                            .Select(o => o.OrderNo)
                            .FirstOrDefaultAsync();

        int next = int.TryParse(last, out int cur) ? cur + 1 : 1;
        OrderNo = next.ToString("D3");
    }

    // ==== 2. Ajax tra khách hàng ====
    public async Task<JsonResult> OnGetCustomer(string id)
    {
        var c = await _db.Customers
                         .Where(c => c.CustomerID == id)
                         .Select(c => new { c.CustomerName, c.Address })
                         .FirstOrDefaultAsync();

        return new JsonResult(c);
    }

    // ==== 3. Lưu hóa đơn (nút Ghi) ====
    public async Task<IActionResult> OnPostSaveAsync()
    {
        var master = new OrderMaster
        {
            OrderDate = OrderDate,
            OrderNo = OrderNo,
            CustomerID = CustomerID,
            TotalAmount = 0m
        };
        _db.OrderMasters.Add(master);
        await _db.SaveChangesAsync();

        TempData["msg"] = "Đã lưu hóa đơn!";
        return RedirectToPage();
    }

}
