using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models;

[Table("admin")]
public partial class Admin
{
    [DisplayName("Admin ID")]
    [Key]
    [Column("Admin_id")]
    public int AdminId { get; set; }

    [DisplayName("User ID")]
    [Column("User_id")]
    public int? UserId { get; set; }

    [DisplayName("User ID")]
    [ForeignKey("UserId")]
    [InverseProperty("Admins")]
    public virtual User? User { get; set; }
}
