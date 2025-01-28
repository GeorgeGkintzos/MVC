using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models;

[Table("bills")]
public partial class Bill
{
    [DisplayName("Bill_Id")]
    [Key]
    [Column("Bill_Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Αυτό κάνει το BillId auto-increment
    public int Bill_Id { get; set; }

    [DisplayName("Phone Number")]
    [Unicode(false)]
    public int? PhoneNumber { get; set; }

    [DisplayName("Cost")]
    [Column(TypeName = "decimal(7, 2)")]
    public decimal? Costs { get; set; }

    [ForeignKey("PhoneNumber")]
    [InverseProperty("Bills")]
    public virtual Phone? PhoneNumberNavigation { get; set; }

    [DisplayName("Paid")]
    public bool? Paid { get; set; } // Μπορεί να είναι null

    [DisplayName("Bill ID")]
    [ForeignKey("BillId")]
    [InverseProperty("Bills")]
    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();
}
