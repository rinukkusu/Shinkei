using Shinkei.API.Entities;

namespace Shinkei.IRC.Events
{
	public class IrcNoticeEvent : IrcMessageEvent
	{
		public IrcNoticeEvent(Server server, IEntity sender, IEntity recipient, string text) : base(server, sender, recipient, text)
		{
		}
	}
}