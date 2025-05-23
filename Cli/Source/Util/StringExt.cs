using System;

namespace DocumentationWarning.Util;

public static class StringExt {
    public static string ReplaceFirst(this string text, string search, string replace) {
        var pos = text.IndexOf(search, StringComparison.Ordinal);
        return pos < 0 ? text : string.Concat(text.AsSpan(0, pos), replace, text.AsSpan(pos + search.Length));
    }

    public static string StripPrefix(this string text, string prefix) {
        return text.StartsWith(prefix) ? text[prefix.Length..] : text;
    }
}