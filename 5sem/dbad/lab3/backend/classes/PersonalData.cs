namespace backend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class PersonalData
{
    [Key]
    public string Fcs { get; set; }

    public string Itn { get; set; }
    public string Address { get; set; }
    public string SnPassport { get; set; }
    public bool? Married { get; set; }
    public int? Kids { get; set; }

    [ForeignKey("HfInfo, EduDoc")]
    public string Inila { get; set; }
}
