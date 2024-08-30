public class XrefShortCode : SyncShortcode
{
    private const string Name = nameof(Name);
    private const string Xref = nameof(Xref);
    private const string IncludeHost = nameof(IncludeHost);
    
    public override ShortcodeResult Execute(KeyValuePair<string, string>[] args, string content, IDocument document, IExecutionContext context)
    {
        var argsDict = args.ToDictionary(Name, Xref, IncludeHost);
        var href = context.GetXrefLink(argsDict.GetString(Xref), argsDict.GetBool(IncludeHost));

        return $"<a href={href}>{argsDict.GetString(Name)}</a>";
    }
}