using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EventManager {
	// Actions and Event Variables
	public delegate void NoParamAction ();
	public delegate void FloatAction (float a);
	public delegate void FloatFloatAction (float a, float b);
	public delegate void IntAction (int a);
	public delegate void StringAction (string a);
	public delegate void PlayerAction (Player player);
	public delegate void PlayerPlayerAction (Player p0, Player p1);

	public event NoParamAction ScreenSizeChangedEvent;
	public event PlayerAction PlayerOpenSmashBallAction;
	public event PlayerPlayerAction PlayerStompPlayerAction;

	// Program Events
	public void OnScreenSizeChanged () { if (ScreenSizeChangedEvent!=null) { ScreenSizeChangedEvent (); } }
	// Game Events
	public void OnPlayerOpenSmashBall(Player player) { if (PlayerOpenSmashBallAction!=null) { PlayerOpenSmashBallAction(player); } }
	public void OnPlayerStompPlayer(Player p0, Player p1) { if (PlayerStompPlayerAction!=null) { PlayerStompPlayerAction(p0,p1); } }

}




