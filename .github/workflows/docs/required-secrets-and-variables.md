# Szükséges GitHub Secrets és Variables

## GitHub Secrets (Settings → Secrets and variables → Actions)

| Secret neve         | Leírás                                                                 |
|---------------------|------------------------------------------------------------------------|
| `AZURE_CREDENTIALS` | Azure Service Principal JSON (`az ad sp create-for-rbac --sdk-auth`)  |
| `AZURE_LOCATION`    | Azure régió, pl. `westeurope`                                          |
| `SQL_ADMIN_USER`    | SQL Server admin felhasználónév                                        |
| `SQL_ADMIN_PASS`    | SQL Server admin jelszó                                                |
| `GHCR_PAT`          | GitHub Personal Access Token (`read:packages` scope elég)             |

## GitHub Environments (Settings → Environments)

Létre kell hozni: `dev` és `prod`

## Service Principal létrehozása

```bash
az ad sp create-for-rbac \
  --name "profilplusz-oms-deploy" \
  --role Contributor \
  --scopes /subscriptions/<SUBSCRIPTION_ID> \
  --sdk-auth
```

A kimenet JSON-t másold be az `AZURE_CREDENTIALS` secretbe.

## GHCR_PAT létrehozása

GitHub → Settings → Developer settings → Personal access tokens → Fine-grained tokens  
Scope: `read:packages`

## Deployment sorrend (friss környezethez)

1. **01 - Infrastructure Provisioning** → provision Azure resources
2. **02 - Build and Push Docker Image** → build + push to GHCR
3. **03 - Deploy to Azure Web App** → automatikusan indul, vagy manuálisan

## Workflow áttekintés

| Fájl                    | Mit csinál                                      | Trigger              |
|-------------------------|-------------------------------------------------|----------------------|
| `infrastructure.yml`    | Resource group, SQL Server, App Service Plan    | Manuális             |
| `build-and-push.yml`    | Docker build + push GHCR-re, GitHub tag         | Manuális             |
| `deploy.yml`            | Azure Web App deploy GHCR image-ből             | Auto (build után) / Manuális |
| `destroy.yml`           | Teljes Azure infrastruktúra törlése             | Manuális             |
| `cleanup-old-runs.yml`  | Régi workflow run-ok törlése                    | Naponta 03:00 UTC    |
| `azure-login-test.yml`  | Azure login diagnosztika                        | Manuális             |
