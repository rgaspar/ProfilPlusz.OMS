# CI/CD Workflow dokumentáció

## Lépések sorrendben

### 1. Infrastructure Provisioning (`infrastructure.yml`)
- Manuálisan futtatandó, egyszer (vagy ha új környezet kell)
- Létrehozza: Resource Group, SQL Server, SQL Database, App Service Plan (F1 Free)
- Idempotens: ha már léteznek az erőforrások, nem hibázik

### 2. Build and Push (`build-and-push.yml`)
- Docker image build az `ASPNET` projektből
- Push a GitHub Container Registry-be (ghcr.io) — **ingyenes**
- Automatikusan generált image tag: `v.YYYY.NNN`
- GitHub tag is létrejön ugyanezzel a névvel

### 3. Deploy (`deploy.yml`)
- Automatikusan indul, ha a build workflow sikeresen lefut (master/main branch)
- Vagy manuálisan indítható tetszőleges image tag-gel
- Azure Web App-ot konfigurálja a GHCR image-dzsel
- Beállítja az adatbázis connection stringet
- A végén kiírja a publikus URL-t

## Secrets lista

Lásd: `docs/required-secrets-and-variables.md`

## Hibakeresés

- Ha a deploy nem indul automatikusan: ellenőrizd, hogy a build workflow neve pontosan `02 - Build and Push Docker Image`
- Ha GHCR pull hibás: ellenőrizd a `GHCR_PAT` secret érvényességét
- Ha SQL connection nem működik: futtasd az `azure-login-test.yml`-t a credentials ellenőrzéséhez
- Cold start (F1 limitáció): az app demó előtt egyszer nyisd meg, hogy "felébredjen"
