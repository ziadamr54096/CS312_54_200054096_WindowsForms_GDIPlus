// HARNESS ONLY: stand-in for System.Text.Json (absent from the .NET Framework reference assemblies used for the WinForms checks).
// The REAL System.Text.Json is exercised by the .NET 8 core test-suite. This mini version supports what NetMessage needs.
using System.Collections;
using System.Reflection;
using System.Text;
namespace System.Text.Json
{
    public class JsonException : Exception { public JsonException(string m) : base(m) { } }
    public static class JsonSerializer
    {
        public static byte[] SerializeToUtf8Bytes<T>(T value) { var sb = new StringBuilder(); Write(sb, value); return Encoding.UTF8.GetBytes(sb.ToString()); }
        static void Write(StringBuilder sb, object? v)
        {
            switch (v)
            {
                case null: sb.Append("null"); break;
                case string s: Quote(sb, s); break;
                case bool b: sb.Append(b ? "true" : "false"); break;
                case int or long: sb.Append(Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture)); break;
                case IDictionary d:
                    sb.Append('{'); bool first = true;
                    foreach (DictionaryEntry e in d) { if (!first) sb.Append(','); first = false; Quote(sb, (string)e.Key); sb.Append(':'); Write(sb, e.Value); }
                    sb.Append('}'); break;
                default:
                    sb.Append('{'); bool f2 = true;
                    foreach (var p in v.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    { if (!p.CanRead || !p.CanWrite) continue; if (!f2) sb.Append(','); f2 = false; Quote(sb, p.Name); sb.Append(':'); Write(sb, p.GetValue(v)); }
                    sb.Append('}'); break;
            }
        }
        static void Quote(StringBuilder sb, string s)
        {
            sb.Append('"');
            foreach (char c in s)
                switch (c) { case '"': sb.Append("\\\""); break; case '\\': sb.Append("\\\\"); break; case '\n': sb.Append("\\n"); break; case '\r': sb.Append("\\r"); break; case '\t': sb.Append("\\t"); break;
                    default: if (c < 0x20) sb.Append("\\u" + ((int)c).ToString("x4")); else sb.Append(c); break; }
            sb.Append('"');
        }
        public static T? Deserialize<T>(byte[] utf8)
        {
            var p = new P(Encoding.UTF8.GetString(utf8)); var tree = p.Value(); p.End();
            return (T?)Bind(tree, typeof(T));
        }
        static object? Bind(object? t, Type type)
        {
            var u = Nullable.GetUnderlyingType(type) ?? type;
            if (t == null) { if (u.IsValueType) throw new JsonException("null for value type"); return null; }
            if (u == typeof(string)) return t is string ? t : throw new JsonException("expected string");
            if (u == typeof(int)) return t is double d && d == Math.Floor(d) && Math.Abs(d) < int.MaxValue ? (int)d : throw new JsonException("expected int");
            if (u == typeof(bool)) return t is bool ? t : throw new JsonException("expected bool");
            if (u == typeof(Dictionary<string, string>))
            {
                if (t is not Dictionary<string, object?> src) throw new JsonException("expected object");
                var r = new Dictionary<string, string>();
                foreach (var kv in src) r[kv.Key] = kv.Value is string sv ? sv : throw new JsonException("expected string value");
                return r;
            }
            if (t is not Dictionary<string, object?> obj) throw new JsonException("expected object");
            var o = Activator.CreateInstance(u)!;
            foreach (var pr in u.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                if (pr.CanWrite && obj.TryGetValue(pr.Name, out var val)) pr.SetValue(o, Bind(val, pr.PropertyType));
            return o;
        }
        sealed class P
        {
            readonly string s; int i;
            public P(string text) { s = text; }
            void Ws() { while (i < s.Length && char.IsWhiteSpace(s[i])) i++; }
            public void End() { Ws(); if (i != s.Length) throw new JsonException("trailing data"); }
            public object? Value()
            {
                Ws(); if (i >= s.Length) throw new JsonException("unexpected end");
                char c = s[i];
                if (c == '{') { i++; var d = new Dictionary<string, object?>(); Ws(); if (i < s.Length && s[i] == '}') { i++; return d; }
                    while (true) { Ws(); var k = Str(); Ws(); Expect(':'); d[k] = Value(); Ws(); if (i < s.Length && s[i] == ',') { i++; continue; } Expect('}'); return d; } }
                if (c == '"') return Str();
                if (c == 't' && Lit("true")) return true; if (c == 'f' && Lit("false")) return false; if (c == 'n' && Lit("null")) return null;
                int st = i; while (i < s.Length && (char.IsDigit(s[i]) || "+-.eE".IndexOf(s[i]) >= 0)) i++;
                if (i == st || !double.TryParse(s.Substring(st, i - st), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var n)) throw new JsonException("invalid JSON");
                return n;
            }
            bool Lit(string l) { if (string.CompareOrdinal(s, i, l, 0, l.Length) == 0) { i += l.Length; return true; } throw new JsonException("invalid literal"); }
            void Expect(char c) { if (i >= s.Length || s[i] != c) throw new JsonException("expected " + c); i++; }
            string Str()
            {
                Expect('"'); var sb = new StringBuilder();
                while (true)
                {
                    if (i >= s.Length) throw new JsonException("unterminated string");
                    char c = s[i++];
                    if (c == '"') return sb.ToString();
                    if (c != '\\') { sb.Append(c); continue; }
                    if (i >= s.Length) throw new JsonException("bad escape");
                    char e = s[i++];
                    switch (e) { case 'n': sb.Append('\n'); break; case 'r': sb.Append('\r'); break; case 't': sb.Append('\t'); break; case 'b': sb.Append('\b'); break; case 'f': sb.Append('\f'); break;
                        case 'u': sb.Append((char)Convert.ToInt32(s.Substring(i, 4), 16)); i += 4; break; default: sb.Append(e); break; }
                }
            }
        }
    }
}
