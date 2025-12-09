using System;

namespace MessageBoard.Domain
{
    [Serializable]
    public class Preferences
    {
        public bool ShowMatureContent { get; set; } = false;
        public string Theme { get; set; } = "Light";
        
        public Preferences()
        {
        }
        
        public Preferences(bool showMatureContent, string theme)
        {
            ShowMatureContent = showMatureContent;
            Theme = theme ?? "Light";
        }
    }
}
