using System.Net;
using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace Web.Wiki
{
    /// <summary>Maps a wiki link target (a page name) to a URL, or null when no page matches.</summary>
    public delegate string? WikiLinkResolver(string target);

    /// <summary>
    /// GitHub wiki style links: <c>[[Page Name]]</c> or <c>[[Link text|Page Name]]</c>. Targets are resolved through a
    /// <see cref="WikiLinkResolver"/> passed in <see cref="MarkdownParserContext.Properties"/>; unresolved links render
    /// as marked-up plain text.
    /// </summary>
    public class WikiLinkExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<WikiLinkParser>())
                pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new WikiLinkParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        public static MarkdownParserContext CreateContext(WikiLinkResolver resolver)
        {
            var context = new MarkdownParserContext();
            context.Properties[typeof(WikiLinkResolver)] = resolver;
            return context;
        }
    }

    public static class WikiLinkPipelineExtensions
    {
        public static MarkdownPipelineBuilder UseWikiLinks(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<WikiLinkExtension>();
            return pipeline;
        }
    }

    public class WikiLinkParser : InlineParser
    {
        public WikiLinkParser()
        {
            OpeningCharacters = ['['];
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (slice.PeekChar() != '[')
                return false;

            if (processor.Context?.Properties.GetValueOrDefault(typeof(WikiLinkResolver)) is not WikiLinkResolver resolver)
                return false;

            var contentStart = slice.Start + 2;
            var searchLength = slice.End - contentStart + 1;
            if (searchLength < 2)
                return false;

            var contentEnd = slice.Text.IndexOf("]]", contentStart, searchLength, StringComparison.Ordinal);
            if (contentEnd <= contentStart)
                return false;

            var content = slice.Text[contentStart..contentEnd];
            if (content.AsSpan().IndexOfAny('[', '\n', '\r') >= 0)
                return false;

            var pipe = content.IndexOf('|');
            var label = (pipe < 0 ? content : content[..pipe]).Trim();
            var target = (pipe < 0 ? content : content[(pipe + 1)..]).Trim();
            if (label.Length == 0 || target.Length == 0)
                return false;

            var position = processor.GetSourcePosition(slice.Start, out var line, out var column);
            var url = resolver(target);

            Inline inline;
            if (url is null)
            {
                inline = new HtmlInline($"<span class=\"wiki-link-missing\" title=\"No page named '{WebUtility.HtmlEncode(target)}'\">{WebUtility.HtmlEncode(label)}</span>");
            }
            else
            {
                var link = new LinkInline(url, "") { IsClosed = true };
                link.AppendChild(new LiteralInline(label));
                inline = link;
            }

            inline.Line = line;
            inline.Column = column;
            inline.Span = new(position, processor.GetSourcePosition(contentEnd + 1));

            processor.Inline = inline;
            slice.Start = contentEnd + 2;
            return true;
        }
    }
}
