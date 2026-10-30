**Proxies** are redstone-automatable blocks designed to simplify automation with a miniaturization field. Craft the type you want and then right-click the proxy item on an active field projector to bind it. Once placed down, breaking the block or destabilizing the field (ie by breaking a projector) will unlink the proxy from the field.

---

### Match Proxy
**Match** proxies are used to read recipe state from an active field. They give full redstone signal (15) when a recipe is matched or actively crafting inside the field.

### Rescan Proxy
**Rescan** proxies are used to trigger a forced *rescan* on a field. Miniaturization fields sometimes fail to pick up on changes from automation-style block placers, so you can use these proxies to trigger a scan on a field. They're especially helpful on a hands-free timer setup, for easy automation of highly-used resources!