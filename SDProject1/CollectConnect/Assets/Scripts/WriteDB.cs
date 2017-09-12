using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;


public class WriteDB : BoardManager
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public static void WriteConnection(string keyword, int cardOne, int cardTwo, IDbConnection _dbconn)
	{
		SqliteConnection dbcmd = new SqliteConnection(_dbconn.ConnectionString);
		//need to open db
		dbcmd.Open();
		const string sqlQuery = "INSERT INTO connections (attribute,playerID,card1ID,card2ID) VALUES (@attribute,@playerID,@card1ID,@card2ID)";

		SqliteCommand command = new SqliteCommand(sqlQuery, dbcmd);
		command.Parameters.AddWithValue ("@attribute", keyword);
		command.Parameters.AddWithValue ("@playerID", 1);
		command.Parameters.AddWithValue ("@card1ID", cardOne);
		command.Parameters.AddWithValue ("@card2ID", cardTwo);

		command.ExecuteNonQuery ();

		Debug.Log ("Wrote to the DB");
	}
}
