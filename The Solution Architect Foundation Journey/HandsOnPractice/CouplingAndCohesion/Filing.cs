using System;
using System.Collections.Generic;
using System.Text;

namespace CouplingAndCohesion;

public class Filing
{
    public int Id { get; set; }
    public int TaxYear { get; set; }
    public decimal Amount { get; set; }
    public string Country { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public string TaxpayerName { get; set; } = string.Empty;
    public string TaxpayerId { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string Currency { get; set; } = "USD";
    public FilingStatus Status { get; set; } = FilingStatus.Pending;
}

public enum FilingStatus
{
    Pending,
    Submitted,
    Approved,
    Rejected
}
