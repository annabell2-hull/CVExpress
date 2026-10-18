using SQLite;

namespace CVExpress.Models;

public class Experience
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int CvId { get; set; }

    public string Poste { get; set; } = "";
    public string Entreprise { get; set; } = "";
    public string Periode { get; set; } = "";
}