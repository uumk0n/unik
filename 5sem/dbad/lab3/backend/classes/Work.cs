namespace backend;
using System.ComponentModel.DataAnnotations;

public class Work
{
    [Key]
    public string RegOrgNum { get; set; }

    public string NameOrg { get; set; }
    public string ItnOrg { get; set; }
    public string OrgAddress { get; set; }
}
