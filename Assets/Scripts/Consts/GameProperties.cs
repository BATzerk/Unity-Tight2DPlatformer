using UnityEngine;
using System.Collections;

public class GameProperties : MonoBehaviour {
	public const string VERSION_NUMBER = "1.0";
	private static readonly string[] player0Names = { "Tobias Fünke", "Blue Man Group Member #3", "Aggressive Blue Dot", "Sapphire Smasher", "Not Green", "Phthalocyanine Punisher" };//"\"I Blue Myself\"", 
	private static readonly string[] player1Names = { "Señor Verde", "The Green Slammer", "Verdant Spasm", "Christian Kale", "Lettuce Slave", "Kate Moss", "1 Rupee" };


	static public Color PlayerColor(int playerIndex) {
		switch(playerIndex) {
			case 0: return new Color(0/255f,82/255f,125/255f);//26/255f,0/255f,218/255f);
			case 1: return new Color(110/255f,210/255f,0/255f);//131/255f,0/255f,115/255f);//
			default: return Color.red; // Hmm.
		}
	}
	static public string PlayerName(int playerIndex) {
		switch(playerIndex) {
			case 0: return player0Names[Random.Range(0,player0Names.Length)];
			case 1: return player1Names[Random.Range(0,player1Names.Length)];
			default: return "undefined...";
		}
	}




}


