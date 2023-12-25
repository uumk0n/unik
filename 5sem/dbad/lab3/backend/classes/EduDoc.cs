namespace backend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EduDocs
{
    [Key]
    public string Inila { get; set; }

    public string SndEduDoc { get; set; }
    public DateTime? DateEnd { get; set; }

    [ForeignKey("Institut")]
    public string RegEduDoc { get; set; }
}
