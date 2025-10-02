# Szükséges GitHub Secrets és Pipeline Változók az Azure infrastruktúra pipeline-hoz

Az alábbi táblázat összefoglalja, hogy milyen titkos kulcsokat (secrets) és pipeline inputokat kell létrehoznod a GitHub-on, illetve hol találod meg vagy hogyan generálhatod őket.

| Név                  | Típus      | Hol kell létrehozni / beállítani         | Hol találod meg / hogyan generálod? |
|----------------------|------------|------------------------------------------|-------------------------------------|
| AZURE_CREDENTIALS    | Secret     | GitHub repo: Settings > Secrets and variables > Actions | Azure Portal: Service Principal JSON (az adatok exportálhatók az Azure CLI-vel: `az ad sp create-for-rbac --sdk-auth`)
| AZURE_LOCATION       | Secret     | GitHub repo: Settings > Secrets and variables > Actions | Pl. `westeurope`, `northeurope` (Azure régió neve)
| SQL_ADMIN_USER       | Secret     | GitHub repo: Settings > Secrets and variables > Actions | Tetszőleges felhasználónév, amit az SQL szerverhez használsz
| SQL_ADMIN_PASS       | Secret     | GitHub repo: Settings > Secrets and variables > Actions | Erős jelszó, amit az SQL szerverhez használsz

## Pipeline input (workflow_dispatch)
- **environment**: (dev, staging, prod) – Ezt a pipeline indításakor választod ki manuálisan.

## Pipeline-ban generált változók (nem kell előre létrehozni)
- RESOURCE_GROUP
- SQL_SERVER_NAME
- SQL_DB_NAME
- APP_SERVICE_PLAN
- WEBAPP_NAME

Ezeket a pipeline automatikusan generálja az environment alapján, nem kell őket előre beállítani.

---

## Példa: Azure Service Principal létrehozása CLI-vel
```sh
az ad sp create-for-rbac --name "github-actions-sp" --role contributor --scopes /subscriptions/<SUBSCRIPTION_ID> --sdk-auth
```
A parancs kimenetét másold be a GitHub-on az `AZURE_CREDENTIALS` secret értékeként.

## Hol találod a beállításokat?
- **GitHub Secrets**: Repository > Settings > Secrets and variables > Actions
- **Azure Portal**: https://portal.azure.com

---

Ha további környezeti változóra vagy secretre van szükség, bővítsd ezt a listát a pipeline igényei szerint.

