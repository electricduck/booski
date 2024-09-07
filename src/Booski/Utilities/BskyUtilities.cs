namespace Booski.Utilities;
using System.Text;
using Booski.Common;
using Booski.Lib.Common;
using Booski.Lib.Polymorphs.AppBsky;

public class BskyUtilities
{
    public static string GetContentWarning(Polymorph labels)
    {
        string contentWarning = "";

        if(labels.GetType() == typeof(Booski.Lib.Polymorphs.ComAtproto.LabelDefsSelfLabels))
        {
            var selfLabels = labels as Booski.Lib.Polymorphs.ComAtproto.LabelDefsSelfLabels;

            foreach(var label in selfLabels.Values)
            {
                contentWarning = label.Val switch
                {
                    "nudity" => "Nudity",
                    "porn" => "Porn",
                    "sexual" => "Suggestive",
                    _ => "Unknown"
                };
            }
        }

        return contentWarning;
    }

    public static Embed ParseEmbeds(
        Polymorph embed,
        string did
    )
    {
        Embed parsedEmbeds = new Embed();
        parsedEmbeds.Items = new List<EmbedItem>();

        Type embedType = embed.GetType();

        switch (embedType)
        {
            case Type when embedType == typeof(EmbedExternal):
                var embedItem = new EmbedItem();
                var externalEmbed = embed as EmbedExternal;
                var externalEmbedUri = externalEmbed.External.Uri;

                if (
                    externalEmbedUri.ToString().Contains("media.tenor.com") &&
                    externalEmbedUri.ToString().Contains(".gif")
                )
                {
                    parsedEmbeds.Type = Enums.EmbedType.Gif;
                    embedItem.MimeType = "image/gif";
                }

                embedItem.Uri = externalEmbedUri;

                parsedEmbeds.Items.Add(embedItem);
                break;

            case Type when embedType == typeof(EmbedImages):
                var imagesEmbed = embed as EmbedImages;
                parsedEmbeds.Type = Enums.EmbedType.Images;

                foreach (var image in imagesEmbed.Images)
                {
                    var imageFile = image.File as Booski.Lib.Internal.ComAtproto.Common.FileBlob;
                    var imageUri = GetCdnUri(did, imageFile.Ref.Link, imageFile.MimeType);

                    parsedEmbeds.Items.Add(
                        new EmbedItem {
                            MimeType = imageFile.MimeType,
                            Uri = imageUri
                        }
                    );
                }
                break;
        }

        return parsedEmbeds;
    }

    public static string ParseFacets(
        string originalString,
        List<Booski.Lib.Internal.AppBsky.Common.Facet> facets,
        string linkStringStart = "<a href=\"[uri]\">",
        string linkStringEnd = "</a>",
        string mentionStringStart = "<a href=\"https://bsky.app/profile/[did]\">",
        string mentionStringEnd = "</a>",
        string tagStringStart = "<a href=\"https://bsky.app/search?q=%23[tag]\">",
        string tagStringEnd = "</a>"
    )
    {
        int facetBuffer = 0;

        if (facets != null)
        {
            foreach (var facet in facets)
            {
                foreach (var facetFeature in facet.Features)
                {
                    Type facetFeatureType = facetFeature.GetType();
                    ParsedFacet parsedFacet = new ParsedFacet();

                    switch (facetFeatureType)
                    {
                        case Type when facetFeatureType == typeof(RichtextFacetLink):
                            var linkFacet = facetFeature as RichtextFacetLink;

                            parsedFacet = ParseIndividualFacet(
                                originalString,
                                linkStringStart
                                    .Replace("[uri]", linkFacet.Uri.ToString()),
                                linkStringEnd
                                    .Replace("[uri]", linkFacet.Uri.ToString()),
                                facet.Index.ByteStart,
                                facet.Index.ByteEnd,
                                facetBuffer
                            );
                            break;

                        case Type when facetFeatureType == typeof(RichtextFacetMention):
                            var mentionFacet = facetFeature as RichtextFacetMention;

                            parsedFacet = ParseIndividualFacet(
                                originalString,
                                mentionStringStart
                                    .Replace("[did]", mentionFacet.Did),
                                mentionStringEnd
                                    .Replace("[did]", mentionFacet.Did),
                                facet.Index.ByteStart,
                                facet.Index.ByteEnd,
                                facetBuffer
                            );
                            break;

                        case Type when facetFeatureType == typeof(RichtextFacetTag):
                            var tagFacet = facetFeature as RichtextFacetTag;

                            parsedFacet = ParseIndividualFacet(
                                originalString,
                                tagStringStart
                                    .Replace("[tag]", tagFacet.Tag),
                                tagStringEnd
                                    .Replace("[tag]", tagFacet.Tag),
                                facet.Index.ByteStart,
                                facet.Index.ByteEnd,
                                facetBuffer
                            );
                            break;
                    }

                    originalString = parsedFacet.Text;
                    facetBuffer = facetBuffer + parsedFacet.Difference;
                }
            }
        }
        return originalString;
    }

    static Uri GetCdnUri(string did, string link, string mimeType)
    {
        // TODO: Is this the right way to do this?
        return new Uri($"https://cdn.bsky.app/img/feed_fullsize/plain/{did}/{link}@{mimeType.Split('/').Last()}");
    }

    static ParsedFacet ParseIndividualFacet(
        string originalString,
        string facetStartString,
        string facetEndString,
        int facetStart,
        int facetEnd,
        int facetBuffer
    )
    {
        ParsedFacet parsedFacet = new ParsedFacet();

        byte[] originalStringByteArray = Encoding.UTF8.GetBytes($"{originalString} ");
        List<byte> newStringByteList = new List<byte>();

        int charCount = 0;
        foreach (byte b in originalStringByteArray)
        {
            if (charCount == facetStart + facetBuffer)
                newStringByteList.AddRange(Encoding.UTF8.GetBytes(facetStartString));

            if (charCount == facetEnd + facetBuffer)
                newStringByteList.AddRange(Encoding.UTF8.GetBytes(facetEndString));

            newStringByteList.Add(b);
            charCount++;
        }

        parsedFacet.Difference = newStringByteList.Count() - originalStringByteArray.Count();
        parsedFacet.Text = Encoding.UTF8.GetString(newStringByteList.ToArray());

        return parsedFacet;
    }
}