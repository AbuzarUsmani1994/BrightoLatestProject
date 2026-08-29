# Brighto (FOS.Web.UI) - Agent Workflow

This project uses 4 dedicated subagents (defined in `.claude/agents/`). For any coding
task in this repo, route the work through these agents rather than writing code directly
in the main thread:

1. **sql-agent** - stored procedures, views, schema scripts (SQL Server, DB `Brighto` on
   local instance `ABUZAR`; prod at `116.58.33.11`). Run this first if the task touches
   data shape.
2. **backend-agent** - ASP.NET MVC 5 / EF6 controllers, `Setup/Manage*.cs` business logic,
   `Shared/*.cs` DTOs. Run after sql-agent so it can match the proc's exact output
   columns.
3. **frontend-agent** - Razor views (.cshtml), Bootstrap 2 markup, jQuery/DataTables/AJAX.
   Run after backend-agent so it can match the exact route/params/JSON shape.
4. **qa-reviewer-agent** - senior QA / code review pass. **Always run this last**, after
   any combination of the above have made changes, before telling the user the task is
   done. It reviews only (no edits) and reports blocking vs non-blocking issues; route
   any fixes it finds back to the relevant agent(s) above.

## Typical flow for a feature/bugfix touching multiple layers
sql-agent -> backend-agent -> frontend-agent -> qa-reviewer-agent -> (fix loop if QA
finds blocking issues) -> report to user.

For single-layer tasks (e.g. "fix this CSS", "add a column to this proc"), invoke only
the relevant specialist agent, then still run qa-reviewer-agent before reporting done.

## Stack summary (see individual agent files for full conventions)
- ASP.NET MVC 5, .NET Framework, old-style .csproj (new files must be added to
  `<Compile Include>` manually).
- EF6 + .edmx, but new/evolving stored procedures are called via raw ADO.NET
  (`SqlConnection`/`SqlCommand`) against `dbContext.Database.Connection.ConnectionString`
  rather than EF function imports, to avoid touching the fragile .edmx.
- Bootstrap 2.x views (`row`/`span*` grid, not `col-*`; `container` has a fixed
  max-width - a common source of overflow bugs).
- Build via MSBuild through the **PowerShell tool**, not Bash.
- SQL scripts live under `SQL/`, use `CREATE OR ALTER PROCEDURE`, applied to local dev
  via `sqlcmd -S ABUZAR -d Brighto -E -i "<script>"`, and still need manual deployment to
  production (`116.58.33.11`) after being verified locally.
