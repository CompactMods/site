const MERMAID_URL = "https://cdn.jsdelivr.net/npm/mermaid@12.2.0/dist/mermaid.esm.min.mjs";

let mermaidLoad;

function loadMermaid() {
  mermaidLoad ??= import(MERMAID_URL).then(({ default: mermaid }) => {
    mermaid.initialize({ startOnLoad: false, theme: "neutral", securityLevel: "strict" });
    return mermaid;
  });
  return mermaidLoad;
}

// Highlights code and renders Mermaid diagrams inside a rendered markdown container.
export async function render(container) {
  if (!container) return;

  // Markdig.Prism emits ```mermaid fences as <pre><code class="language-mermaid">. Convert them in place,
  // keeping the <pre> itself so Blazor's tracked markup nodes stay intact.
  const diagrams = [];
  for (const code of container.querySelectorAll("pre > code.language-mermaid")) {
    const pre = code.parentElement;
    pre.textContent = code.textContent;
    pre.classList.add("mermaid");
    diagrams.push(pre);
  }

  window.Prism?.highlightAllUnder(container);

  if (diagrams.length === 0) return;

  try {
    const mermaid = await loadMermaid();
    await mermaid.run({ nodes: diagrams, suppressErrors: true });
  } catch (e) {
    // Mermaid couldn't load; fall back to showing the diagram source
    mermaidLoad = undefined;
    diagrams.forEach(pre => pre.classList.replace("mermaid", "mermaid-source"));
    console.error("Failed to render Mermaid diagrams", e);
  }
}
