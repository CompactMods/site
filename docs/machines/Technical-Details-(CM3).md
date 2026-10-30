* The inside of Compact Machines are rooms in another dimension (id=144 by default)
* The rooms in the other dimension are automatically chunk-loaded when the Compact Machine itself is
    chunkloaded
* They only trigger chunk reloads when a block next to one of the tunnels changes. There is also no redstone
    signals being transferred between the tunnels and the machine block.
* There are two kinds of Compact Machine Wall blocks. The actual machines are made out of unbreakable blocks
    (you can still break them in creative mode while holding a Personal Shrinking Device), while the machine
    structures you build for crafting are made out of breakable Wall blocks.
* Right clicking a Compact Machine renders the contents of the machine in a GUI. On multiplayer games this is
    handled by sending the chunk contents to the client every time he opens the GUI. There is currently no
    auto-refresh of the machines content.
* Compact Machines can be nested
* Compact Machines spawn along the x-axis in the Compact Machine dimension at y=40, x=1024*machine-id, z=0.
    This means they all fit in exactly one chunk.
* Currently mobs can not spawn within Compact Machines. This will probably change in the future, but there
    will be a config option to turn this back off.
* Players can not leave the machine block they should currently be in according to the last one they've
    entered using a Personal Shrinking Device. Trying to enter a Compact Machines with other means results in
    the player being thrown out of the machine and getting some bad status effect applied. They might even die.

* If players die without a bed in the CM dimension or with the "allowRespawning" config option disabled,
    they'll be teleported out of the machine dimension.