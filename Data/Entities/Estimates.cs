namespace ctrlAltDefeatApp.Data.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ctrlAltDefeatApp.Data.Models;

public class Estimates
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}
    public double EstimatedTime {get; set;}
    public double RealEffort {get; set;}
    public int Session {get; set;}

    public Guid UserId {get; set;}

    public EstimateState currentEstimateState {get; set;}

}
