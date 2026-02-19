# Repository Guidelines

## Project Structure & Module Organization
The main app lives in `src/` (`MinimalIdentityUI.csproj`, .NET 8 Razor Pages).
- `src/Areas/Identity/Pages/`: customized ASP.NET Identity UI pages (`.cshtml` + `.cshtml.cs`).
- `src/Pages/Shared/`: shared layouts and partials for app shell/navigation.
- `src/Data/`: `ApplicationDbContext` and EF Core migrations.
- `src/wwwroot/`: static assets (`css`, `js`, `dist/all.css`, vendor libs).
- `images/`: screenshots used in docs.
- `README.md`: usage and integration instructions for consumers.

## Build, Test, and Development Commands
Run commands from `src/` unless noted.
- `npm install`: install Tailwind dependencies.
- `npm run tailwind`: watch and rebuild `wwwroot/dist/all.css` during UI work.
- `npm run tailwind:build`: production/minified CSS build.
- `dotnet watch`: run app with hot reload for Razor/C# edits.
- `dotnet build MinimalIdentityUI.sln`: build project (also runs `tailwind:build` via MSBuild target).
- `dotnet test MinimalIdentityUI.sln`: execute tests (currently no dedicated test project).

## Coding Style & Naming Conventions
- Use standard C# conventions: 4-space indentation, `PascalCase` for types/methods, `camelCase` for locals/params.
- Keep Razor Page pairs aligned by name (`Login.cshtml` and `Login.cshtml.cs`).
- Prefer small, focused page-model methods and keep UI utility classes centralized (see `Areas/Identity/Pages/Classes.cs`).
- Avoid hand-editing generated or vendored files under `src/wwwroot/lib/`.

## Testing Guidelines
No standalone test project is committed yet. For contributions:
- Run `dotnet build` and verify app boots with `dotnet watch`.
- Manually validate changed Identity flows (login, register, reset password, 2FA, account management).
- If UI/layout changes, include before/after screenshots in your PR.

## Commit & Pull Request Guidelines
Recent history favors short, imperative commit subjects (for example, `Add Rate Limiting`, `Fix tabs formatting`).
- Keep commit subject concise and action-first.
- Group related code + docs updates in the same PR.
- PRs should include: purpose, impacted pages/files, manual test steps, and screenshots for visible UI changes.
- Link related issue(s) when applicable and call out config changes (`appsettings*.json`, rate-limiter settings).
