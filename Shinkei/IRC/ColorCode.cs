using System;
using System.Text.RegularExpressions;

namespace Shinkei.IRC
{
    public static class ColorCode
    {
        public static string COLOR = Char.ConvertFromUtf32(3);
        public static string NORMAL = Char.ConvertFromUtf32(15);
        public static string BOLD = Char.ConvertFromUtf32(2);
        public static string ITALIC = Char.ConvertFromUtf32(9);
        public static string STRIKETROUGH = Char.ConvertFromUtf32(19);
        public static string UNDERLINE = Char.ConvertFromUtf32(21);
        public static string UNDERLINE_ALT = Char.ConvertFromUtf32(31);
        public static string REVERSE = Char.ConvertFromUtf32(22);
        public static string WHITE = COLOR + "00";
        public static string BLACK = COLOR + "01";
        public static string DARK_BLUE = COLOR + "02";
        public static string DARK_GREEN = COLOR + "03";
        public static string RED = COLOR + "04";
        public static string BROWN = COLOR + "05";
        public static string PURPLE = COLOR + "06";
        public static string OLIVE = COLOR + "07";
        public static string YELLOW = COLOR + "08";
        public static string GREEN = COLOR + "09";
        public static string TEAL = COLOR + "10";
        public static string CYAN = COLOR + "11";
        public static string BLUE = COLOR + "12";
        public static string MAGENTA = COLOR + "13";
        public static string DARK_GRAY = COLOR + "14";
        public static string LIGHT_GRAY = COLOR + "15";

        public static String StripColors(this String msg)
        {
            return Regex.Replace(msg, @"[\x02\x1F\x0F\x16]|\x03(\d\d?(,\d\d?)?)?", String.Empty);
        }
    }
}