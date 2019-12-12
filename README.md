### Generate Config
To enable secrets the AppConfigData.cs is generated from local settings.

To generate `AppConfigData.cs` create `app_config.json` in the root directory and fill in the key-values corrisponding to `template/AppConfigData.cs`.  The generated values are wrapped in `{{key}}`

Example app_config.json
```json
{
  "REDDIT_API_KEY": "api_key",
  "REDDIT_API_REDIRECT_URL": "redirect_url"
}
```
