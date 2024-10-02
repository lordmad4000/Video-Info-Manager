namespace VideoInfoManager.Presentation.CrossCutting.Models;

public class VideoInfoRenameConfiguration
{
    public int Position { get; set; } = 0;
    public string[] FirstDelimiter { get; set; } = Array.Empty<string>();
    public string[] LastDelimiter { get; set; } = Array.Empty<string>();
    public string[] IgnoreDelimiter { get; set; } = Array.Empty<string>();
    public string[] WordsToDelete { get; set; } = Array.Empty<string>();
    public string Separator {  get; set; } = string.Empty;
    public string[] AuthorSeparators { get; set; } = Array.Empty<string>();
}
