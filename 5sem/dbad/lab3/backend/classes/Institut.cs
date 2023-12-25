namespace backend;
using System.ComponentModel.DataAnnotations;

public class Institut
{
    [Key]
    public string RegEduDoc { get; set; }

    public string Type { get; set; }
    public string Name { get; set; }
}
