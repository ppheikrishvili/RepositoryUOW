using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RepositoryUOWDomain.Entities.Base;
using RepositoryUOWDomain.Shared.Enums;

namespace RepositoryUOWDomain.Entities.System;

public class User : BaseEntity
{
    [Column("User Security ID")]
    public Guid User_Security_ID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column("User ID"), MaxLength(20), Required, RegularExpression(@"^[A-Za-z0-9]{1,20}$",
         ErrorMessage = "User ID - Only Latin uppercase characters and numbers with a length from 1 to 20 are required")]
    public string? User_ID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column("User Full Name"), MaxLength(80), RegularExpression(@"^[A-Za-z]{3,}$",
         ErrorMessage = "User_FullName - Only Latin characters with a length from 3 to 80 are required")]
    public string? User_FullName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Range(0, 1)]
    public UserStateEnum State { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column("Expiry Date"), DataType(DataType.Date)]
    public DateTime Expiry_Date { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column("Windows Security ID"), MaxLength(119), RegularExpression(@"^[A-Z0-9]{2,}$",
         ErrorMessage = "Windows Security ID - Only Latin uppercase characters with a length from 2 to 119 are required")]
    public string? Windows_Security_ID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column("Change Password")]
    public bool Change_Password { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column("License Type"), Range(0, 2)]
    public UserLicenseTypeEnum License_Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column("Contact Email"), EmailAddress]
    public string? Contact_Email { get; set; }
}