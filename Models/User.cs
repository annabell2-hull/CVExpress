using SQLite;

namespace CVExpress.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Prenom { get; set; } = "";
    public string Nom { get; set; } = "";

    [Unique]
    public string Email { get; set; } = "";

    public string MotDePasseHash { get; set; } = "";
    public string Sel { get; set; } = "";
}
