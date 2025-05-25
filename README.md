# Verteiltes System Beispiel (C#)

## Dienste
- `AuthService`: Gibt bei `/login?username=admin&password=1234` ein JWT zurück
- `DataService`: Geschützter Zugriff via `/data` mit JWT im Header

## Starten
```bash
docker-compose up --build