return await Bootstrapper
    .Factory
    .CreateDocs(args)
    .AddSourceFiles("../Unity-SV-Unitune/Runtime/**/*.cs")
    .AddSetting(Keys.LinksUseHttps, "true")
    .AddSetting(Statiq.Markdown.MarkdownKeys.MarkdownExtensions, "Bootstrap")
    .AddShortcode<CalloutShortCode>("Callout")
    .AddShortcode<KeyFeatureShortCode>("KeyFeatures")
    .AddShortcode<XrefShortCode>("Xref")
    .AddShortcode<AlertShortCode>("Alert")
    // https://github.com/statiqdev/Statiq.Docs/issues/58
    .AddSetting(WebKeys.OptimizeContentFileNames, "false")
    .RunAsync();