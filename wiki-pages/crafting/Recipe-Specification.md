The following page goes over the specifications for creating a new recipe for Miniaturization crafting. Here is a basic template:

```json
{
  "type": "compactcrafting:miniaturization",
  "recipeSize": 0,
  "layers": [ ],
  "catalyst": { },
  "components": { },
  "outputs": [ ]
}
```

## Type
This must be set to `compactcrafting:miniaturization` for the recipe system to pick up the recipe.

## Recipe Size (recipeSize)
If you specify a recipe that has no fixed-size layers (such as a `mixed` layer), then you need to tell the recipe system how large the recipe is. It's recommended to use an odd number, to match the field sizes (ie 3, 5, 7, etc). If you have a layer with a fixed size, you do not have to specify this property.

## Layers
This property is an array of layer definitions. See [the layer specs page](Recipe-Layer-Specification) for more information on how to define layers.

Example layer definition, using the [filled](Filled-Layer-Specification) layer type:
```jsonc
{
  "type": "compactcrafting:filled",
  "component": "C"
}
```

## Catalyst
This property is a JSON representation of the catalyst itemstack. It currently takes an item ID and a count, but the count will be set to 1 if it is anything different. This is planned to change in a future update -- it will eventually become possible to require a player to have, for example, a full stack of redstone to trigger a craft. 

**Note that `Count` is case sensitive; this is due to the JSON loader.**

```json
"catalyst": {
    "id": "minecraft:diamond",
    "Count": 1
}
```

## Components
Components are a means of specifying *something* inside a recipe. The most common component is the `block` component, which specifies information on how to match a block in the recipe. See an example below:

```jsonc
"components": {
  "C": {
    "type": "compactcrafting:block",
    "block": "minecraft:coal_block"
  }
}
```

## Outputs
Similar to the catalyst definition, this is an array of output item stacks that the recipe will create once the crafting process is completed.

Example (note `Count` is case sensitive):
```jsonc
{
  "id": "minecraft:diamond_block",
  "Count": 27
}

```