using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraqBankingApp.Models;

// Represents a transaction captured against an account
public class TransactionEntry
{
    [Key]
    [Column("code")]
    public int Code { get; set; }

    [Required]
    [Column("account_code")]
    public int AccountCode { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Column("capture_date")]
    public DateTime CaptureDate { get; set; }

    [Required]
    [Column("amount", TypeName = "money")]
    [Range(typeof(decimal), "-79228162514264337593543950335", "79228162514264337593543950335")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(100)]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    public Account? Account { get; set; }
}
