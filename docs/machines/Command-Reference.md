
# Eject
Allows a player to escape a room they're trapped in, or allows an op to eject a trapped player.

```console
compactmachines eject [[Player]]
```

# Give
Gives players a new, pre-bound machine block that can be used to enter a room. Requires OP by default.

```console
compactmachines give [Player] [room xz]
```

# Re-Add Dimension
Allows a server operator to forcibly register the compact world dimension into the current server level file. This command should make a backup before modify the `level.dat` file, but it's always good to make a full backup before running.

```console
compactmachines registerdim
```


# Rebind
Allows an operator to change which room a machine block is bound to. Only works if the machine does not have any tunnels tied to it, and the new room matches the block's size.

```console
compactmachines rebind [pos xyz] [room xz]
```


# Rooms
The rooms command allows players and operators to determine room coordinates for other commands. There are several formats:

```console
compactmachines rooms machblock [pos xyz]
compactmachines rooms findplayer [Player]
compactmachines rooms ownedby [Player]
```

`machblock` is useful to figure out which room a machine is tied to. Must be used in the dimension the machine block is in.

`findplayer` can be used to determine which room a player is currently in.

`ownedby` can be used to get a list of rooms a player owns. This will eventually change to a searchable GUI.


# Spawn
Allows for spawnpoint management of rooms. Requires operator.

```console
compactmachines spawn reset [room xz]
```