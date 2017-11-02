namespace AguaSB.Utilerias
{
    public static class Cadenas
    {
        public static string Capitalizar(string s) => char.ToUpper(s[0]) + s.Substring(1);
    }
}
