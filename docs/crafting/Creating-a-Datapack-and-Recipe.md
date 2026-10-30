## About Datapacks
Datapacks are Minecraft's way to add and remove data and features from the game while it's running. You can find the [wiki page here](https://minecraft.fandom.com/wiki/Data_Pack) if you want more information, but this page will go over the basics.

1. Create a new folder inside your world save folder, under `datapacks`. You can name it anything, and it will show up in MC that way. The rest of the steps will use `mydata` as a placeholder.
2. Under the `mydata` folder, create a `pack.mcmeta` file (named exactly that) and insert the text below.
3. Create a `data` folder under `mydata`.
4. Create a `NAMESPACE` folder under `data`. Replace `NAMESPACE` with your mod ID or something unique (like `cclaustrophia`) as this will become the recipe's namespace.
4. Create a `recipes` folder under `mydata`.

The file structure should look like this when you're done:

![](https://i.imgur.com/24YUn8d.png)

### pack.mcmeta
*(From [Minecraft Wiki](https://minecraft.fandom.com/wiki/Data_Pack#pack.mcmeta))*

```json
{
    "pack": {
        "pack_format": 6,
        "description": "The default data for Minecraft"
    }
}
```

## Creating a recipe
Once you have a basic datapack structure, you can now follow the recipe specification to add any recipes you like (not just Compact Crafting recipes, either!). For that, go to the [Recipe Specification](Recipe-Specification) page. In short, create a new file called `myrecipe.json` (name it something unique) and place it under `recipes` in the pack structure above.