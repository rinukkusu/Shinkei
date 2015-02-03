
namespace Shinkei.IRC
{
    enum ResponseCodes
    {
        RplWelcome             = 001,      // The first message sent after client registration. The text used varies widely
        RplYourhost            = 002,      // Part of the post-registration greeting. Text varies widely
        RplCreated             = 003,      // Part of the post-registration greeting. Text varies widely
        RplMyinfo              = 004,      // Part of the post-registration greeting
        RplBounce              = 005,      // Sent by the server to a user to suggest an alternative server, sometimes used when the connection is refused because the server is already full.


        RplUmodeis             = 221,      // Information about a user's own modes. Some daemons have extended the mode command and certain modes take parameters (like channel modes).
        
        RplAway                = 301,      // Used in reply to a command directed at a user who is marked as away

        RplUserhost            = 302,      // Reply used by USERHOST (see RFC)
        RplIson                = 303,      // Reply to the ISON command (see RFC)

        RplText                = 304,      // ?

        RplUnaway              = 305,      // Reply from AWAY when no longer marked as away
        RplNowaway             = 306,      // Reply from AWAY when marked away

        RplWhoisuser           = 311,      // Reply to WHOIS - Information about the user
        RplWhoisserver         = 312,      // Reply to WHOIS - What server they're on
        RplWhoisoperator       = 313,      // Reply to WHOIS - User has IRC Operator privileges
        RplWhowasuser          = 314,      // Reply to WHOWAS - Information about the user
        RplEndofwho            = 315,      // Used to terminate a list of RPL_WHOREPLY replies

        RplWhoisidle           = 317,      // Reply to WHOIS - Idle information
        RplEndofwhois          = 318,      // Reply to WHOIS - End of list
        RplWhoischannels       = 319,      // Reply to WHOIS - Channel list for user (See RFC)

        RplListstart           = 321,      // Channel list - Header
        RplList                = 322,      // Channel list - A channel
        RplListend             = 323,      // Channel list - End of list

        RplChannelmodeis       = 324,      // ?

        RplNotopic             = 331,      // Response to TOPIC when no topic is set
        RplTopic               = 332,      // Response to TOPIC with the set topic

        RplInviting            = 341,      // Returned by the server to indicate that the attempted INVITE message was successful and is being passed onto the end client
        RplWhoreply            = 352,      // Reply to vanilla WHO (See RFC). This format can be very different if the 'WHOX' version of the command is used (see ircu).
        RplNamreply            = 353,      // Reply to NAMES (See RFC)

        RplInfo                = 371,      // Reply to INFO
        RplMotd                = 372,      // Reply to MOTD
        RplInfostart           = 373,      // ?
        RplEndofinfo           = 374,      // Termination of an RPL_INFO list
        RplMotdstart           = 375,      // Start of an RPL_MOTD list
        RplEndofmotd           = 376,      // Termination of an RPL_MOTD list

        RplTime                = 391,      // Response to the TIME command. The string format may vary greatly. Also see #679.

        ErrUnknownerror        = 400,      // Sent when an error occured executing a command, but it is not specifically known why the command could not be executed.
        ErrNosuchnick          = 401,      // Used to indicate the nickname parameter supplied to a command is currently unused
        ErrNosuchserver        = 402,      // Used to indicate the server name given currently doesn't exist
        ErrNosuchchannel       = 403,      // Used to indicate the given channel name is invalid, or does not exist
        ErrCannotsendtochan    = 404,      // Sent to a user who does not have the rights to send a message to a channel
        ErrToomanychannels     = 405,      // Sent to a user when they have joined the maximum number of allowed channels and they tried to join another channel
        ErrWasnosuchnick       = 406,      // Returned by WHOWAS to indicate there was no history information for a given nickname
        ErrToomanytargets      = 407,      // The given target(s) for a command are ambiguous in that they relate to too many targets

        ErrNoorigin            = 409,      // PING or PONG message missing the originator parameter which is required since these commands must work without valid prefixes
        ErrNorecipient         = 411,      // Returned when no recipient is given with a command
        ErrNotexttosend        = 412,      // Returned when NOTICE/PRIVMSG is used with no message given
        ErrNotoplevel          = 413,      // Used when a message is being sent to a mask without being limited to a top-level domain (i.e. * instead of *.au)
        ErrWildtoplevel        = 414,      // Used when a message is being sent to a mask with a wild-card for a top level domain (i.e. *.*)
        ErrBadmask             = 415,      // Used when a message is being sent to a mask with an invalid syntax
        ErrToomanymatches      = 416,      // Returned when too many matches have been found for a command and the output has been truncated.
        ErrUnknowncommand      = 421,      // Returned when the given command is unknown to the server (or hidden because of lack of access rights)
        ErrNomotd              = 422,      // Sent when there is no MOTD to send the client

        ErrNonicknamegiven     = 431,      // Returned when a nickname parameter expected for a command isn't found
        ErrErroneusnickname    = 432,      // Returned after receiving a NICK message which contains a nickname which is considered invalid, such as it's reserved etc..
        ErrNicknameinuse       = 433,      // Returned by the NICK command when the given nickname is already in use
        ErrNickcollision       = 436,      // Returned by a server to a client when it detects a nickname collision

        ErrUsernotinchannel    = 441,      // Returned by the server to indicate that the target user of the command is not on the given channel
        ErrNotonchannel        = 442,      // Returned by the server whenever a client tries to perform a channel effecting command for which the client is not a member
        ErrUseronchannel       = 443,      // Returned when a client tries to invite a user to a channel they're already on

        ErrNeedmoreparams      = 461,      // Returned by the server by any command which requires more parameters than the number of parameters given
        ErrAlreadyregistered   = 462,      // Returned by the server to any link which attempts to register again

        ErrChannelisfull       = 471,      // Returned when attempting to join a channel which is set +l and is already full
        ErrUnknownmode         = 472,      // Returned when a given mode is unknown
        ErrInviteonlychan      = 473,      // Returned when attempting to join a channel which is invite only without an invitation
        ErrBannedfromchan      = 474,      // Returned when attempting to join a channel a user is banned from
        ErrBadchannelkey       = 475,      // Returned when attempting to join a key-locked channel either without a key or with the wrong key
        ErrBadchanmask         = 476,      // The given channel mask was invalid

        ErrNoprivileges        = 481,      // Returned by any command requiring special privileges (eg. IRC operator) to indicate the operation was unsuccessful
        ErrChanoprivsneeded    = 482,      // Returned by any command requiring special channel privileges (eg. channel operator) to indicate the operation was unsuccessful

        ErrNooperhost          = 491       // Returned by OPER to a client who cannot become an IRC operator because the server has been configured to disallow the client's host

    }
}
