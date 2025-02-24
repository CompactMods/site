# Miniaturization Crafting
This is a crafting mechanic added to create the Compact Machine blocks and some of the utility blocks and items it provides. This can also be easily extended by pack makers for custom recipes.

## Setup
You will require 4 Miniaturization Field Projectors placed in a cross shape creating a odd-sized cube structure. Right click a Field Project to let it tell you where you need to place the next projector - missing locations will be highlighted in the world. You might have to dig out the floor or place the projectors on pedestals.

Once you placed the projectors correctly and no blocks are obstructing the miniaturization field, the field should visualize.

Watch [this video on YouTube](https://www.youtube.com/watch?v=p-F8ScV3z4U) for a basic crafting example:

![](https://camo.githubusercontent.com/c0a944cedafdfafc1a67d82f986dbc0aed102346/68747470733a2f2f696d672e796f75747562652e636f6d2f76692f702d4638536356337a34552f302e6a7067)


You can disable the field projection by applying a redstone signal to any of the projectors.

## Creating custom recipes
Recipes are added by placing `.json` files in the `config/compactmachines3/recipes` folder.

All recipes in that folder are being loaded additionally to the recipes shipped within the jar file. If you want to disable a recipe simply create a recipe json file with the same filename and add a `"disabled": true` property to the recipe object.
You can also extract all recipes in the jar using the `/compactmachines3 recipe unpack-defaults` command.
Use the `/compactmachines3 recipe copy-shape` command to copy the shape in the field of the field projector you are looking at into your clipboard.
If you need to specify custom NBT data for the catalyst item, e.g. the following line would make the catalyst required to be enchanted with "Holding III":
```json
{ "catalyst-nbt": "{StoredEnchantments: [{lvl:3s,id: 11s}]}" }
```

The value is just the JSON encoding of the nbt tag, you can view those e.g. by enabling Advanced Tooltips (F3 + H) and running Actually Additions.
The rest of the format should be intuitive enough that it does not require further explanation. If it isn't feel free to open an issue.