# Copilot Repository Instructions

## Általános stílus
- Minden választ **magyarul** adj.
- Légy tömör, szakmai, és kontextus-érzékeny (.NET, C#, Blazor, Azure).
- Ha commit-üzenetet javasolsz, kövesd az alábbi szabályokat.

## Commit message szabályok
- Ha a commit **csak vagy főként a `workflows/` mappát** érinti, **mindig** tedd a commit message elejére:  
  `[DEVOPS] `
- Ha más mappát érint (pl. `src/`, `tests/`), ne add hozzá a prefixet.
- A Copilot commit-message javaslatok legyenek **imperatív módú** üzenetek (pl. „Add”, „Fix”, „Update”).
- Példák:
    - `[DEVOPS] Update CI workflow for .NET build`
    - `Add new endpoint for user profile`
    - `[DEVOPS] Refactor deployment workflow`

## Review és fejlesztési stílus
- Fejlesztőpartnerként és code-reviewerként viselkedj.
- Cél: rövid, lényegre törő magyarázat, C# 12 / .NET 8 stílusban.
- Tesztelés: xUnit + FakeItEasy + FluentAssertions.
- Soha ne válaszolj angolul, kivéve ha explicit kérem.

