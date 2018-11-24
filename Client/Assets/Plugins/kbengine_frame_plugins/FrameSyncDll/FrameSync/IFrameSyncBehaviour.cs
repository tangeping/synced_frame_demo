using System;

namespace KBEngine
{
	public interface IFrameSyncBehaviour
	{
		void SetGameInfo(FPPlayerInfo localOwner, int numberOfPlayers);
	}
}
