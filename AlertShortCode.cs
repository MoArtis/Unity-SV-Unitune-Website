public class AlertShortCode : SyncShortcode
{
    private const string Type = nameof(Type);

    public override ShortcodeResult Execute(KeyValuePair<string, string>[] args, string content, IDocument document,
        IExecutionContext context)
    {
        var dictionary = args.ToDictionary(Type);

        return $"<div class=\"alert alert-{dictionary[Type]}\" role=\"alert\">\n{content}\n</div>";
    }
}