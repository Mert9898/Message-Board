namespace Message_Board;

public enum Theme
{
    Default,
    Light,
    Dark
}

public class Preferences
{
    private bool MatureContent {get; set; }
    private Theme Theme {get; set;}

    public Preferences(bool matureContent, Theme theme)
    {
        this.MatureContent = matureContent;
        this.Theme = theme;
    }

    public void EditPreferences(bool matureContent, Theme theme)
    {
        this.MatureContent = matureContent;
        this.Theme = theme;
    }
}