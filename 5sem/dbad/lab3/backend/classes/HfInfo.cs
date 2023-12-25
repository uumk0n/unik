namespace backend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HFInfo
{
    [Key]
    public string Inila { get; set; }

    public string Status { get; set; }
    public string? IdInfo { get; set; }
    public DateTime? DateOrd { get; set; }

    [ForeignKey("Work")]
    public string RegOrgNum { get; set; }
}
