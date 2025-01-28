using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models;

[Table("programs")]
public partial class Program
{
    [Key]
    [Column("ProgramName")]
    [StringLength(50)]
    [Unicode(false)]
    public string ProgramName { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Benefits { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal? Charge { get; set; }

    [InverseProperty("ProgramNameNavigation")]
    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();

}
