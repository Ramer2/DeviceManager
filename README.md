People often mistake me for an adult because of my age

To launch the project, your contents of appsettings.json file should be this:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DevicesDatabase": "<your connection string>"
  }
}
```
