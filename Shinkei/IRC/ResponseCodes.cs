
namespace Shinkei.IRC
{
    enum ResponseCodes
    {
        RPL_WELCOME = 001,      // The first message sent after client registration. The text used varies widely
        RPL_YOURHOST = 002,      // Part of the post-registration greeting. Text varies widely
        RPL_CREATED = 003,      // Part of the post-registration greeting. Text varies widely
        RPL_MYINFO = 004,      // Part of the post-registration greeting
        RPL_BOUNCE = 005,      // Sent by the server to a user to suggest an alternative server, sometimes used when the connection is refused because the server is already full.


        RPL_UMODEIS = 221,      // Information about a user's own modes. Some daemons have extended the mode command and certain modes take parameters (like channel modes).

        RPL_AWAY = 301,      // Used in reply to a command directed at a user who is marked as away

        RPL_USERHOST = 302,      // Reply used by USERHOST (see RFC)
        RPL_ISON = 303,      // Reply to the ISON command (see RFC)

        RPL_TEXT = 304,      // ?

        RPL_UNAWAY = 305,      // Reply from AWAY when no longer marked as away
        RPL_NOWAWAY = 306,      // Reply from AWAY when marked away

        RPL_WHOISUSER = 311,      // Reply to WHOIS - Information about the user
        RPL_WHOISSERVER = 312,      // Reply to WHOIS - What server they're on
        RPL_WHOISOPERATOR = 313,      // Reply to WHOIS - User has IRC Operator privileges
        RPL_WHOWASUSER = 314,      // Reply to WHOWAS - Information about the user
        RPL_ENDOFWHO = 315,      // Used to terminate a list of RPL_WHOREPLY replies

        RPL_WHOISIDLE = 317,      // Reply to WHOIS - Idle information
        RPL_ENDOFWHOIS = 318,      // Reply to WHOIS - End of list
        RPL_WHOISCHANNELS = 319,      // Reply to WHOIS - Channel list for user (See RFC)

        RPL_LISTSTART = 321,      // Channel list - Header
        RPL_LIST = 322,      // Channel list - A channel
        RPL_LISTEND = 323,      // Channel list - End of list

        RPL_CHANNELMODEIS = 324,      // ?

        RPL_NOTOPIC = 331,      // Response to TOPIC when no topic is set
        RPL_TOPIC = 332,      // Response to TOPIC with the set topic

        RPL_INVITING = 341,      // Returned by the server to indicate that the attempted INVITE message was successful and is being passed onto the end client
        RPL_WHOREPLY = 352,      // Reply to vanilla WHO (See RFC). This format can be very different if the 'WHOX' version of the command is used (see ircu).
        RPL_NAMREPLY = 353,      // Reply to NAMES (See RFC)

        RPL_INFO = 371,      // Reply to INFO
        RPL_MOTD = 372,      // Reply to MOTD
        RPL_INFOSTART = 373,      // ?
        RPL_ENDOFINFO = 374,      // Termination of an RPL_INFO list
        RPL_MOTDSTART = 375,      // Start of an RPL_MOTD list
        RPL_ENDOFMOTD = 376,      // Termination of an RPL_MOTD list

        RPL_TIME = 391,      // Response to the TIME command. The string format may vary greatly. Also see #679.

        ERR_UNKNOWNERROR = 400,      // Sent when an error occured executing a command, but it is not specifically known why the command could not be executed.
        ERR_NOSUCHNICK = 401,      // Used to indicate the nickname parameter supplied to a command is currently unused
        ERR_NOSUCHSERVER = 402,      // Used to indicate the server name given currently doesn't exist
        ERR_NOSUCHCHANNEL = 403,      // Used to indicate the given channel name is invalid, or does not exist
        ERR_CANNOTSENDTOCHAN = 404,      // Sent to a user who does not have the rights to send a message to a channel
        ERR_TOOMANYCHANNELS = 405,      // Sent to a user when they have joined the maximum number of allowed channels and they tried to join another channel
        ERR_WASNOSUCHNICK = 406,      // Returned by WHOWAS to indicate there was no history information for a given nickname
        ERR_TOOMANYTARGETS = 407,      // The given target(s) for a command are ambiguous in that they relate to too many targets

        ERR_NOORIGIN = 409,      // PING or PONG message missing the originator parameter which is required since these commands must work without valid prefixes
        ERR_NORECIPIENT = 411,      // Returned when no recipient is given with a command
        ERR_NOTEXTTOSEND = 412,      // Returned when NOTICE/PRIVMSG is used with no message given
        ERR_NOTOPLEVEL = 413,      // Used when a message is being sent to a mask without being limited to a top-level domain (i.e. * instead of *.au)
        ERR_WILDTOPLEVEL = 414,      // Used when a message is being sent to a mask with a wild-card for a top level domain (i.e. *.*)
        ERR_BADMASK = 415,      // Used when a message is being sent to a mask with an invalid syntax
        ERR_TOOMANYMATCHES = 416,      // Returned when too many matches have been found for a command and the output has been truncated.
        ERR_UNKNOWNCOMMAND = 421,      // Returned when the given command is unknown to the server (or hidden because of lack of access rights)
        ERR_NOMOTD = 422,      // Sent when there is no MOTD to send the client

        ERR_NONICKNAMEGIVEN = 431,      // Returned when a nickname parameter expected for a command isn't found
        ERR_ERRONEUSNICKNAME = 432,      // Returned after receiving a NICK message which contains a nickname which is considered invalid, such as it's reserved etc..
        ERR_NICKNAMEINUSE = 433,      // Returned by the NICK command when the given nickname is already in use
        ERR_NICKCOLLISION = 436,      // Returned by a server to a client when it detects a nickname collision

        ERR_USERNOTINCHANNEL = 441,      // Returned by the server to indicate that the target user of the command is not on the given channel
        ERR_NOTONCHANNEL = 442,      // Returned by the server whenever a client tries to perform a channel effecting command for which the client is not a member
        ERR_USERONCHANNEL = 443,      // Returned when a client tries to invite a user to a channel they're already on

        ERR_NEEDMOREPARAMS = 461,      // Returned by the server by any command which requires more parameters than the number of parameters given
        ERR_ALREADYREGISTERED = 462,      // Returned by the server to any link which attempts to register again

        ERR_CHANNELISFULL = 471,      // Returned when attempting to join a channel which is set +l and is already full
        ERR_UNKNOWNMODE = 472,      // Returned when a given mode is unknown
        ERR_INVITEONLYCHAN = 473,      // Returned when attempting to join a channel which is invite only without an invitation
        ERR_BANNEDFROMCHAN = 474,      // Returned when attempting to join a channel a user is banned from
        ERR_BADCHANNELKEY = 475,      // Returned when attempting to join a key-locked channel either without a key or with the wrong key
        ERR_BADCHANMASK = 476,      // The given channel mask was invalid

        ERR_NOPRIVILEGES = 481,      // Returned by any command requiring special privileges (eg. IRC operator) to indicate the operation was unsuccessful
        ERR_CHANOPRIVSNEEDED = 482,      // Returned by any command requiring special channel privileges (eg. channel operator) to indicate the operation was unsuccessful

        ERR_NOOPERHOST = 491       // Returned by OPER to a client who cannot become an IRC operator because the server has been configured to disallow the client's host

    }
}