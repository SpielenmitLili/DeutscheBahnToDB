//
//  _     _ _ _               _
// | |   (_) (_)___ _   _ ___| |_ ___ _ __ ___
// | |   | | | / __| | | / __| __/ _ \ '_ ` _ \
// | |___| | | \__ \ |_| \__ \ ||  __/ | | | | |
// |_____|_|_|_|___/\__, |___/\__\___|_| |_| |_|
//                  |___/
//
// This program was created with ❤️ by Lili Chelsea Urban in Saarburg (Germany)
// Contact: https://spielenmitlili.com/kontakt
//
//

// Reads DB-data from multiple sources and writes all data into a database so it can be accessed easily

using System;
using System.Net;
using MySqlConnector;

namespace DeutscheBahnToDB 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Define all needed variables
            int allRecords = 0;
            string[] recordsByTable = new String[100];
            string sourceList = "https://spielenmitlili.com/media/static/db/sourceList/list.txt";
            string sourceListTables = "https://spielenmitlili.com/media/static/db/tables/list.txt";;
            string[] dataList;
            string[] columnList;
            string[] dataFromServer;
            string[] tableNames;
            
            //DatabaseCredentials
            string databaseIP = "REMOVED_FOR_SECURITY_REASON";
            string databasePort = "REMOVED_FOR_SECURITY_REASON";
            string databasePasswort = "REMOVED_FOR_SECURITY_REASON";
            string databaseUsername = "REMOVED_FOR_SECURITY_REASON";
            string databaseName = "REMOVED_FOR_SECURITY_REASON";
            string SQLConnectionString = "Server=" + databaseIP + ";Port=" + databasePort +";User ID=" + databaseUsername + ";Password=" + databasePasswort + ";Database=" + databaseName;
            
            //Versionverwaltung
            string version = "alpha-0.0.1";
            string versionCheckServer = "https://spielenmitlili.com/media/static/software-update/db/version.txt";;

            // Get current time
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            //Some information needed to proceed the request
            string userName = System.Environment.UserName;
            string machineName = System.Environment.MachineName;

            // Start program with header
            Console.WriteLine("#############################################################################");
            Console.WriteLine(currentTime + " | Process started for user " + userName + " on " + machineName +  " in program DeutscheBahnToDB");
            Console.WriteLine(currentTime + " | DeutscheBahnToDB by Lili Chelsea Urban started!");
            Console.WriteLine("#############################################################################");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");

            try
            {
                //Überprüfe ob Updates vorhanden sind bevor das Programm läuft um zu verhindern, dass das Programm etwas macht, was nicht gemacht werden sollte
                SoftwareUpdate(version, versionCheckServer);
                
                //Zuerst muss abgerufen werden welche Listen überhaupt von der DB geliefert werden damit mit denen weiter gearbeitet werden kann
                dataList = GetDatalist(sourceList);
                tableNames = GetDatalist(sourceListTables);

                int currentFile = 0;
                
                foreach (string data in dataList)
                {
                    //Zuerst wird eine Abfrage gestartet und Daten vom Server abgerufen
                    dataFromServer = GetDataFromServer(data);

                    //Dann werden aus der entsprechenden Liste alle Spalten abgerufen
                    columnList = GetColumns(dataFromServer[0]);
                    
                    //Hier wird der Programmteil aufgerufen welcher in der Datenbank die entsprechenden Änderungen macht
                    if (dataFromServer[0] != "")
                    {
                        dropAndRecreateTable(columnList, tableNames[currentFile], SQLConnectionString);

                        //Hier werden die Daten in die Datenbank eingefügt
                        writeDataToTable(dataFromServer, tableNames[currentFile], SQLConnectionString, columnList);
                    }
                    else
                    {
                        throw new Exception("Es ist ein Fehler beim Herunterladen der Daten von " + data + " aufgetreten! Es konnten keine Daten gefunden werden! Programm hat den Import abgebrochen und die aktuelle Tabelle nicht geändert!");
                    }
                    
                    
                    //Hier werden die Anzahl der Datensätze in eine Variable geschrieben damit wir am Ende eine Information über Anzahl der Datensätze bekommen
                    allRecords = allRecords + dataFromServer.Length;
                    
                    recordsByTable[currentFile] = tableNames[currentFile] + " <> " + dataFromServer.Length.ToString();

                    currentFile++;
                }

                
                WriteResults(recordsByTable, allRecords);
            }
            catch (Exception error)
            {
                ErrorHandler(error, machineName, userName);
            }

            //Get current time
            currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("#############################################################################");
            Console.WriteLine(currentTime + " | Process completed for user " + userName + " on " + machineName + " in program DeutscheBahnToDB");
            Console.WriteLine(currentTime + " | DeutscheBahnToDB by Lili Chelsea Urban ended!");
            Console.WriteLine("#############################################################################");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
        }

        static void ErrorHandler(Exception error, string machineName, string userName)
        {
            // Get current time
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("#############################################################################");
            Console.WriteLine(currentTime + " | Error occured for user " + userName + " on " + machineName + " in program DeutscheBahnToDB");
            Console.WriteLine(currentTime + " | Error: " + error.ToString());
            Console.WriteLine("#############################################################################");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" "); 
        }

        static string[] GetDatalist(string sourceList)
        {
            string responseFromServer = GetWebData(sourceList);
            string[] dataList = SplitToArray(responseFromServer, "\n");
            
            return dataList;
        }

        static string[] SplitToArray(string dataToConvert, string splitChar)
        {
            string[] splittedArray = dataToConvert.Split(splitChar);
            return splittedArray;
        }

        static string[] GetColumns(string columnRow)
        {
            string[] dataList = SplitToArray(columnRow, ";");
            rewriteColumns(dataList);
            return dataList;
        }

        static string[] GetDataFromServer(string currentFileLink)
        {
            string responseFromServer = GetWebData(currentFileLink);
            
            string[] dataList = SplitToArray(responseFromServer, "\n");
            return dataList;
        }

        static string GetWebData(string currentLink)
        {
            WebRequest getDatalist = WebRequest.Create(currentLink);
            HttpWebResponse datalistFromServer = (HttpWebResponse)getDatalist.GetResponse();
            Stream dataStream = datalistFromServer.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            return responseFromServer;
        }

        static void dropAndRecreateTable(string[] columns, string tableName, string SQLConnectionString)
        {
            using var connection = new MySqlConnection(SQLConnectionString);
            connection.Open();
            
            string sql;
            string tempsql = "";
            
            //Zuerst muss die alte Tabelle gelöscht werden
            using var command = new MySqlCommand("DROP TABLE IF EXISTS " + tableName, connection);
            command.ExecuteNonQuery();

            //Hier werden alle Spalten in ein Abschnitt umgeschrieben
            foreach (var column in columns)
            {
                tempsql = tempsql + column + " varchar(100),";
            }

            tempsql = tempsql.Remove(tempsql.Length - 1, 1);

            sql = "CREATE TABLE " + tableName + "(" + tempsql + ");";
            
            //Anschließend muss eine neue Tabelle erstellt werden mit entsprechenden Spalten
            using var command1 = new MySqlCommand(sql, connection);
            //Console.WriteLine("CREATE TABLE " + tableName + "(" + tempsql + ");");
            command1.ExecuteNonQuery();
        }

        static void writeDataToTable(string[] data, string tableName, string SQLConnectionString, string[] columnlist)
        {
            using var connection = new MySqlConnection(SQLConnectionString);
            connection.Open();
            
            int currentRow = 0;
            int columnCount = columnlist.Length;

            foreach (string row in data)
            {
                string[] currentDataToWrite;
                string sql;
                string tempsql = "";
                
                //Erste Zeile überspringen, da darin die Namen der Spalten genannt sind
                if (currentRow != 0)
                {
                    currentDataToWrite = SplitToArray(row, ";");
                    sql = "INSERT INTO " + tableName + " VALUES (";
                    
                    //Console.WriteLine("Anzahl der Spalten: " + currentDataToWrite.Length);
                    
                    if (currentDataToWrite.Length > columnCount && tableName == "Bahnhoefe")
                    {
                        int currentLineTemp = 0;
                        string currentToRemember = "";
                        
                        foreach (string dataset in currentDataToWrite)
                        {
                            if (currentLineTemp == 7)
                            {
                                currentToRemember = dataset.Trim();
                            }
                            else if (currentLineTemp == 8)
                            {
                                currentToRemember = currentToRemember + dataset.Trim();
                                tempsql = tempsql + "'" + currentToRemember + "',";
                            }
                            else
                            {
                                tempsql = tempsql + "'" + dataset.Trim() + "',";
                            }

                            //Console.WriteLine(dataset);
                            currentLineTemp++;
                        }
                    
                        tempsql = tempsql.Remove(tempsql.Length - 1, 1);
                        sql = sql + tempsql + ")";
                    
                        //Console.WriteLine(sql);
                        using var command = new MySqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                    else if(currentDataToWrite.Length == columnCount)
                    {
                        foreach (string dataset in currentDataToWrite)
                        {
                            string datasetManipulated = dataset.Replace("'", "^");
                            tempsql = tempsql + "'" + datasetManipulated + "',";
                            //Console.WriteLine(dataset);
                        }
                    
                        tempsql = tempsql.Remove(tempsql.Length - 1, 1);
                        sql = sql + tempsql + ")";
                        
                        using var command = new MySqlCommand(sql, connection);
                        //Console.WriteLine(sql);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        //Do nothing
                    }
                }

                currentRow++;
            }
            // Get current time
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            
            Console.WriteLine(currentTime + " | Tabelle " + tableName + " wurde erfolgreich befüllt!");
        }

        static string[] rewriteColumns(string[] columnsToRewrite)
        {
            int currentIndex = 0;
            string[] rewrittenColumns = columnsToRewrite;
            
            foreach (string column in columnsToRewrite)
            {
                string changedColumn;
                //Hier werden die Columns so umgeschrieben, dass das Programm nicht meckert
                changedColumn = column.Replace(". ", "_");
                changedColumn = changedColumn.Replace(".", "");
                changedColumn = changedColumn.Replace("ß", "ss");
                changedColumn = changedColumn.Replace("Ä", "Ae");
                changedColumn = changedColumn.Replace("ä", "ae");
                changedColumn = changedColumn.Replace("Ö", "Oe");
                changedColumn = changedColumn.Replace("ö", "oe");
                changedColumn = changedColumn.Replace("Ü", "Ue");
                changedColumn = changedColumn.Replace("ü", "ue");
                changedColumn = changedColumn.Replace(" ", "");
                changedColumn = changedColumn.Replace("(", "");
                changedColumn = changedColumn.Replace(")", "");
                changedColumn = changedColumn.Replace("-", "");
                changedColumn = changedColumn.Replace("[", "");
                changedColumn = changedColumn.Replace("]", "");

                //Schreibe in an die richtige Stelle
                rewrittenColumns[currentIndex] = changedColumn;
                
                currentIndex++;
            }

            return rewrittenColumns;
        }

        static void WriteResults(string[] resultsByColumn, int allResults)
        {
            // Get current time
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            
            Console.WriteLine(currentTime + " | Der Datenimport ist erfolgreich gewesen! Ergebnisse werden geladen...");
            Console.WriteLine(" ");
            
            Console.WriteLine(currentTime + " | Gesamtanzahl der Datensätze: " + allResults.ToString());

            foreach (var result in resultsByColumn)
            {
                if (result != null && result != "")
                {
                    string[] resultToOutput = result.Split(" <> ");
                    Console.WriteLine(currentTime + " | Tabelle: " + resultToOutput[0] + " mit " + resultToOutput[1] + " Datensätzen");
                }
            }
            
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
        }

        static void SoftwareUpdate(string version, string updateserver)
        {
            // Get current time
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            
            //Prüfe die aktuell neuste Version
            string[] currentVersion = GetDatalist(updateserver);

            Console.WriteLine(" ");
            
            if (currentVersion[0] == version)
            {
                Console.WriteLine(currentTime + " | Die von dir genutzte Version des Programms ist aktuell! Viel Spaß bei der Nutzung");
            }
            else
            {
                Console.WriteLine("#############################################################################");
                Console.WriteLine(currentTime + " | Du benutzt nicht die neuste Version von DeutscheBahnToDB!!!");
                Console.WriteLine(currentTime + " | Wenn du fortfährst kann das zu Problemen führen");
                Console.WriteLine(currentTime + " | Wir empfehlen die neuste Version des Programms unter https://git.spielenmitlili.com/SpielenmitLili/deutschebahntodb/ und das Programm anschließend neu zu starten bevor du fortfährst");
                Console.WriteLine(currentTime + " | Drücke beliebige Taste um fortzufahren!!!");
                Console.WriteLine("#############################################################################");
                
                //Warten auf Tastendruck
                Console.ReadKey();
            }
            
            Console.WriteLine(" ");
        }
    }
}

//
// Copyright 2023 by Lili Chelsea Urban. All Rights Reserved
//
