using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraqBankingApp.Models;

// Represents a bank account owned by a person
public class Account
{
    [Key]
    [Column("code")]
    public int Code { get; set; }

    [Required]
    [Column("person_code")]
    public int PersonCode { get; set; }

    [Required]
    [StringLength(50)]
    [Column("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    [Column("outstanding_balance", TypeName = "money")]
    public decimal OutstandingBalance { get; set; }

    // Additional navigation/status
    public Person? Person { get; set; }

    public int? StatusCode { get; set; } // not in DDL but needed to manage open/closed via Status table
    public Status? Status { get; set; }

    public ICollection<TransactionEntry> Transactions { get; set; } = new List<TransactionEntry>();
}
