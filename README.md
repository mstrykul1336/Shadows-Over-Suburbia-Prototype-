# Shadows Over Suburbia Prototype 
10/7/24:
Tasks:
- Fix attack system and get it working.
- Fix health UI to display properly 
- Make it so players leave the lobby (because winning and voting both rely on the playerList from photon and I don't want to go in and try to change that. It's easier to just force the player out of the game so that it doesn't count them. Eventually they'll get to drop a death note though so at least they'll contribute something if they die.)
- Implement chat box with being able to disable it for spectating players (if that's the way I go) and disable the chat box at night cycle.
- Make sure voting runs smoothly (I think currently it needs time for votes to be counted before immediately voting out a player and needs to check if someone has equal votes). I think I also want to change it so voting doesn't happen on the first day cycle. Just start the game and give the players 30ish seconds to read their role and get used to their abilities. Then go to night cycle, then when day happens again, voting will start. 
- Add in basics for character names, assigning characters at random and debugging in what abilities you get as that character.
- Add in debug for what abilities you get as each role. 
Backlog:
- Add shop and coin system. (This will require I add items too)
- Add options to menu screen.
- Add in neutral character side for OldMan character. 

What was done: 
- Fixed health UI, now players have separate health UI and can't see other health. (Still buggy)

- Removed RoleAssigner script and moved its contents to Game manager. This was to fix a problem with player controllers not receiving role information, therefore everyone was given the same health. Now health works properly per role, so each role accurately has the health they're supposed to. 

- Fixed attacking so now everyone has a separate attack UI, drop downs properly fill with players, and players can attack up to one other player. Drop-down makes sure not to include yourself. It still seems a little buggy though.

- Added a day counter at the top so you can see what day you're on. 

- Added a 30 second intermission for day 0, then it goes directly to night cycle. No voting needed if you don't know who to vote for! Voting starts at normal on day 1. 

- Voting now accounts for extra time to collect votes (30 seconds) and if there is a tie. If there is a tie, it will restart voting. It also makes sure players can only vote once. 

- The shop has arrived! Spend that hard earned gold on some items (currently just one for proof of concept)! Items will run out after a certain number of purchases! Don't read the fine print about items not currently doing anything. Walk by the shopkeeper and hit F to open the shop! Hit it again to close it.

- Want to admire those items you just bought? Hit I and open up that inventory of yours!

- I want to still add characters but I feel that's more of a formal element. Not required for the game loop. I will be adding these in vertical slice. 

- Abilities exist now! Just...in the debug, but they get assigned! Not all abilities are thought of yet tho so they're not all there.
- It tries to force the player out of the lobby, but sometimes it works and other times it doesn't. I think it has to do with who is the master client, even though I have it set to change master client to someone else if the master client was killed.
- I tried to make a chat but had to delete it because it was buggy and felt really weird to use in first POV. I think I'll stick with trying to add voice chat instead. 

9/30/24:
Tasks: 
Attack system and health system
- Winning and losing based on roles and alignment 
- Chat box 
- Announce what players were killed over night and how during day cycle
- Do something to players when they die (I want them to be able to spectate the game, so I will implement that or if that doesn't work, force them out of the game if they die so they can't interrupt it.)
- Add some options to the menu screen (I'm not sure if they'll work yet, but just adding the functionality of the buttons)
- Fix the two capsule issue if possible 
- Make sure voting works smoothy (check to see if two people have the same # of votes, it won't just crash and will either eliminate both or revote)
- Add in basics for characters (assign random character to each player and debug log what buff that gives them)
- Maybe start to implement shop, if time allows. Basic shop functions.
- What was done:
- -Fixed the 2 capsules issue.
- Tried to work on an attack system for at least 6 hours, it doesn't work. I can't get the player ID to figure out what players exist.
- Health system is there, UI is buggy right now. But it exists and every role has different health.
- Winning and losing exists, depending on what side you are on and how many players are left. This function is called after voting and during the start of the day cycle to check win cons. It will put text on the screen with winning team and then send you back to title screen. 
- Announces what players were killed over the night (for now, you can test this by killing yourself since attacks don't work.)
- Attack UI and code is there, it's just a mess. 
- If you are voted out or die, it will destroy your game object for now. I wanted it to disconnect you, but photon told me no. 

9/23/24:

Tasks:

Finish any changes needed to multiplayer lobby to make it work.
Get the core game loop working of day and night cycles until one group of players win. 
Change the game from 2D to 3D (I was originally following a guide to work on my game loop, but the guide turned out to be horrible so I need to change the game back to 3D to follow my original player experience idea)
Get role assigning working, no abilities or anything yet. Just health and ability to attack someone. 
Get basic day and night working, no scene changes yet. 
Get core game loop working so that day happens for about 3 minutes, then night happens, you get an option to attack someone, then day happens and the results of the night are revealed in text for now. 
I want to get the first person point of view working, but we'll see. For now, the plan is just to have capsules sitting in a room.
Set up heart system in text and player list. 

What was completed:
- Voting is now working wooo! You can use a dropdown to vote and vote out a player (currently doesn't do anything to them)
- Multiplayer is now working! You can join the room and run around! (don't worry about the second capsule attached to your body!)
- Day and night cycles now exist! You can vote in the day and exist in the backrooms in the night! And repeat this infinitely! Yay!
- While the timers are only 10 seconds for debug, the day will be able to be 3 minutes, then voting for around 30 seconds and then night cycle for another 3 minutes.
- First person point of view works, is using a script from asset store. You can run, jump, zoom, and crouch! perfect for inspecting your capsule friends closely!
- Randomizer roles and alignment works! It'll randomize a role and alignment for all players. Right now, it is set so that it can start with just 1 player for debug. This will not be true later, you will need 3 players to even player the game.
  
9/17/24:
Tasks: 
- Figure out which multiplayer engine to use. (Unity or Proton? More than likely Proton, but I will be watching a beginner's video to Unity multiplayer to see if I like it. This is the video: https://youtu.be/E9eHefMpVnM?si=_1fXScONaJZOLwzG) 
- Get multiplayer lobby running where players can join a lobby and queue up for a game.
- Get text chat working for players
- Have a login page for players 
- Assign basic roles to players (no abilities yet, just basic roles)
- Set up a day and night cycle.
- Work out basic and functional UI (text box, leave button, list of players, basic square icons, health)
- Have cubes at a circle table, but no movement yet. 
- Establish a day and night cycle, but no scene changes yet. Just have them in the same room for now.
- Establish winning and losing based on what roles are left. 
- For now, just give the option to attack someone at night. 
- Set up heart system. 
- Simple instructions on what roles are and what their winning condition is. 
What was completed:
- Added multiplayer lobby functionality (two different types, one works better than the other)
- Made a game scene that breaks because the old lobby was messed up and now it doesn't work with the new lobby.
- Made basic title screen.
- Followed a bad guide that did not work and took up 4-5 hours of my time.
- Also the names don't show up on the lobby because I took the code from an old build in multiplayer that had those not working. Do not know how to fix it like I did last time.
