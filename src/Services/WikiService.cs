using System.Collections.Frozen;
using System.Reflection;

namespace Web.Services
{
    /// <summary>
    /// Builds the wiki route table from the page index baked in at build time (see AddWikiIndex in src.csproj).
    /// Pages live in /docs/{project}/{path}.md, are served from wwwroot/wiki-pages, and route to "/wiki/{project}/{slug}".
    /// </summary>
    public class WikiService
    {
        private const string IndexKey = "WikiPage";
        private const string ContentRoot = "wiki-pages/";
        private const string Extension = ".md";

        private static readonly FrozenDictionary<string, WikiProject> Projects = BuildIndex();

        private readonly HttpClient _client;

        public WikiService(HttpClient client)
        {
            _client = client;
        }

        public WikiProject? GetProject(string project) => Projects.GetValueOrDefault(project);

        public async Task<string?> ReadPage(WikiPage page)
        {
            var response = await _client.GetAsync(page.ContentPath);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
        }

        private static FrozenDictionary<string, WikiProject> BuildIndex()
        {
            return typeof(WikiService).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(a => a.Key == IndexKey && a.Value is not null && a.Value.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Value!.Replace('\\', '/').Split('/', 2))
                .Where(parts => parts.Length == 2)
                .GroupBy(parts => parts[0], StringComparer.OrdinalIgnoreCase)
                .ToFrozenDictionary(
                    g => g.Key,
                    g => new WikiProject(g.Key, g
                        .Select(parts => CreatePage(parts[0], parts[1]))
                        .ToFrozenDictionary(p => p.Slug, StringComparer.OrdinalIgnoreCase)),
                    StringComparer.OrdinalIgnoreCase);
        }

        private static WikiPage CreatePage(string project, string relativePath)
        {
            var path = relativePath[..^Extension.Length];
            var slashIndex = path.LastIndexOf('/');
            var folder = slashIndex < 0 ? null : path[..slashIndex];
            var name = path[(slashIndex + 1)..];
            var contentPath = ContentRoot + string.Join('/', $"{project}/{relativePath}".Split('/').Select(Uri.EscapeDataString));

            return new WikiPage(
                Slug: path.Replace(' ', '-').ToLowerInvariant(),
                Title: name.Replace('-', ' '),
                Folder: folder,
                ContentPath: contentPath);
        }

        public record WikiProject(string Slug, FrozenDictionary<string, WikiPage> Pages);

        public record WikiPage(string Slug, string Title, string? Folder, string ContentPath);
    }
}
