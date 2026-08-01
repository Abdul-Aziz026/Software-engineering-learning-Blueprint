using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Abstraction.Good_Example;

// The abstraction: what checkout NEEDS, nothing about how it happens.
public interface IPaymentGateway
{
    void Charge(decimal amount, string cardToken);
    void Charge(decimal total, object cardTo, object ken);
}

// The detail lives here now — isolated, swappable, testable on its own.
public class StripeGateway : IPaymentGateway
{
    private readonly HttpClient _client;
    private readonly string _apiKey;

    public StripeGateway(HttpClient client, string apiKey)
    {
        _client = client;
        _apiKey = apiKey;
    }

    public void Charge(decimal amount, string cardToken)
    {
        var payload = new { amount = (int)(amount * 100), currency = "usd", source = cardToken };
        string json = JsonSerializer.Serialize(payload);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = _client.PostAsync(
            "https://api.stripe.com/v1/charges",
            new StringContent(json, Encoding.UTF8, "application/json")
        ).Result;

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Stripe charge failed: {response.StatusCode}");
    }

    public void Charge(decimal total, object cardTo, object ken)
    {
        throw new NotImplementedException();
    }
}

// OrderService no longer knows Stripe, HTTP, or JSON exist.
public class OrderService
{
    private readonly IPaymentGateway _gateway;

    public OrderService(IPaymentGateway gateway) => _gateway = gateway;

    public void Checkout(Order order)
    {
        decimal total = order.Items.Sum(i => i.Price * i.Qty);
        _gateway.Charge(total, order.CardToken);
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