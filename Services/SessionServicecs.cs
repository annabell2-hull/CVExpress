using CVExpress.Models;

namespace CVExpress.Services;

public class SessionService
{
    public int? UserId { get; private set; }
    public string Prenom { get; private set; } = "";
    public string Nom { get; private set; } = "";
    public string Email { get; private set; } = "";

    public void Ouvrir(User utilisateur)
    {
        UserId = utilisateur.Id;
        Prenom = utilisateur.Prenom;
        Nom = utilisateur.Nom;
        Email = utilisateur.Email;
    }

    public void Fermer()
    {
        UserId = null;
        Prenom = "";
        Nom = "";
        Email = "";
    }
}