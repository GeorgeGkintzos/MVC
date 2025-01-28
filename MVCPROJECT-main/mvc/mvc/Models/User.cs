using Microsoft.EntityFrameworkCore;
using mvc.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace mvc.Models;

[Table("users")]
public partial class User
{
    [DisplayName("User ID")]
    [Key]
    [Column("User_id")]
    public int UserId { get; set; }

    [DisplayName("First Name")]
    [Column("First_Name")]
    [StringLength(50)]
    [Unicode(false)]
    [Required(ErrorMessage = "This Field is required")]
    public string? FirstName { get; set; }

    [DisplayName("Last Name")]
    [Column("Last_Name")]
    [StringLength(50)]
    [Unicode(false)]
    [Required(ErrorMessage = "This Field is required")]
    public string? LastName { get; set; }

    [DisplayName("Username")]
    [StringLength(50)]
    [Unicode(false)]
    [Required(ErrorMessage = "This Field is required")]
    public string? Username { get; set; }

    [DisplayName("Property")]
    [StringLength(50)]
    [Unicode(false)]
    [Required(ErrorMessage = "Please select a property")]
    public string? Property { get; set; }

    [DisplayName("Password")]
    [StringLength(50)]
    [Unicode(false)]
    [Required(ErrorMessage = "This Field is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    [InverseProperty("User")]
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    [InverseProperty("User")]
    public virtual ICollection<Seller> Sellers { get; set; } = new List<Seller>();
}
