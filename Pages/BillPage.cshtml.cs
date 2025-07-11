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
    [BindProperty]
    public List<OrderItemDto> Items { get; set; } = new();

    // ==== 1. Lấy OrderNo mới khi mở trang ====
    public async Task OnGetAsync()
    {
        var last = await _db.OrderMasters
                            .OrderByDescending(o => o.OrderNo)
                            .Select(o => o.OrderNo)
                            .FirstOrDefaultAsync();

        int next = int.TryParse(last, out int cur) ? cur + 1 : 1;
        OrderNo = next.ToString("D3");
        Items.Add(new OrderItemDto());
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
            CustomerID = CustomerID!,
            TotalAmount = Items.Sum(x => x.Quantity * x.Price),
        };
        _db.OrderMasters.Add(master);
        await _db.SaveChangesAsync(); // Lưu để có OrderID

        int line = 1;
        foreach (var item in Items)
        {
            var detail = new OrderDetail
            {
                OrderID = master.OrderID,
                LineNumber = line++,
                ItemID = item.ItemID,
                Quantity = item.Quantity,
                Price = item.Price,
                Amount = item.Amount
            };
            _db.OrderDetails.Add(detail);
        }
        await _db.SaveChangesAsync();

        TempData["msg"] = "Đã lưu hóa đơn!";
        return RedirectToPage();
    }

    public class OrderItemDto
    {
        public string ItemID { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount => Quantity * Price;
    }

    public List<OrderItemDto> OrderItems { get; set; } = new(); // mẫu tĩnh (hoặc lấy từ DB)
    public int TotalQuantity => OrderItems.Sum(i => i.Quantity);
    public decimal TotalAmount => OrderItems.Sum(i => i.Amount);

    public async Task<JsonResult> OnGetItemAsync(string id)
    {
        var item = await _db.Items
            .Where(i => i.ItemID == id || i.ItemName == id)
            .Select(i => new {
                i.ItemID,
                i.ItemName,
                Unit = i.InvUnitOfMeasr,
                Price = 0 // hoặc giá mặc định nếu có
            })
            .FirstOrDefaultAsync();

        return new JsonResult(item);
    }

    public async Task<JsonResult> OnGetAllItemsAsync()
    {
        var items = await _db.Items
            .Select(i => new {
                i.ItemID,
                i.ItemName,
                Unit = i.InvUnitOfMeasr,
                Price = 0m
            }).ToListAsync();

        return new JsonResult(items);
    }
}
