namespace Djambi.UI.Resources
{
    public class Theme
    {
        public string ThemeName { get; set; }
        public Names Names { get; set; }
        public Style Style { get; set; }
    }

    public class Names
    {
        public string Assassin { get; set; }
        public string Chief { get; set; }
        public string Corpse { get; set; }
        public string Diplomat { get; set; }
        public string Journalist { get; set; }
        public string Thug { get; set; }
        public string Undertaker { get; set; }
        public string Seat { get; set; }
        public string Game { get; set; }
    }

    public class Style
    {
        public string[] PlayerColors { get; set; }
        public string[] CellColors { get; set; }
        public string BoardLabelColor { get; set; }
        public string BoardLabelBackgroundColor { get; set; }
        public string SelectionHighlightColor { get; set; }
        public string SelectionOptionHighlightColor { get; set; }
    }
}
