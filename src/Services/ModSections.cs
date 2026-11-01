using System.Collections.Immutable;

namespace Web.Services
{
    /// <summary>
    /// A top-level site section for one mod, reachable at "/{Slug}". Its wiki pages ("/wiki/{WikiProject}/...") count as
    /// part of the section too.
    /// </summary>
    /// <param name="Image">Promo image; also the faded page background throughout the section.</param>
    /// <param name="ItemImage">Pixel art shown in the section's hotbar slot; <paramref name="ItemIcon"/> is used when null.</param>
    /// <param name="ItemIcon">Font Awesome classes for the hotbar slot when there's no item image.</param>
    /// <param name="ItemColor">CSS colour for <paramref name="ItemIcon"/>.</param>
    public record ModSection(
        string Slug,
        string Name,
        string? Tagline,
        string? Description,
        string Image,
        string Curseforge,
        string? WikiProject,
        string? ItemImage = null,
        string? ItemIcon = null,
        string? ItemColor = null);

    public static class ModSections
    {
        public static readonly ImmutableArray<ModSection> All =
        [
            new("machines", "Compact Machines",
                Tagline: "Building things in small places!",
                Description: "Build entire factories inside pocket dimensions, then carry them around as one tidy machine.",
                Image: "media/promo-compact-machines.png",
                Curseforge: "https://www.curseforge.com/minecraft/mc-mods/compact-machines",
                WikiProject: "machines",
                ItemImage: "personal-shrinking-device.png"),

            new("crafting", "Compact Crafting",
                Tagline: "Crafting just got smaller.",
                Description: "Miniaturization recipes built from layered, in-world structures, definable with datapacks.",
                Image: "media/promo-crafting.png",
                Curseforge: "https://www.curseforge.com/minecraft/mc-mods/compact-crafting",
                WikiProject: "crafting",
                ItemImage: "media/projector.webp"),

            new("honey", "Simple Honey",
                Tagline: "Because automating vanilla honey is anything but simple.",
                Description: null,
                Image: "media/promo-simple-honey.png",
                Curseforge: "https://www.curseforge.com/minecraft/mc-mods/simple-honey",
                WikiProject: null,
                ItemImage: "media/honey_drop.png"),
        ];

        /// <summary>Background for pages that don't belong to a section.</summary>
        public static ModSection Default => All[0];

        /// <summary>Finds the section a base-relative path belongs to, e.g. "crafting" or "wiki/machines/cm5/home".</summary>
        public static ModSection? FromPath(string path)
        {
            var segments = path.Split(['?', '#'], 2)[0].Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                return null;

            if (segments[0].Equals("wiki", StringComparison.OrdinalIgnoreCase))
            {
                return segments.Length > 1
                    ? All.FirstOrDefault(s => string.Equals(s.WikiProject, segments[1], StringComparison.OrdinalIgnoreCase))
                    : null;
            }

            return All.FirstOrDefault(s => s.Slug.Equals(segments[0], StringComparison.OrdinalIgnoreCase));
        }
    }
}
