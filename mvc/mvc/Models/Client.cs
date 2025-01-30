using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models;

[Table("clients")]
public partial class Client
{
    [DisplayName("Client ID")]
    [Key]
    [Column("Client_ID")]
    public int ClientId { get; set; }

    [DisplayName("AFM")]
    [Column("AFM")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Afm { get; set; }

    [DisplayName("Phone Number")]
    [Unicode(false)]
    public int? PhoneNumber { get; set; }

    [DisplayName("User ID")]
    [Column("User_id")]
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Clients")]
    public virtual User? User { get; set; }

    [ForeignKey("PhoneNumber")] // Ορισμός του Foreign Key
    [InverseProperty("Clients")] // Αντίστροφη πλοήγηση
    public virtual Phone? Phone { get; set; }

}
