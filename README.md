# MinimalIdentityUI
A "prettier" replacement for .NET built-in Identity UI

## Development
To build tailwind locally and watch for changes in Pages and Areas execute in the root of the web project.
```
npm install
npm run tailwind
``` 

The source file is `wwwroot\css\site.css` and the output file is `wwwroot\dist\all.css`.

## Build for Production

The project has a build target that will execute a NPM script to build and minify the css file.
```
npm run tailwind:build
```
to compile the CSS on build.

