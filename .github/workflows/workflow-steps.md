# CI/CD lépések és workflow dokumentáció

## Fő lépések

1. **Build and Push Docker Image to ACR**
   - Automatikusan fut minden branch-en.
   - Létrehozza az image-et, pusholja az Azure Container Registry-be.
   - Automatikusan generálja az image tag-et (pl. v.2025.001).
   - Feltölti az image nevét artifactként.
   - Törli a legrégebbi 5 image-et az ACR-ből.
   - Létrehoz egy GitHub tag-et az aktuális image tag alapján.

2. **WebApp Deploy**
   - Automatikusan indul, ha a build workflow sikeresen lefut.
   - Letölti az image nevét artifactból.
   - Létrehozza vagy frissíti az Azure Web App-ot az új image-dzsel.
   - Beállítja az adatbázis connection stringet.

## Titkok (secrets)
- `AZURE_CREDENTIALS`: Azure service principal JSON
- `ACR_LOGIN_SERVER`: pl. profilpluszomsacr.azurecr.io
- `ACR_USERNAME`, `ACR_PASSWORD`: ACR admin user
- `SQL_ADMIN_USER`, `SQL_ADMIN_PASS`: SQL szerverhez

## Hibakeresés
- Ha a deploy workflow nem indul el automatikusan, ellenőrizd:
  - Mindkét workflow ugyanazon a branch-en van-e
  - A workflow fájlok fent vannak-e a repositoryban
  - A build workflow neve pontosan egyezik-e a triggerben
  - A build workflow sikeresen lefutott-e
- Ha az ACR image törlés nem működik, ellenőrizd az Azure login lépést és a titkokat.

## Manuális parancsok
- Docker image push: `docker push <acr>/<repo>:<tag>`
- GitHub tag létrehozás: `git tag <tag>; git push origin <tag>`

## Egyéb tippek
- A workflow fájlok módosítása után mindig commitolj és pusholj!
- Az image tag automatikusan generálódik, nem kell kézzel megadni.
- Az artifactok 90 napig elérhetők GitHubon.

---

Ha bővíteni szeretnéd, csak írd hozzá a további lépéseket, tapasztalatokat vagy parancsokat!

