using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Order>>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] PaymentStatus? paymentStatus = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<PagedResult<Order>>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            var query = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Book)
                .AsQueryable();

            // Non-admin users can only see their own orders
            if (!isAdmin)
            {
                query = query.Where(o => o.UserId == userGuid);
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (paymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= endDate.Value);
            }

            var totalCount = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Order>
            {
                Items = orders,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Order>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, new ApiResponse<PagedResult<Order>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Order>>> GetOrder(Guid id)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<Order>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            var query = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Book)
                .Include(o => o.User)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(o => o.UserId == userGuid);
            }

            var order = await query.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new ApiResponse<Order>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            return Ok(new ApiResponse<Order>
            {
                Success = true,
                Data = order
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with ID: {OrderId}", id);
            return StatusCode(500, new ApiResponse<Order>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Order>>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Order>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<Order>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            // Validate items and calculate totals
            decimal subtotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemRequest in request.Items)
            {
                if (itemRequest.ProductId.HasValue)
                {
                    var product = await _context.Products.FindAsync(itemRequest.ProductId.Value);
                    if (product == null || !product.IsActive || !product.IsInStock)
                    {
                        return BadRequest(new ApiResponse<Order>
                        {
                            Success = false,
                            Message = $"Product {itemRequest.ProductId} not available"
                        });
                    }

                    var unitPrice = product.DiscountPrice ?? product.Price;
                    var totalPrice = unitPrice * itemRequest.Quantity;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = itemRequest.ProductId.Value,
                        ItemName = product.Name,
                        SKU = product.SKU,
                        UnitPrice = unitPrice,
                        Quantity = itemRequest.Quantity,
                        TotalPrice = totalPrice
                    });

                    subtotal += totalPrice;
                }
                else if (itemRequest.BookId.HasValue)
                {
                    var book = await _context.Books.FindAsync(itemRequest.BookId.Value);
                    if (book == null || !book.IsActive)
                    {
                        return BadRequest(new ApiResponse<Order>
                        {
                            Success = false,
                            Message = $"Book {itemRequest.BookId} not available"
                        });
                    }

                    var unitPrice = book.IsFree ? 0 : book.Price;
                    var totalPrice = unitPrice * itemRequest.Quantity;

                    orderItems.Add(new OrderItem
                    {
                        BookId = itemRequest.BookId.Value,
                        ItemName = book.Title,
                        SKU = book.ISBN,
                        UnitPrice = unitPrice,
                        Quantity = itemRequest.Quantity,
                        TotalPrice = totalPrice
                    });

                    subtotal += totalPrice;
                }
            }

            if (orderItems.Count == 0)
            {
                return BadRequest(new ApiResponse<Order>
                {
                    Success = false,
                    Message = "No valid items in order"
                });
            }

            // Generate order number
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = userGuid,
                Status = OrderStatus.Pending,
                SubTotal = subtotal,
                TaxAmount = 0, // Calculate tax based on business rules
                ShippingAmount = 0, // Calculate shipping based on business rules
                DiscountAmount = 0,
                TotalAmount = subtotal,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                ShippingName = request.ShippingAddress.Name,
                ShippingAddress = request.ShippingAddress.Address,
                ShippingCity = request.ShippingAddress.City,
                ShippingState = request.ShippingAddress.State,
                ShippingPostalCode = request.ShippingAddress.PostalCode,
                ShippingCountry = request.ShippingAddress.Country,
                ShippingPhone = request.ShippingAddress.Phone,
                Notes = request.Notes,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Add order items
            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
            }

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            // Load the complete order for response
            var completeOrder = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new ApiResponse<Order>
            {
                Success = true,
                Message = "Order created successfully",
                Data = completeOrder!
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, new ApiResponse<Order>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = User.Identity?.Name;

            // Update timestamps based on status
            switch (request.Status)
            {
                case OrderStatus.Shipped:
                    order.ShippedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Order status updated successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status for order ID: {OrderId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}/payment-status")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusRequest request)
    {
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            order.PaymentStatus = request.PaymentStatus;
            order.PaymentTransactionId = request.TransactionId;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Payment status updated successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for order ID: {OrderId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Order operations
public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}

public class UpdatePaymentStatusRequest
{
    public PaymentStatus PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
}
