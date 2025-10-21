using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraqBankingApp.Models;

// Represents a person client in the system
public class Person
{
    [Key]
    [Column("code")]
    public int Code { get; set; }

    [StringLength(50)]
    [Column("name")]
    public string? Name { get; set; }

    [StringLength(50)]
    [Column("surname")]
    public string? Surname { get; set; }

    [Required]
    [StringLength(50)]
    [Column("id_number")]
    public string IdNumber { get; set; } = string.Empty;

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
