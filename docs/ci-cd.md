# CI/CD Pipeline

## Règles de branche

| Branche | Push direct | PR requise | CI obligatoire |
|---------|-------------|------------|----------------|
| `main` | Non | Oui | Oui |
| `develop` | Oui | Non | Oui |
| `feature/*` | Oui | Non | Oui |

## Processus de travail

1. Créer une branche `feature/xxx` depuis `develop`
2. Travailler et committer sur la branche feature
3. Pousser et créer une Pull Request vers `develop`
4. La CI s'exécute automatiquement
5. Après revue et CI verte, merger la PR

## Pipeline GitHub Actions

### Job 1 : build-and-test
- Restore des dépendances .NET
- Build en mode Release
- Exécution des tests unitaires avec couverture
- Upload des résultats (.trx) et couverture (.cobertura.xml)

### Job 2 : docker-build (après build-and-test)
- Lowercase du nom de repository pour compatibilité GHCR
- Construction de l'image Docker (multi-stage, Buildx)
- Scan de sécurité avec Trivy (CRITICAL/HIGH)
- Upload des résultats SARIF vers GitHub Security
- Push vers ghcr.io uniquement sur `main`

### Déclencheurs
- Push sur `main`, `develop`, `feature/*`
- Pull Request vers `main` ou `develop`

### Sécurité
- Scan Trivy : CRITICAL et HIGH bloquent (continue-on-error temporaire)
- Aucun secret dans le code
- Image construite avec utilisateur non-root (appuser)
- Permissions : `packages:write`, `security-events:write`

## Limites du pipeline GitHub

Le pipeline GitHub ne déploie pas sur minikube car :
- minikube tourne sur votre machine locale
- Les runners GitHub n'ont pas accès à votre cluster
- Le déploiement local est déclenché par Terraform + Ansible

## Commandes utiles

```bash
# Vérifier le statut du pipeline
gh run list

# Voir les détails d'un run
gh run view <run-id>

# Télécharger les artifacts
gh run download <run-id>
```
