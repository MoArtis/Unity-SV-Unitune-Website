using System.Xml.Linq;

public static class ShortCodeUtility
{
    public static void ProcessLinks(XElement element, string elementText)
    {
        var potentialLinks = elementText.Split('[');
        element.Add(potentialLinks[0]);
        for (var j = 1; j < potentialLinks.Length; j++)
        {
            var potentialLink = potentialLinks[j];

            var linkNameEndIndex = potentialLink.IndexOf(']');
            if (linkNameEndIndex == -1)
            {
                element.Add("[");
                element.Add(potentialLink);
                continue;
            }

            var linkName = potentialLink.Substring(0, linkNameEndIndex);
            var linkHrefStartIndex = linkNameEndIndex + 1;
            if (potentialLink.Length < linkHrefStartIndex || potentialLink[linkNameEndIndex + 1] != '(')
            {
                element.Add("[");
                element.Add(potentialLink);
                continue;
            }

            var linkHrefEndIndex = potentialLink.IndexOf(')');
            if (linkHrefEndIndex == -1)
            {
                element.Add("[");
                element.Add(potentialLink);
                continue;
            }

            var linkHref = potentialLink.Substring(linkHrefStartIndex + 1, linkHrefEndIndex - (linkHrefStartIndex + 1));
                
            element.Add(new XElement("a", new XAttribute("href", linkHref), linkName));
            element.Add(potentialLink.Substring(linkHrefEndIndex + 1));
        }
    }
}