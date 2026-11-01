using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web.Services
{
    /// <summary>
    /// Builds the wiki route table from the page index baked in at build time (see AddWikiIndex in Web.csproj).
    /// Pages live in /docs/{project}/{path}.md and are served from wwwroot/wiki-pages.
    /// <para>
    /// Projects listed in /docs/versions.json are versioned: a top-level folder named after a version slug holds that
    /// version's pages, routed to "/wiki/{project}/{version}/{slug}". Pages outside any version folder are global:
    /// every version includes them. Projects without an entry route to "/wiki/{project}/{slug}".
    /// </para>
    /// </summary>
    public class WikiService
    {
        private const string IndexKey = "WikiPage";
        private const string VersionsResource = "wiki-versions.json";
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
            var versions = LoadVersions();

            return typeof(WikiService).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(a => a.Key == IndexKey && a.Value is not null && a.Value.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Value!.Replace('\\', '/').Split('/', 2))
                .Where(parts => parts.Length == 2)
                .GroupBy(parts => parts[0], StringComparer.OrdinalIgnoreCase)
                .ToFrozenDictionary(
                    g => g.Key,
                    g => CreateProject(g.Key, g.Select(parts => parts[1]).ToList(), versions.GetValueOrDefault(g.Key)),
                    StringComparer.OrdinalIgnoreCase);
        }

        private static FrozenDictionary<string, ProjectVersionsConfig> LoadVersions()
        {
            using var stream = typeof(WikiService).Assembly.GetManifestResourceStream(VersionsResource);
            if (stream is null)
                return FrozenDictionary<string, ProjectVersionsConfig>.Empty;

            var config = JsonSerializer.Deserialize(stream, WikiJsonContext.Default.DictionaryStringProjectVersionsConfig);
            return (config ?? []).ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        }

        private static WikiProject CreateProject(string project, List<string> paths, ProjectVersionsConfig? config)
        {
            if (config?.Versions is not { Length: > 0 } versionConfigs)
            {
                var unversioned = new WikiVersion(null, project, null, CreatePages(project, paths.Select(p => (p, p, false))));
                return new WikiProject(project, [unversioned], unversioned);
            }

            var slugs = versionConfigs.Select(v => v.Slug).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var shared = paths
                .Where(p => p.IndexOf('/') is var i && (i < 0 || !slugs.Contains(p[..i])))
                .Select(p => (p, p, true));

            var versions = versionConfigs
                .Select(v =>
                {
                    var prefix = v.Slug + "/";
                    var own = paths
                        .Where(p => p.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        .Select(p => (p, p[prefix.Length..], false));

                    // A version's own page wins over a shared page with the same slug
                    return new WikiVersion(v.Slug, v.Name, v.Minecraft, CreatePages(project, shared.Concat(own)));
                })
                .ToImmutableArray();

            var defaultVersion = versions.FirstOrDefault(v => string.Equals(v.Slug, config.Default, StringComparison.OrdinalIgnoreCase)) ?? versions[0];
            return new WikiProject(project, versions, defaultVersion);
        }

        private static FrozenDictionary<string, WikiPage> CreatePages(string project, IEnumerable<(string FilePath, string PagePath, bool IsGlobal)> files)
        {
            var pages = new Dictionary<string, WikiPage>(StringComparer.OrdinalIgnoreCase);
            foreach (var (filePath, pagePath, isGlobal) in files)
            {
                var page = CreatePage(project, filePath, pagePath, isGlobal);
                pages[page.Slug] = page;
            }

            return pages.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        }

        /// <param name="filePath">Path of the file within the project folder.</param>
        /// <param name="pagePath">Path of the page within its version, which forms the slug and folder.</param>
        /// <param name="isGlobal">Whether the page sits outside any version folder and so appears in every version.</param>
        private static WikiPage CreatePage(string project, string filePath, string pagePath, bool isGlobal)
        {
            var path = pagePath[..^Extension.Length];
            var slashIndex = path.LastIndexOf('/');
            var folder = slashIndex < 0 ? null : path[..slashIndex];
            var name = path[(slashIndex + 1)..];
            var contentPath = ContentRoot + string.Join('/', $"{project}/{filePath}".Split('/').Select(Uri.EscapeDataString));

            return new WikiPage(
                Slug: path.Replace(' ', '-').ToLowerInvariant(),
                Title: name.Replace('-', ' '),
                Folder: folder,
                ContentPath: contentPath,
                IsGlobal: isGlobal);
        }

        /// <param name="Versions">Versions in versions.json order; a single unversioned entry when the project isn't versioned.</param>
        public record WikiProject(string Slug, ImmutableArray<WikiVersion> Versions, WikiVersion DefaultVersion)
        {
            public bool IsVersioned => DefaultVersion.Slug is not null;

            public WikiVersion? GetVersion(string slug) =>
                Versions.FirstOrDefault(v => string.Equals(v.Slug, slug, StringComparison.OrdinalIgnoreCase));

            /// <summary>Finds the version a page lives in, preferring the default version.</summary>
            public WikiVersion? FindVersionWithPage(string pageSlug) =>
                DefaultVersion.Pages.ContainsKey(pageSlug) ? DefaultVersion : Versions.FirstOrDefault(v => v.Pages.ContainsKey(pageSlug));

            /// <summary>
            /// Resolves a wiki link target such as "Command Reference" to a page URL. Matches a full slug or a page name in
            /// any folder, looking in <paramref name="version"/> first and then the other versions.
            /// </summary>
            public string? ResolveLink(WikiVersion version, string target)
            {
                var slug = target.Trim().Replace(' ', '-').ToLowerInvariant();
                foreach (var v in Versions.Remove(version).Insert(0, version))
                {
                    var page = v.Pages.GetValueOrDefault(slug)
                               ?? v.Pages.Values.FirstOrDefault(p => p.Slug.EndsWith("/" + slug, StringComparison.OrdinalIgnoreCase));
                    if (page is not null)
                        return Url(v, page.Slug);
                }

                return null;
            }

            public string Url(WikiVersion version, string pageSlug) =>
                version.Slug is null ? $"/wiki/{Slug}/{pageSlug}" : $"/wiki/{Slug}/{version.Slug}/{pageSlug}";
        }

        /// <param name="Slug">Version folder name, or null for an unversioned project.</param>
        public record WikiVersion(string? Slug, string Name, string? Minecraft, FrozenDictionary<string, WikiPage> Pages)
        {
            public string Label => Minecraft is null ? Name : $"{Name} (MC {Minecraft})";
        }

        /// <param name="IsGlobal">True for pages of a versioned project that live outside the version folders.</param>
        public record WikiPage(string Slug, string Title, string? Folder, string ContentPath, bool IsGlobal);
    }

    internal record ProjectVersionsConfig(string? Default, VersionConfig[]? Versions);

    internal record VersionConfig(string Slug, string Name, string? Minecraft);

    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true)]
    [JsonSerializable(typeof(Dictionary<string, ProjectVersionsConfig>))]
    internal partial class WikiJsonContext : JsonSerializerContext;
}
