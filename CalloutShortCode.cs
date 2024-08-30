public class CalloutShortCode : SyncShortcode
{
    private const string Type = nameof(Type);
    private const string Title = nameof(Title);
    
    public override ShortcodeResult Execute(KeyValuePair<string, string>[] args, string content, IDocument document, IExecutionContext context)
    {
        var dictionary = args.ToDictionary(Type, Title);
        
        return $"<div class=\"callout callout-{dictionary[Type]}\">\n<h4>{dictionary[Title]}</h4>\n{content}</div>";
        
        // var div = new XElement(
        //     "div",
        //     new XAttribute("Class", $"callout callout-"));
        //
        // div.Value = content;
        //
        // var title = dictionary.GetString(Title);
        // if (!string.IsNullOrWhiteSpace(title))
        // {
        //     div.AddFirst(new XElement("h4", title));
        // }
        //
        // return div.ToString();
    }
}