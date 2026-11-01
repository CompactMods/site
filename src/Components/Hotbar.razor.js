const EDITABLE = "input, textarea, select, [contenteditable]:not([contenteditable='false'])";

// Wires up number-key slot selection and the scroll-progress XP bar, and publishes the space the hotbar takes at the
// bottom of the viewport as --hotbar-clearance so the layout can keep content above it. Returns a handle with dispose().
export function init(hud) {
  // The layout's scrolling content area, falling back to the window
  const scrollRoot = document.querySelector("[data-scroll-root]");
  const scrollTarget = scrollRoot ?? window;

  const onKeyDown = e => {
    if (e.repeat || e.defaultPrevented || e.ctrlKey || e.metaKey || e.altKey) return;
    if (!/^[1-9]$/.test(e.key) || e.target.closest?.(EDITABLE)) return;

    const slot = hud.querySelector(`a.hotbar-slot[data-slot="${e.key}"]`);
    if (!slot) return;

    e.preventDefault();
    slot.click();
  };

  // Pages too short to scroll count as fully read
  const updateXp = () => {
    const [top, max] = scrollRoot
      ? [scrollRoot.scrollTop, scrollRoot.scrollHeight - scrollRoot.clientHeight]
      : [window.scrollY, document.documentElement.scrollHeight - window.innerHeight];
    const progress = max > 0 ? Math.min(1, Math.max(0, top / max)) : 1;
    hud.style.setProperty("--xp", progress.toFixed(4));
  };

  const updateClearance = () => {
    const clearance = Math.max(0, window.innerHeight - hud.getBoundingClientRect().top);
    document.documentElement.style.setProperty("--hotbar-clearance", `${Math.ceil(clearance)}px`);
  };

  const onResize = () => {
    updateClearance();
    updateXp();
  };

  // Content loads asynchronously (e.g. wiki pages), so track size changes as well as scrolling
  const resizeObserver = new ResizeObserver(onResize);
  resizeObserver.observe(hud);
  resizeObserver.observe(scrollRoot?.firstElementChild ?? document.body);
  document.addEventListener("keydown", onKeyDown);
  scrollTarget.addEventListener("scroll", updateXp, { passive: true });
  window.addEventListener("resize", onResize);
  onResize();

  return {
    dispose() {
      resizeObserver.disconnect();
      document.removeEventListener("keydown", onKeyDown);
      scrollTarget.removeEventListener("scroll", updateXp);
      window.removeEventListener("resize", onResize);
      document.documentElement.style.removeProperty("--hotbar-clearance");
    },
  };
}
