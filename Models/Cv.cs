using SQLite;

namespace CVExpress.Models;

public class Cv
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int UserId { get; set; }

    public string Titre { get; set; } = "";
    public DateTime DateModification { get; set; } = DateTime.Now;

    public string Prenom { get; set; } = "";
    public string Nom { get; set; } = "";
    public string Profession { get; set; } = "";
    public string Telephone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Ville { get; set; } = "";
    public string APropos { get; set; } = "";

    public string Modele { get; set; } = "Classique";
    public string Formation { get; set; } = "";
    public string Competences { get; set; } = "";
}