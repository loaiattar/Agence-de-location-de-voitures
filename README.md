# Agence de Location de Voitures

Application ASP.NET Core MVC de location de voitures avec persistance SQLite.

## Structure du projet

```
├── src/
│   ├── CarAgence.Domain/      # Entités métier
│   ├── CarAgence.Data/        # Contexte EF Core + SQLite
│   ├── CarAgence.Services/    # Logique métier
│   └── CarAgence.Web/         # Application web MVC
├── tests/
│   └── CarAgence.Tests/       # Tests unitaires
├── .github/workflows/         # CI/CD GitHub Actions
├── Dockerfile                 # Image Docker
└── docs/                      # Documentation
```

## Branche `main` protégée

La branche `main` est protégée :

- **Pas de push direct** — tous les changements passent par Pull Request
- **CI obligatoire** — le job `build-and-test` doit passer avant merge
- **1 approval requis** — au moins 1 revue obligatoire
- **Stale reviews** — les approvals sont révoquées si de nouveaux commits sont poussés

## Développement

```bash
# Restaurer les dépendances
dotnet restore

# Builder
dotnet build

# Tester
dotnet test
```
