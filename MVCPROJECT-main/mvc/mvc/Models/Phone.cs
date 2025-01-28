using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models;

[Table("phones")]
public partial class Phone
{
    [Key]
    [Column("PhoneNumber")]
    [Unicode(false)]
    public int PhoneNumber { get; set; }

    [Column("ProgramName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ProgramName { get; set; }

    [InverseProperty("PhoneNumberNavigation")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    [ForeignKey("ProgramName")]
    public virtual Program? ProgramNameNavigation { get; set; }

    [InverseProperty("Phone")] // Αντίστροφη σχέση με τον Client
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

}
