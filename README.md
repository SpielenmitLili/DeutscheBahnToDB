# DeutscheBahnToDB

Einfache App zum Exportieren von Daten von der Deutschen Bahn AG in eine eigene Datenbank um diese für weitere Zwecke verwenden zu können

## Erklärung

Mithilfe dieser App wird automatisch verfügbaren von DB AG bereitgestellten Daten heruntergeladen und in eine vom Benutzer vorgegebene Datenbank eingelesen.

Anschließend kann mit diesen Daten weiter gearbeitet werden.

Bitte beachte, dass die Tabellen **bei jedem Durchlauf neu generiert werden**. Damit werden Änderungen die direkt an den Tabellen durchgeführt werden gelöscht!

Sollte es dazu kommen, dass dein Computer keine Daten aus einer der übermittelten Dateien empfangen kann, so wird die Tabelle nicht bearbeitet und du bekommst eine Rückmeldung

## Welche Daten können abgerufen werden?

Zum aktuellen Zeitpunkt können unter anderem folgende Daten abgerufen werden. 

- **Reisezentren** (Adressen, Öffnungszeiten, ... von Reisezentren der DB AG in Deutschland)
- **Bahnhöfe** (Abkürzungen, Adressen, ... von Bahnhöfen der DB AG in Deutschland)
- **Aufzüge** (Standort, Anzahl, ... der Aufzüge an unterschiedlichen Standorten)
- **Bahnsteige** (Höhe, Anzahl, ... der Bahnsteige an unterschiedlichen Bahnhöfen)
- **Serviceeinrichtungen** (Standort, Typ, ... der Serviceeinrichtungen der DB AG)

## Wie wird dieses Programm verwendet

Es sind Änderungen an Quelltext nötig um dieses Package verwenden zu können.

So müssen in der Datei `program.cs` die Zugangsdaten für die Datenbank eingetragen werden, bevor das Programm ausgeführt werden:

```
//Hier ist die Datei mit Beispieldaten ausgefüllt
 string databaseIP = "10.0.0.10"; //HIER DIE IP EINTRAGEN UNTER WELCHER DIE DATENBANK ERREICHBAR IST
 string databasePort = "3006"; //HIER EINMAL PORT EINTRAGEN UNTER DEM DER PORT LÄUFT
 string databasePasswort = "password12345"; //HIER DAS PASSWORT FÜR DIE DATENBANK EINTRAGEN
 string databaseUsername = "root"; //HIER DEN BENUTZERNAMEN DES DATENBANKBENUTZERS EINTRAGEN
 string databaseName = "TESTDATABASE"; //HIER DEN NAMEN DER JEWEILIGEN DATENBANK EINTRAGEN
 ```

**Nuget-Pakete**

- Für die Nutzung des Programs muss ein Nuget-Paket installiert sein. Dieses Paket ist: **`MySqlConnector`**. Bei der Entwicklung des Programms wurde die Funktionstüchtigkeit mithilfe der Version `2.3.0-beta.1` getestet

## Update

Das Programm verfügt über eine Sequenz zu Software-Update. Dabei wird geprüft, ob das Programm in der neusten Version vorliegt oder nicht. Dementsprechend wird eine kurze Rückmeldung in die Konsole abgegeben, sodass der Benutzer Bescheid weiß, ob es aktuell ist oder nicht

## Ergebnisse

Die Ergebnisse des Programms werden in die angegebene Datenbank geschrieben. Sonst ist nichts weiter zu beachten an dieser Stelle.

## Lizensierung

Hinter diesem Projekt steckt eine Menge Arbeit. Du darfst das Projekt gerne gemäß der Lizenz in der Datei `LICENSE` benutzen und weiter verbreiten. 
