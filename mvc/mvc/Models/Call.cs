using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models;

[Table("calls")]
public partial class Call
{
    [DisplayName("Call ID")]
    [Key]
    [Column("Call_ID")]
    public int CallId { get; set; }

    [DisplayName("Description")]
    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [Key]
    [Column("PhoneNumber")]
    [Unicode(false)]
    public int PhoneNumber { get; set; }

    [Key]
    [Column("Incoming")]
    [Unicode(false)]
    public int Incoming { get; set; }

    [ForeignKey("CallId")]
    [InverseProperty("Calls")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
