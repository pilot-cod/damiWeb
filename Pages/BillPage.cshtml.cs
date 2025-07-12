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

    [BindProperty] public string OrderNo { get; set; } = string.Empty;
    [BindProperty] public DateTime OrderDate { get; set; } = DateTime.Today;

    [BindProperty] public string? CustomerID { get; set; }     
    [BindProperty] public string? CustomerName { get; set; }    
    [BindProperty] public string? Address { get; set; }            
    [BindProperty] public List<OrderItemDto> Items { get; set; } = new();

    public List<Customer> AllCustomers { get; set; } = new();     
    public List<Item> AllItems { get; set; } = new();
    public List<string> ExistingOrderNos { get; set; } = new();

    public async Task OnGetAsync()
    {
        var last = await _db.OrderMasters
                            .OrderByDescending(o => o.OrderNo)
                            .Select(o => o.OrderNo)
                            .FirstOrDefaultAsync();

        int next = int.TryParse(last, out int cur) ? cur + 1 : 1;
        OrderNo = next.ToString("D3");
        Items.Add(new OrderItemDto());

        ExistingOrderNos = await _db.OrderMasters.OrderBy(o => o.OrderNo).Select(o => o.OrderNo).ToListAsync();
        AllCustomers = await _db.Customers.OrderBy(c => c.CustomerID).ToListAsync();
        AllItems = await _db.Items.OrderBy(i => i.ItemID).ToListAsync();
    }

    public async Task<JsonResult> OnGetCustomer(string id)
    {
        var c = await _db.Customers
                         .Where(c => c.CustomerID == id)
                         .Select(c => new { c.CustomerName, c.Address })
                         .FirstOrDefaultAsync();

        return new JsonResult(c);
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        var existingOrder = await _db.OrderMasters
                                     .Include(m => m.OrderDetails)
                                     .FirstOrDefaultAsync(m => m.OrderNo == OrderNo);

        if (existingOrder != null)
        {
            existingOrder.OrderDate = OrderDate;
            existingOrder.CustomerID = CustomerID!;
            existingOrder.TotalAmount = Items.Sum(x => x.Quantity * x.Price);

            _db.OrderDetails.RemoveRange(existingOrder.OrderDetails);

            int line = 1;
            foreach (var item in Items)
            {
                existingOrder.OrderDetails.Add(new OrderDetail
                {
                    ItemID = item.ItemID,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Amount = item.Amount,
                    LineNumber = line++,
                });
            }

            await _db.SaveChangesAsync();
            TempData["msg"] = "Đã cập nhật hóa đơn!";
        }
        else
        {
            var master = new OrderMaster
            {
                OrderDate = OrderDate,
                OrderNo = OrderNo,
                CustomerID = CustomerID!,
                TotalAmount = Items.Sum(x => x.Quantity * x.Price),
            };
            _db.OrderMasters.Add(master);
            await _db.SaveChangesAsync();

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
            TempData["msg"] = "Đã lưu hóa đơn mới!";
        }

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

    public List<OrderItemDto> OrderItems { get; set; } = new();
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
                Price = 0M
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

    public async Task<JsonResult> OnGetLoadOrderAsync(string orderNo)
    {
        var order = await _db.OrderMasters
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderNo == orderNo);

        if (order == null)
            return new JsonResult(null);

        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.CustomerID == order.CustomerID);

        var itemDict = await _db.Items
            .ToDictionaryAsync(i => i.ItemID);

        var result = new
        {
            order.OrderNo,
            OrderDate = order.OrderDate.ToString("yyyy-MM-dd"),
            order.CustomerID,
            CustomerName = customer?.CustomerName ?? "",
            Address = customer?.Address ?? "",
            Items = order.OrderDetails.Select(d => new {
                d.ItemID,
                ItemName = itemDict.ContainsKey(d.ItemID) ? itemDict[d.ItemID].ItemName : "",
                Unit = itemDict.ContainsKey(d.ItemID) ? itemDict[d.ItemID].InvUnitOfMeasr : "",
                d.Quantity,
                d.Price
            }).ToList()
        };

        return new JsonResult(result);
    }
}
