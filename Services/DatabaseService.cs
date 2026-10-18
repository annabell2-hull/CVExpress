using System.Security.Cryptography;
using SQLite;
using CVExpress.Models;

namespace CVExpress.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> ConnexionAsync()
    {
        if (_db != null) return _db;

        var chemin = Path.Combine(FileSystem.AppDataDirectory,
                                  "cvexpress.db3");
        var db = new SQLiteAsyncConnection(chemin);
        await db.CreateTableAsync<User>();
        await db.CreateTableAsync<Cv>();
        await db.CreateTableAsync<Experience>();
        _db = db;
        return db;
    }

    private static byte[] Hacher(string motDePasse, byte[] sel)
        => Rfc2898DeriveBytes.Pbkdf2(motDePasse, sel, 100_000,
                                     HashAlgorithmName.SHA256, 32);

    // ----- Comptes -----

    // Retourne null si l'e-mail est déjà utilisé
    public async Task<User?> InscrireAsync(string prenom, string nom,
                                           string email, string motDePasse)
    {
        var db = await ConnexionAsync();

        var emailNormalise = email.Trim().ToLowerInvariant();
        var existant = await db.Table<User>()
            .Where(u => u.Email == emailNormalise)
            .FirstOrDefaultAsync();
        if (existant != null) return null;

        var sel = RandomNumberGenerator.GetBytes(16);
        var utilisateur = new User
        {
            Prenom = prenom,
            Nom = nom,
            Email = emailNormalise,
            Sel = Convert.ToBase64String(sel),
            MotDePasseHash = Convert.ToBase64String(Hacher(motDePasse, sel))
        };

        await db.InsertAsync(utilisateur);
        return utilisateur;
    }

    // Retourne null si l'e-mail ou le mot de passe est faux
    public async Task<User?> ConnecterAsync(string email, string motDePasse)
    {
        var db = await ConnexionAsync();

        var emailNormalise = email.Trim().ToLowerInvariant();
        var utilisateur = await db.Table<User>()
            .Where(u => u.Email == emailNormalise)
            .FirstOrDefaultAsync();
        if (utilisateur == null) return null;

        var sel = Convert.FromBase64String(utilisateur.Sel);
        var calcule = Hacher(motDePasse, sel);
        var attendu = Convert.FromBase64String(utilisateur.MotDePasseHash);

        return CryptographicOperations.FixedTimeEquals(calcule, attendu)
            ? utilisateur
            : null;
    }

    // ----- CV -----

    public async Task<List<Cv>> ListerCvAsync(int userId)
    {
        var db = await ConnexionAsync();
        return await db.Table<Cv>()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.DateModification)
            .ToListAsync();
    }

    public async Task<Cv> AjouterCvAsync(int userId, string titre)
    {
        var db = await ConnexionAsync();
        var cv = new Cv
        {
            UserId = userId,
            Titre = titre,
            DateModification = DateTime.Now
        };

        await db.InsertAsync(cv);
        return cv;
    }
    public async Task<Cv> AjouterCvAsync(Cv cv)
    {
        var db = await ConnexionAsync();
        cv.DateModification = DateTime.Now;
        await db.InsertAsync(cv);
        return cv;
    }

    public async Task<Cv?> ObtenirCvAsync(int cvId)
    {
        var db = await ConnexionAsync();
        return await db.Table<Cv>()
            .Where(c => c.Id == cvId)
            .FirstOrDefaultAsync();
    }

    public async Task SauvegarderCvAsync(Cv cv)
    {
        var db = await ConnexionAsync();
        cv.DateModification = DateTime.Now;
        await db.UpdateAsync(cv);
    }
    // ----- Expériences -----

    public async Task<List<Experience>> ListerExperiencesAsync(int cvId)
    {
        var db = await ConnexionAsync();
        return await db.Table<Experience>()
            .Where(e => e.CvId == cvId)
            .OrderBy(e => e.Id)
            .ToListAsync();
    }

    public async Task<Experience?> ObtenirExperienceAsync(int id)
    {
        var db = await ConnexionAsync();
        return await db.Table<Experience>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task SauvegarderExperienceAsync(Experience experience)
    {
        var db = await ConnexionAsync();
        if (experience.Id == 0) await db.InsertAsync(experience);
        else await db.UpdateAsync(experience);
    }

    public async Task SupprimerExperienceAsync(Experience experience)
    {
        var db = await ConnexionAsync();
        await db.DeleteAsync(experience);
    }
}