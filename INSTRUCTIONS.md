# Project Instructions

## Pipelines
- **infrastructure.yml**: Creates all Azure resources (resource group, SQL, App Service Plan, Web App, ACR-based Docker deployment, etc.).
- **deploy.yml**: Builds, tests, and deploys the application (manual trigger).
- **destroy.yml**: Deletes the selected environment's resource group and all resources in it (manual trigger, approval recommended).
- **acr-setup.yml**: Creates Azure Container Registry (ACR), enables admin user, and outputs secrets for GitHub Actions.

## Required GitHub Secrets
- `AZURE_CREDENTIALS`: Service Principal JSON (see below for generation)
- `AZURE_LOCATION`: Azure region (e.g., westeurope)
- `SQL_ADMIN_USER`: SQL admin username (must meet Azure requirements)
- `SQL_ADMIN_PASS`: SQL admin password
- `ACR_LOGIN_SERVER`: e.g., myregistry.azurecr.io
- `ACR_USERNAME`: ACR admin user or service principal
- `ACR_PASSWORD`: ACR admin password or service principal secret

## Service Principal Generation
Generate with Azure CLI:
```sh
az ad sp create-for-rbac --name "github-actions-sp" --role contributor --scopes /subscriptions/<SUBSCRIPTION_ID> --sdk-auth
```
Copy the full JSON output to the `AZURE_CREDENTIALS` secret.

## ACR Setup
Run the `acr-setup.yml` pipeline with the required inputs. Copy the outputted secrets to GitHub repository Settings > Secrets and variables > Actions.

## Developer Rules
- All code comments must be in English.
- Use only valid, secure values for secrets.
- For .NET 9.0 support, use Docker-based deployment (see infrastructure.yml and Dockerfile).

## Useful CLI Commands
- List available App Service runtimes:
  - Linux: `az webapp list-runtimes --os-type linux`
  - Windows: `az webapp list-runtimes --os-type windows`
- Check Service Principal:
  ```sh
  az ad sp show --id <clientId>
  ```

## Troubleshooting
- If Azure login fails, check the `AZURE_CREDENTIALS` secret format and content.
- If App Service Plan OS switch fails, delete the resource group or use a new name.
- For .NET 9.0, only Docker-based deployment is supported as of October 2025.

---
Update this file as the project evolves or as new requirements arise.

