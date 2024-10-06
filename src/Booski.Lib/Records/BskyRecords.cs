using Booski.Lib.Internal.AppBsky.Common;
using AppBskyCommon = Booski.Lib.Internal.AppBsky.Common;
using AppBskyLexicons = Booski.Lib.Lexicons.AppBsky;
using ComAtprotoCommon = Booski.Lib.Internal.ComAtproto.Common;

// TODO: Move to Helpers

namespace Booski.Lib.Records {
    public static class BskyRecords {
        public static AppBskyLexicons.FeedPost BskyPost(
            string text,
            DateTime? createdAt = null,
            List<Facet> facets = null,
            string[] langs = null
        ) {
            AppBskyLexicons.FeedPost post = new AppBskyLexicons.FeedPost();

            createdAt = createdAt == null ? DateTime.UtcNow : createdAt;
            text = text == null ? "" : text;

            post.CreatedAt = (DateTime)createdAt;
            post.Facets = facets;
            post.Langs = langs;
            post.Text = text;

            return post;
        }

        public static AppBskyLexicons.FeedPost BskyPostWithImagesEmbed(
            string text,
            List<AppBskyCommon.Image> images,
            DateTime? createdAt = null,
            List<Facet> facets = null,
            string[] langs = null
        ) {
            AppBskyLexicons.FeedPost post = new AppBskyLexicons.FeedPost();

            createdAt = createdAt == null ? DateTime.UtcNow : createdAt;
            text = text == null ? "" : text;

            var imageEmbedPoly = new AppBskyLexicons.EmbedImages {
                Images = images
            };

            post.CreatedAt = (DateTime)createdAt;
            post.Embed = imageEmbedPoly;
            post.Facets = facets;
            post.Langs = langs;
            post.Text = text;

            return post;
        }

        public static AppBskyLexicons.FeedPost BskyPostWithExternalEmbed(
            string text,
            AppBskyCommon.External external,
            DateTime? createdAt = null,
            ComAtprotoCommon.FileBlob externalThumb = null,
            List<Facet> facets = null,
            string[] langs = null
        ) {
            AppBskyLexicons.FeedPost post = new AppBskyLexicons.FeedPost();

            createdAt = createdAt == null ? DateTime.UtcNow : createdAt;
            text = text == null ? "" : text;

            external.Thumb = externalThumb == null ? external.Thumb : externalThumb;

            var externalEmbedPoly = new AppBskyLexicons.EmbedExternal {
                External = external
            };

            post.CreatedAt = (DateTime)createdAt;
            post.Embed = externalEmbedPoly;
            post.Facets = facets;
            post.Langs = langs;
            post.Text = text;

            return post;
        }
    }
}