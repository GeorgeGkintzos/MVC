using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models;

[Table("sellers")]
public partial class Seller
{
    [DisplayName("Seller ID")]
    [Key]
    [Column("Seller_id")]
    public int SellerId { get; set; }

    [DisplayName("User ID")]
    [Column("User_id")]
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Sellers")]
    public virtual User? User { get; set; }
}
