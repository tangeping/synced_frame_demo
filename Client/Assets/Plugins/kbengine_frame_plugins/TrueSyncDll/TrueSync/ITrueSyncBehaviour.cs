using System;

namespace KBEngine
{
	public interface ITrueSyncBehaviour
	{
		void SetGameInfo(TSPlayerInfo localOwner, int numberOfPlayers);
	}
}
