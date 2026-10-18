using System.Globalization;
using System.Text;

namespace CVExpress.Services;

// Mini générateur de PDF (texte + traits), sans dépendance externe
public class PdfSimple
{
    private const double Largeur = 595, Hauteur = 842, Marge = 50;
    private readonly List<StringBuilder> _pages = new();
    private double _y;

    public PdfSimple() => NouvellePage();

    private void NouvellePage()
    {
        _pages.Add(new StringBuilder());
        _y = Hauteur - Marge;
    }

    private static string F(double v)
        => v.ToString("0.##", CultureInfo.InvariantCulture);

    private static string Echapper(string texte)
    {
        var sb = new StringBuilder();
        foreach (var c in texte)
        {
            switch (c)
            {
                case '\\': case '(': case ')': sb.Append('\\').Append(c); break;
                case '\r': case '\n': case '\t': sb.Append(' '); break;
                case '’': case '‘': sb.Append('\''); break;
                case '“': case '”': sb.Append('"'); break;
                case '–': case '—': sb.Append('-'); break;
                case '…': sb.Append("..."); break;
                case 'œ': sb.Append("oe"); break;
                case 'Œ': sb.Append("OE"); break;
                case '€': sb.Append("EUR"); break;
                default:
                    sb.Append(c < ' ' || (c >= '\u007f' && c <= '\u009f')
                              || c > 'ÿ' ? '?' : c);
                    break;
            }
        }
        return sb.ToString();
    }

    private static IEnumerable<string> Couper(string texte, int max)
    {
        foreach (var paragraphe in texte.Split('\n'))
        {
            var ligne = "";
            foreach (var mot in paragraphe.Split(' ',
                         StringSplitOptions.RemoveEmptyEntries))
            {
                if (ligne.Length > 0 && ligne.Length + mot.Length + 1 > max)
                {
                    yield return ligne;
                    ligne = mot;
                }
                else ligne = ligne.Length == 0 ? mot : ligne + " " + mot;
            }
            yield return ligne;
        }
    }

    public void Texte(string texte, double taille = 11, bool gras = false,
                      Color? couleur = null, double retrait = 0)
    {
        var c = couleur ?? Color.FromArgb("#222222");
        var maxCar = (int)((Largeur - 2 * Marge - retrait) / (taille * 0.52));

        foreach (var ligne in Couper(texte ?? "", maxCar))
        {
            if (_y - taille < Marge) NouvellePage();
            _pages[^1].Append(
                $"BT /{(gras ? "F2" : "F1")} {F(taille)} Tf " +
                $"{F(c.Red)} {F(c.Green)} {F(c.Blue)} rg " +
                $"{F(Marge + retrait)} {F(_y - taille)} Td " +
                $"({Echapper(ligne)}) Tj ET\n");
            _y -= taille * 1.35;
        }
    }

    public void Trait(Color couleur, double epaisseur = 1)
    {
        if (_y - 6 < Marge) NouvellePage();
        _pages[^1].Append(
            $"{F(couleur.Red)} {F(couleur.Green)} {F(couleur.Blue)} RG " +
            $"{F(epaisseur)} w {F(Marge)} {F(_y - 2)} m " +
            $"{F(Largeur - Marge)} {F(_y - 2)} l S\n");
        _y -= 8;
    }

    public void Espace(double hauteur) => _y -= hauteur;

    public byte[] Generer()
    {
        var n = _pages.Count;
        var objets = new List<string>
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "", // pages, rempli ci-dessous
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>"
        };

        var kids = string.Join(" ",
            Enumerable.Range(0, n).Select(i => $"{5 + 2 * i} 0 R"));
        objets[1] = $"<< /Type /Pages /Kids [{kids}] /Count {n} >>";

        for (var i = 0; i < n; i++)
        {
            objets.Add(
                $"<< /Type /Page /Parent 2 0 R " +
                $"/MediaBox [0 0 {F(Largeur)} {F(Hauteur)}] " +
                $"/Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> " +
                $"/Contents {6 + 2 * i} 0 R >>");

            var flux = _pages[i].ToString();
            var longueur = Encoding.Latin1.GetByteCount(flux);
            objets.Add($"<< /Length {longueur} >>\nstream\n{flux}\nendstream");
        }

        var sortie = new MemoryStream();
        void Ecrire(string s)
        {
            var octets = Encoding.Latin1.GetBytes(s);
            sortie.Write(octets, 0, octets.Length);
        }

        Ecrire("%PDF-1.4\n");
        var positions = new List<long>();
        for (var i = 0; i < objets.Count; i++)
        {
            positions.Add(sortie.Position);
            Ecrire($"{i + 1} 0 obj\n{objets[i]}\nendobj\n");
        }

        var xref = sortie.Position;
        Ecrire($"xref\n0 {objets.Count + 1}\n0000000000 65535 f \n");
        foreach (var p in positions) Ecrire($"{p:D10} 00000 n \n");
        Ecrire($"trailer\n<< /Size {objets.Count + 1} /Root 1 0 R >>\n" +
               $"startxref\n{xref}\n%%EOF");

        return sortie.ToArray();
    }
}