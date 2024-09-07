using Booski.Lib.Internal.AppBsky.Common;
using AppBskyCommon = Booski.Lib.Internal.AppBsky.Common;
using AppBskyPolymorphs = Booski.Lib.Polymorphs.AppBsky;
using ComAtprotoCommon = Booski.Lib.Internal.ComAtproto.Common;

namespace Booski.Lib.Records {
    public static class BskyRecords {
        public static AppBskyPolymorphs.FeedPost BskyPost(
            string text,
            DateTime? createdAt = null,
            List<Facet> facets = null,
            string[] langs = null
        ) {
            AppBskyPolymorphs.FeedPost post = new AppBskyPolymorphs.FeedPost();

            createdAt = createdAt == null ? DateTime.Now : createdAt;
            text = text == null ? "" : text;

            post.CreatedAt = (DateTime)createdAt;
            post.Facets = facets;
            post.Langs = langs;
            post.Text = text;

            return post;
        }

        public static AppBskyPolymorphs.FeedPost BskyPostWithImagesEmbed(
            string text,
            List<AppBskyCommon.Image> images,
            DateTime? createdAt = null,
            List<Facet> facets = null,
            string[] langs = null
        ) {
            AppBskyPolymorphs.FeedPost post = new AppBskyPolymorphs.FeedPost();

            createdAt = createdAt == null ? DateTime.Now : createdAt;
            text = text == null ? "" : text;

            var imageEmbedPoly = new AppBskyPolymorphs.EmbedImages {
                Images = images
            };

            post.CreatedAt = (DateTime)createdAt;
            post.Embed = imageEmbedPoly;
            post.Facets = facets;
            post.Langs = langs;
            post.Text = text;

            return post;
        }

        public static AppBskyPolymorphs.FeedPost BskyPostWithExternalEmbed(
            string text,
            AppBskyCommon.External external,
            DateTime? createdAt = null,
            ComAtprotoCommon.FileBlob externalThumb = null,
            List<Facet> facets = null,
            string[] langs = null
        ) {
            AppBskyPolymorphs.FeedPost post = new AppBskyPolymorphs.FeedPost();

            createdAt = createdAt == null ? DateTime.Now : createdAt;
            text = text == null ? "" : text;

            external.Thumb = externalThumb == null ? external.Thumb : externalThumb;

            var externalEmbedPoly = new AppBskyPolymorphs.EmbedExternal {
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