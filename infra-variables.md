# Szükséges GitHub Secrets és Environment Variables az infrastruktúra pipeline-hoz

Az alábbi változókat kell létrehoznod a GitHub repository-ban, hogy az infrastruktúra pipeline (infrastructure.yml) helyesen működjön minden environment (dev, staging, prod) esetén.

## 1. Hol kell létrehozni?
- **GitHub repository**: Settings > Secrets and variables > Actions
- **Ajánlott**: Environment scope-pal (dev, staging, prod), hogy minden környezethez külön értékeket adhass meg.

## 2. Szükséges Secrets/Variables

| Név                | Hol kell létrehozni? | Leírás | Hogyan szerezhető be? |
|--------------------|----------------------|--------|-----------------------|
| AZURE_CREDENTIALS  | Secret (env scope)   | Azure Service Principal JSON, a pipeline Azure login lépéséhez | Azure Portal > Azure Active Directory > App registrations > (app) > Certificates & secrets + szükséges adatok (az adatok exportálhatók az Azure CLI-vel is) |
| AZURE_LOCATION     | Variable/Secret (env scope) | Azure régió, pl. `westeurope` | Azure Portal régiólistából vagy dokumentációból |
| SQL_ADMIN_USER     | Secret (env scope)   | SQL szerver admin felhasználónév, pl. `sqladmin` | Saját választás |
| SQL_ADMIN_PASS     | Secret (env scope)   | SQL szerver admin jelszó (erős jelszó!) | Saját választás |

## 3. Példa beállítás (dev environmentre)

- **Settings > Environments > dev > Add Secret/Variable**
  - AZURE_CREDENTIALS
  - AZURE_LOCATION
  - SQL_ADMIN_USER
  - SQL_ADMIN_PASS

Ugyanezt ismételd meg staging/prod environmentekre is, ha eltérő értékeket szeretnél.

## 4. Hol találod meg az értékeket?
- **AZURE_CREDENTIALS**: Azure Portal vagy Azure CLI (pl. `az ad sp create-for-rbac --name "github-action-sp" --sdk-auth`)
- **AZURE_LOCATION**: Azure Portal régiólistája (pl. westeurope, northeurope, etc.)
- **SQL_ADMIN_USER / SQL_ADMIN_PASS**: Saját választás, de jegyezd fel, mert az adatbázis eléréséhez szükséges lesz.

---

Ha további környezeti változóra vagy titokra van szükség (pl. SMTP, JWT, stb.), azokat is hasonló módon add hozzá az adott environmenthez!

