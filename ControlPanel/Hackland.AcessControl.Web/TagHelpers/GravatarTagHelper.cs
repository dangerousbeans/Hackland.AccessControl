using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.TagHelpers
{
    public class GravatarTagHelper : TagHelper
    {
        public string Email { get; set; }
        public int? Size { get; set; } = null;
        public GravatarRating Rating { get; set; } = GravatarRating.Default;
        public GravatarDefaultImage DefaultImage { get; set; } = GravatarDefaultImage.MysteryMan;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";

            output.Attributes.SetAttribute("alt", Email + " gravatar");

            var url = new StringBuilder("//www.gravatar.com/avatar/", 90);
            url.Append(GetEmailHash(Email));

            var isFirst = true;
            Action<string, string> addParam = (p, v) =>
            {
                url.Append(isFirst ? '?' : '&');
                isFirst = false;
                url.Append(p);
                url.Append('=');
                url.Append(v);
            };

            if (Size != null)
            {
                if (Size < 1 || Size > 512)
                    throw new ArgumentOutOfRangeException("size", Size,
                          "Must be null or between 1 and 512, inclusive.");
                addParam("s", Size.Value.ToString());
            }

            if (Rating != GravatarRating.Default)
                addParam("r", Rating.ToString().ToLower());

            if (DefaultImage != GravatarDefaultImage.Default)
            {
                if (DefaultImage == GravatarDefaultImage.Http404)
                    addParam("d", "404");
                else if (DefaultImage == GravatarDefaultImage.Identicon)
                    addParam("d", "identicon");
                if (DefaultImage == GravatarDefaultImage.MonsterId)
                    addParam("d", "monsterid");
                if (DefaultImage == GravatarDefaultImage.MysteryMan)
                    addParam("d", "mm");
                if (DefaultImage == GravatarDefaultImage.Wavatar)
                    addParam("d", "wavatar");
            }

            output.Attributes.SetAttribute("src", url.ToString());

            if (Size != null)
            {
                output.Attributes.SetAttribute("width", Size.ToString());
                output.Attributes.SetAttribute("height", Size.ToString());
            }
        }

        private static string GetEmailHash(string email)
        {
            if (email == null)
                return new string('0', 32);

            email = email.Trim().ToLower();

            var emailBytes = Encoding.ASCII.GetBytes(email);
            var hashBytes = new MD5CryptoServiceProvider()
                                                .ComputeHash(emailBytes);

            var hash = new StringBuilder();
            foreach (var b in hashBytes)
                hash.Append(b.ToString("x2"));

            return hash.ToString();
        }
    }

    public enum GravatarDefaultImage
    {
        /// 
        /// The default value image. That is, the image returned
        /// when no specific default value is included
        /// with the request.
        /// At the time of authoring, this image is the Gravatar icon.
        /// 
        Default,

        /// 
        /// Do not load any image if none is associated with the email
        /// hash, instead return an HTTP 404 (File Not Found) response.
        /// 
        Http404,

        /// 
        /// A simple, cartoon-style silhouetted outline of a person
        /// (does not vary by email hash).
        /// 
        MysteryMan,

        /// 
        /// A geometric pattern based on an email hash.
        /// 
        Identicon,

        /// 
        /// A generated 'monster' with different colors, faces, etc.
        /// 
        MonsterId,

        /// 
        /// Generated faces with differing features and backgrounds.
        /// 
        Wavatar
    }

    public enum GravatarRating
    {
        /// 
        /// The default value as specified by the Gravatar service.
        /// That is, no rating value is specified
        /// with the request. At the time of authoring,
        /// the default level was <see cref="G"/>.
        /// 
        Default,

        /// 
        /// Suitable for display on all websites with any audience type.
        /// This is the default.
        /// 
        G,

        /// 
        /// May contain rude gestures, provocatively dressed individuals,
        /// the lesser swear words, or mild violence.
        /// 
        Pg,

        /// 
        /// May contain such things as harsh profanity, intense violence,
        /// nudity, or hard drug use.
        /// 
        R,

        /// 
        /// May contain hardcore sexual imagery or 
        /// extremely disturbing violence.
        /// 
        X
    }
}
