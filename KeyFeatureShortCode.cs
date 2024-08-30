using System.Xml.Linq;

public class KeyFeatureShortCode : SyncShortcode
{
    public override ShortcodeResult Execute(KeyValuePair<string, string>[] args, string content, IDocument document,
        IExecutionContext context)
    {
        var doc = context.Outputs.FromPipeline("Data").FilterSources("Data/Features.json").FirstOrDefault();
        var features = doc.GetDocuments("Features").ToArray();

        var container = new XElement(
            "div",
            new XAttribute("Class", "container"),
            content);

        var rowAttribute = new XAttribute("Class", "row");
        var row = new XElement("div", rowAttribute);
        container.Add(row);

        var cardContainerAttribute = new XAttribute("Class", "col-md-6 p-1");
        var cardAttribute = new XAttribute("Class", "card");
        var imgAttribute = new XAttribute("Class", "feature-img card-img-top");
        var titleAttribute = new XAttribute("Class", "feature-title d-flex align-items-center justify-content-center");
        var h2Attribute = new XAttribute("Class", "card-title text-center");
        var cardBodyAttribute = new XAttribute("Class", "feature-body card-body");
        var descriptionAttribute = new XAttribute("Class", "feature-description");

        for (var i = 0; i < features.Length; i++)
        {
            var feature = features[i];

            var cardContainer = new XElement("div", cardContainerAttribute);
            var card = new XElement("div", cardAttribute);

            var titleText = feature.GetString("Title");
            var imgSrc = feature.GetString("Img");
            var isVideo = feature.GetBool("IsVideo");
            var descriptionText = feature.GetString("Description");

            XElement media;
            if (isVideo)
            {
                media = new XElement("video", imgAttribute,
                    new XAttribute("autoplay", "true"),
                    new XAttribute("muted", "true"),
                    new XAttribute("loop", "true"),
                    new XAttribute("src", imgSrc), titleText);
            }
            else
            {
                media = new XElement("img", imgAttribute, new XAttribute("src", imgSrc), new XAttribute("alt", titleText));
            }

            var cardBody = new XElement("div", cardBodyAttribute);

            var title = new XElement("div", titleAttribute);
            var h2 = new XElement("h2", h2Attribute, titleText);


            var description = new XElement("p", descriptionAttribute);

            ShortCodeUtility.ProcessLinks(description, descriptionText);

            title.Add(h2);

            cardBody.Add(title);
            // cardBody.Add(description);

            card.Add(media);
            // card.Add(cardBody);

            cardContainer.Add(card);

            row.Add(cardContainer);
        }

        return container.ToString();
    }
}