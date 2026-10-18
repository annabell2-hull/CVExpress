using CVExpress.Models;

namespace CVExpress.Services;

public class CvPdfService
{
    // Crée le PDF et retourne son chemin
    public string Generer(Cv cv, List<Experience> experiences)
    {
        var moderne = cv.Modele == "Moderne";
        var sobre = cv.Modele == "Sobre";

        var accent = moderne ? Color.FromArgb("#1F6174")
                   : sobre ? Color.FromArgb("#6B7280")
                   : Colors.Black;

        var pdf = new PdfSimple();

        void Section(string titre)
        {
            pdf.Espace(12);
            pdf.Texte(sobre ? titre.ToUpperInvariant() : titre,
                      sobre ? 11 : 15, true, accent);
            if (!sobre) pdf.Trait(accent, 0.7);
            pdf.Espace(2);
        }

        pdf.Texte($"{cv.Prenom} {cv.Nom}".Trim(), sobre ? 22 : 26, true, accent);
        pdf.Texte(cv.Profession, 13, true);

        var contact = string.Join("   |   ",
            new[] { cv.Telephone, cv.Email, cv.Ville }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
        if (contact.Length > 0) pdf.Texte(contact, 10, false, Colors.Gray);

        pdf.Espace(6);
        if (!sobre) pdf.Trait(accent, moderne ? 2 : 1);

        if (!string.IsNullOrWhiteSpace(cv.APropos))
        {
            Section("À propos");
            pdf.Texte(cv.APropos, 11);
        }

        if (experiences.Count > 0)
        {
            Section("Expérience");
            foreach (var e in experiences)
            {
                pdf.Texte(e.Poste, 12, true);
                var detail = string.Join(" - ",
                    new[] { e.Entreprise, e.Periode }
                        .Where(s => !string.IsNullOrWhiteSpace(s)));
                pdf.Texte(detail, 11, false, Colors.Gray);
                pdf.Espace(6);
            }
        }

        if (!string.IsNullOrWhiteSpace(cv.Formation))
        {
            Section("Formation");
            pdf.Texte(cv.Formation, 11);
        }

        if (!string.IsNullOrWhiteSpace(cv.Competences))
        {
            Section("Compétences");
            pdf.Texte(cv.Competences, 11);
        }

        var nom = string.Concat(
            $"CV_{cv.Prenom}_{cv.Nom}"
                .Where(c => char.IsLetterOrDigit(c) || c == '_'));
        var chemin = Path.Combine(FileSystem.CacheDirectory, nom + ".pdf");
        File.WriteAllBytes(chemin, pdf.Generer());
        return chemin;
    }
}