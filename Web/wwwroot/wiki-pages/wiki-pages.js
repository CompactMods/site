// Importing File System module
const fs = require('fs')
const path = require("path");

const dir = process.argv[2] || process.cwd();

async function run() {
    let files = [];

    // The process.cwd() gives current
    // working directory
    let filenames = await fs.promises.readdir( dir );

    for (let filename of filenames) {
        if(!filename.endsWith(".md"))
            continue;

        files.push({
            original: filename,
            slug: filename.toLowerCase().substring(0, filename.length - 3)
        })
    }

    await fs.promises.writeFile(path.join(dir, "project.json"), JSON.stringify(files));
}

run();