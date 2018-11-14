using System;

namespace KBEngine
{
	public interface ITrueSyncBehaviourGamePlay : ITrueSyncBehaviour
	{
		void OnPreSyncedUpdate();

		void OnSyncedInput();

		void OnSyncedUpdate();
	}
}
