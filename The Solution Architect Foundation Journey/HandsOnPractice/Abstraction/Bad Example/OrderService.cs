using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Abstraction.Bad_Example;

public class OrderService
{
    public void Checkout(Order order)
    {
        // ... calculate total, apply discounts ...
        decimal total = order.Items.Sum(i => i.Price * i.Qty);

        // Now charge the customer — directly, inline, right here.
        var payload = new
        {
            amount = (int)(total * 100), // Stripe wants cents
            currency = "usd",
            source = order.CardToken
        };
        string json = JsonSerializer.Serialize(payload);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "sk_live_51Hx9aBCDEF..."); // hardcoded key

        var response = client.PostAsync(
            "https://api.stripe.com/v1/charges",
            new StringContent(json, Encoding.UTF8, "application/json")
        ).Result;

        string responseBody = response.Content.ReadAsStringAsync().Result;
        if (!responseBody.Contains("\"status\":\"succeeded\""))
            throw new Exception("Payment failed: " + responseBody);

        order.Status = OrderStatus.Paid;
    }
}


public class Order
{
    public List<OrderItem> Items { get; set; } = new();
    public string CardToken { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}

public class OrderItem
{
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
}

public enum OrderStatus
{
    Pending,
    Paid,
    Cancelled
}