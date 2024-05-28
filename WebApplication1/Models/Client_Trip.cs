using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lab8.Models;

[PrimaryKey("IdClient", "IdTrip")]
[Table("Client_Trip")]
public partial class Client_Trip
{
    [Key]
    public int IdClient { get; set; }

    [Key]
    public int IdTrip { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RegisteredAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [ForeignKey("IdClient")]
    [InverseProperty("Client_Trips")]
    public virtual Client IdClientNavigation { get; set; } = null!;

    [ForeignKey("IdTrip")]
    [InverseProperty("Client_Trips")]
    public virtual Trip IdTripNavigation { get; set; } = null!;
}
