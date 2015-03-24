Shinkei [![Build Status](https://travis-ci.org/rinukkusu/Shinkei.svg?branch=master)](https://travis-ci.org/rinukkusu/Shinkei)
=======

an IRC bot written in C# with a massive plugin system.

Fancy stuff that needs to be done
---------------------------------
- [x] an input loop in <code>Program.cs</code> for interacting on console level
- [x] aliases for servers to interact properly from the console or to find servers from our plugins
- [x] usage of template functions for registering events (don't expose <code>Eventsink</code>)
- [ ] allow console to interact with all commands by adding an argument for the server identifier, which will set the Server on CommandMessage but won't be visible to the commands
- [ ] a log method for plugins instead of using Console.WriteLine
- [ ] also add log files