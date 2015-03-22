namespace Shinkei.IRC
{
    public class ColorsCode
    {
        public static string WHITE = ControlCode.COLOR + "00";
        public static string BLACK = ControlCode.COLOR + "01";
        public static string DARK_BLUE = ControlCode.COLOR + "02";
        public static string DARK_GREEN = ControlCode.COLOR + "03";
        public static string RED = ControlCode.COLOR + "04";
        public static string BROWN = ControlCode.COLOR + "05";
        public static string PURPLE = ControlCode.COLOR + "06";
        public static string OLIVE = ControlCode.COLOR + "07";
        public static string YELLOW = ControlCode.COLOR + "08";
        public static string GREEN = ControlCode.COLOR + "09";
        public static string TEAL = ControlCode.COLOR + "10";
        public static string CYAN = ControlCode.COLOR + "11";
        public static string BLUE = ControlCode.COLOR + "12";
        public static string MAGENTA = ControlCode.COLOR + "13";
        public static string DARK_GRAY = ControlCode.COLOR + "14";
        public static string LIGHT_GRAY = ControlCode.COLOR + "15";
    }

    public class ControlCode
    {
        public static char COLOR = (char)3;
        public static char RESET = (char)15;
        public static char BOLD = (char)2;
        public static char ITALIC = (char) 9;
        public static char STRIKETROUGH = (char) 19;
        public static char UNDERLINE = (char)21;
        public static char UNDERLINE_ALT = (char)31;
        public static char REVERSE = (char)22;
    }
}