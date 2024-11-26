# Shadows Over Suburbia Prototype 
**Credits:**
First Person Controller: https://assetstore.unity.com/packages/3d/characters/modular-first-person-controller-189884?srsltid=AfmBOorWmRFs9bx7ebwFwd15ni4eQk7cPH1-OeGcAaLAW0HUaVO0Xw5H
ChatGPT for questions and bugs, learning how to do certain things I haven't done before in Unity.
Canva for demo asset icons (I will be drawing some of them later) 
Fantasy Forest Village: https://assetstore.unity.com/packages/3d/environments/fantasy/fantasy-forest-village-123484
Polygon City Pack: 
https://assetstore.unity.com/packages/3d/polygon-city-pack-environment-and-interior-free-101685
Farland Skies:
https://assetstore.unity.com/packages/2d/textures-materials/sky/farland-skies-cloudy-crown-60004
Fantasy Terrain Textures: 
https://assetstore.unity.com/packages/2d/textures-materials/free-fantasy-terrain-textures-233640
Fantasy Towns Music Pack: 
https://chrislsound.itch.io/fantasy-towns-music-pack
Ghost model: 
https://assetstore.unity.com/packages/3d/characters/creatures/ghost-character-free-267003
Night room models: 
https://assetstore.unity.com/packages/3d/environments/interior-house-assets-urp-257122
Office pack for Town Hall: 
https://assetstore.unity.com/packages/3d/props/interior/office-pack-free-258600\
Villager Models: 
https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/stylized-npc-peasant-nolant-demo-252440
Witch, Citizen, and Elf models:
https://assetstore.unity.com/packages/3d/characters/low-poly-female-free-low-poly-3d-by-shokubutsu-299182
Goblin Model:
https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/low-poly-stylized-cartoon-goblin-293432

**11/25/24** 
**What was done**
- As a player, I want to see the current mouse sensitivity value so I can adjust it more accurately. (added a current text next to mouse sensitivity so the player knows what the mouse sensitivity is currently at)

-As a player, I want the option to invert camera controls so I can play the game in a way that feels natural to me. (added an invert toggle to be able to invert the camera control)

-As a player, I want clear text explaining that character models are cosmetic and don’t affect gameplay so I can make informed choices. (changed character selection text to "character model" and added text to say that this will not have an effect on the gameplay, it's just cosmetic to address some of the confusion.)

-As a player, I want text in night and day cutscenes to clarify what’s happening in the game’s progression. (added text to the night and day cutscenes to hopefully make them a little more clear on what's happening.)

-As a player, I want all roles to have final drawn images so the game feels polished and visually complete. (all roles have their final drawn images.)

-As a player, I want the detective role implemented with its final image so I can enjoy a complete character experience. (the detective has their final drawn player image and has been implemented)

-As a player, I want the medic role implemented with its final image so I can enjoy a complete character experience. (the medic has their final drawn player image and has been implemented)

-As a player, I want the baker role implemented with its final image so I can enjoy a complete character experience. (the baker has their final drawn player image and has been implemented)

-As a player, I want the game to recognize and declare winners so I can feel rewarded for achieving victory. (winning works now! yay!)

-As a player, I want more time during the first day and extended day/night cycles so I can fully strategize and enjoy the gameplay (added more time to the first day time remaining, day cycle and night cycles)

-As a player, I want a lore-filled intro scene with voiceover and subtitles to immerse me in the game’s universe. (Added a new intro sequence scene where WiggleBlorp explains the lore behind the universe, including voice over (done by me) and subtitles)

-As a player, I want a basic tutorial with visual examples so I can understand the game mechanics before starting (Added in a basic tutorial in the intro scene for players to go over what the game is before playing with visual examples. )

-As a player, I want art for WiggleBlorp to enhance the game’s visual appeal and bring the character to life. (Drew art for wiggleblorp and added that. )

**To-Do: From user stories 4:**
-As a player, I would like my camera controls to be inverted so that I have an easier time walking around the room. / As a player I would like my controls to be inverted. (will be adding in option to invert it)

-As someone who likes having options for characters in a game, I would like some information on what's different about each character so I can weigh the pros and cons of each one. / As a new player, I would like to know more about the character types before selecting one. (will be adding in more lore about the roles and a disclaimer / info that the character selection screen is just for your character model) 

-As a new player, I would like to know more of what is happening during the cut scenes. / As a new player I would like some more insight as to what the cutscene was for.

-As a new player, I would like more time during the day cycle so that I have more time walk around and interact with players and NPCs. / As a player who is kinda slow, I would like some more time during both cycles to be able to think about what I want to do. (add more time) 

-As a new player, I would like some sort of intro sequence to be able to understand what is going on.

-As a confused player, I would like a tutorial mode so that I can figure out movement and UI before it is timed.

-Tasks from me:
-As a player who likes dramatic elements, I would like all roles to have their final drawn images. 

-As a player who likes challenge and good code, I would like the winning and losing to work properly.

-As a player who likes stories, I want to know the lore behind the game so I can better get into character. 

-As a player who likes stories, I want to know the lore behind each of the characters so I can better understand their motives. 
 




**11/18/24** **What was done**:

-As a player, I want the lighting on the models in the model select screen to be brighter so that I can see the details of each model more clearly.( made the lighting more bright on the models in the model select screen)

-As a player, I want the "F1" control to be updated to the left bracket key and reflected in the controls image so that the controls are easier to understand and use. (changed F1 to left bracket and updated the controls image )

-As a player, I want the "F3" control to be updated to the right bracket key and reflected in the controls image so that the controls are easier to understand and use. (changed F3 to right bracket and updated the controls image )

-As a player, I want separate audio groups for music and SFX with volume sliders in the settings menu so that I can customize my audio experience. (added in two audio groups, music and SFX whose volume can be controlled with sliders by the player in the settings menu)

-As a player, I want music to play during the day cycle, night cycle, and voting time so that the game feels more immersive. (added in music to the day cycle, night cycle, and voting time)

-added in sound effects for the timers going off, for the beginning of the night cycle animation and for the beginning of the day cycle animation

-As a player, I want sound effects to play when timers go off so that I am clearly alerted to changes in game phases. (any other time timers go off, I made it play the timer sound effect)

-made it so it checks photon view before switching back to player camera, just so you don't get someone else's camera. It appears to work and now you get your own camera when it switches from main to the player.

-drew the old man's profile picture and implemented it into the game

-drew the clairvoyant's profile picture and implemented it into the game

-drew the villager's profile picture and implemented it into the game

-drew the assistant's profile picture and implemented it into the game

-drew sketches for the detective, Baker and medic and implemented those temporarily in the game until the drawings are complete

-As a player, I want the profile pictures of the characters that won to appear on the winning screen so that I can see which characters were victorious. (profile pictures of the characters that won will show up on the winning screen now )

-As a player, I want the names of the players that won to appear on the winning screen so that I can identify the winning participants (player names of the players that won show up on the winning screen )

-I have implemented some new code to the win condition function, hopefully fixing winning. It needs to be tested more. I tested with it, but it does not like photon so it's a bit broken right now. It is not correctly getting the player alignments from the photon custom properties, even though other functions use it just fine.

**To Do:**

As a player that likes dramatic elements, I want the game to have music and sound effects so that it adds to the aesthetic.

As a player that likes dramatic elements, I want all of the characters to have their profile picture images so the game has more world building.

As a player that likes clean UI, I want the models on the select model screen to be in the right place.

As a player that likes challenge, I want the winning and losing to flow better and work properly for the conditions it should.

As a player that likes changing camera angles, I want to make sure that I am not changing someone else’s camera or using someone else’s camera in the game.

As a player that likes fast paced games, I want there to be more options for killing or attacking other players.

As a player that likes dramatic elements and winning, I want my profile picture and name to show up on the winning screen.

As a player that uses WebGL builds, I want to change any of the F keys to new key binds because WebGL builds will open a new tab with F1 and the find bar with F3. (This will require changes to the control images and changes to the code to change these key binds)
**11/11/24:**
**What was done:**
- created a how to play screen and added it into the how to play section of the title screen. It is now a graphic with more organized steps should it should be clearer to understand and pick up.

- created a roles and abilities screen and added it into the roles and abilities section of the title screen. It is also now a graphic so it should be easier to understand.

- added the new roles and abilities image to the F1 key too

- added in the ability to have a 3rd person POV camera. You can toggle between 1st POV and 3rd by hitting F3. 

- Added F3 to the final controls image and added that to controls 

- Added new animations of the neighborhood for day and night cycles. 

- For transition to night cycle, the sky box is a night sky. It will switch everyone's cameras to the main camera to play the town animation, with fancy red spot light. 

- For transition to day cycle, the sky box will turn to the day sky. The animation will play again. 

- Timers have been mostly fixed (hopefully). All timers have been changed to use Photon Network time to hopefully keep synchronized. 

- Added a character model select screen! Choose from villager, goblin, elf, witch, citizen or villager (but blue)! Every other player will see the model you chose in game (thanks hashtables).

- Made it so if the player doesn't choose a model, it will default to a goblin (otherwise it crashes).

- Added new background image to lobby screen (it used to be just black). 

- Changed the shopkeepers model from a capsule to one of the villagers (and changed the collider to be bigger)

- Added controls image to settings menu 

- Added in code to make sure the mayor is pointed out with a diamond on the player list 

- Made sure player list looked neat 

- Drew profile picture asset for Mayor 

- Added in code to change your profile picture based on your role (I need to still draw the rest of them, so for now it's demo assets)

- Added in code functionality so that your profile picture displays in the newspaper on death (which will change based on role)

- Added intro music to the lobby screen 

- Added villagers to animation in the title screen 

- Added how to play, controls and roles/abilities instructions to lobby screen while waiting for the host to start the game (requested through a user story)

- **To Do:**
- As a player, I want clearer role instructions in a picture format so I can visualize my abilities and other role’s abilities.  

- As a player, I want to be clearer on how to play instructions in a picture format so I can visualize and comprehend the game mechanics easier.  

- As a visual player, I want animations between night and day cycles so that the game has more dramatic elements and worldbuilding. 

- As a visual player, I want the player profile pictures so that I know what character I am visually.  

- As a player, I want the timers to sync properly so I can play the game synchronized with other players.  

- As a player. I want the option to change the camera point of view from first person to third person so that I don’t get dizzy or sick from the camera controls.  

- User Stories 2: 

- As someone who enjoys deduction games, I would like a brief explanation of rules and controls before the gameplay starts, possibly on the game room waiting screen, so people can understand the game better while playing. 

- User Stories 3:  

- As a new player, I would like a ui text telling me the controls, especially for player abilities. 

- As a player, I want to see more worldbuilding in the game so that I can be more immersed in the game.  

- As a player who likes story and characters, I would like to see character models for the players 

**11/4/24:**
**What was done:**
- serialized all of the timers and added to check if master client before any coroutines are called. 
- changed party name to town name in the menu to fit aesthetic 
- made it so you can use escape for settings menu 
- made a mouse sensitivity slider in the settings so players can change mouse sensitivity
- changed all canvas UI to 1920 x 1080 and tried to lock all UI to their respective places. 
- changed the build resolution to 1270 x 720 to match the UI and still make it visible in the WebGL build. (1920 x 1080 made it too big to play)
- added more lighting and changed a lot of the lighting to white. this lighting will be changed when I change all the models
- made new 2D pixel sprites for all of the abilities and used them in place of the demo assets
- added a new red, bloody cursor 
- created a photon hashtable to store player roles and abilities 
- made another win screen for the old man if he wins and coded it in. 
- added in a bunch of new environment props and objects to the town hall room 
- added in a bunch of new environment props into every player's night room
- changed code for winning and losing to include old man's neutral alignment screen and hopefully by using the hashtable, the mechanic should work better. 
- changed the role reveal function to use the hashtable instead of trying to find the role in player controller list
  
**To Do:**
  
- I added in the how to play a different method of the voting system that is used in other games similar to mine. I want to implement this: 
- First round of voting will be held, where a player is selected to be voted out.  
- Then, another voting round will be held to either vote out the player or not. The player should get 30 seconds to make their case before players vote.  
-serialize timers and make sure any coroutine is always checking for the master client to do it, so hopefully this fixes any timer glitches.  
- make all UI scale to screen size with a screen size of 1920×1080. This includes locking the UI in certain places (From User Stories 3: As a player with bad eyesight, I would like the UI to lock to the corners, so that it is easier to read and I can see the center of the screen.) 
- make the HUD nicer and cleaner, making it easier to understand what’s going on and what you need to do. (From User Stories 3: “As a player who likes being given information, I would like the HUD to be a bit more organized so its not in my way and readable”). 
- try to make player list look nice and make sure the mayor icon displays for all players, not just for the mayor. 
- make UI to change mouse sensitivity in the settings wheel (From User Stories 3: “As a player, I would like to be able to change mouse sensitivity. “ ‘As a player who is picky with gameplay settings, I want an options menu so that I can adjust my mouse sensitivity and screen brightness”) 
- if mouse sensitivity does not fix the issues, consider and mess with changing the game from first POV to third POV (even though this isn’t my intended player experience, it may help with confusion, mouse clicking, etc). (From User Stories 3: “As a player, I would like there to be a secondary option to either move the camera or click using the mouse, so that I have an easier time clicking on things without my camera being weird or mouse locking.”)  
- winning and losing seems to be weird, where it sometimes wins if the sides are correct and sometimes doesn’t. I need to make sure this code is clear on winning and losing. (I am assuming it’s not correctly taking the alignments of players and still keeping dead players in count, so I will check there first. ) 
- newspaper role is just saying mayor no matter what their actual role was. Change how this works to properly implement what role the player was.  
- change the lighting in the rooms to a brighter direct light and use the red as spotlights to make the room clearer to see. (User Stories 3: “As a player who is picky with gameplay settings, I want an options menu so that I can adjust my mouse sensitivity and screen brightness”) 

**10/21/24:**
**What was done:**
- Made the voting UI cleaner and better by adding a background image, adding an image to the button, and adding some text to explain it. 
- Added in font everywhere that matches the game’s aesthetic. (similar to Luigi’s Mansion)
- Added in the town scene at the title screen with lighting and effects. This will be used later for animations. 
- Added in a keyboard image with all controls into Controls in title screen
- Added theme music to the title screen. 
- Added in abilities for:
- Clairvoyant: Can check two players to see if they are friends or enemies with each other. (Will not reveal neutral role)
- Assistant: Can check the alignment of a player. (Will reveal neutral role)
- Detective: Can watch one player for a night to see what actions they take. (Even if they take the action before or after you investigate them)
- Old Man: Vote counts for two votes when they vote. 
- Villager: Can attack a player for 1/4th heart. 
- Added in new spawn points so players stopped spawning on top of each other. 
- Added in text UI for when you are attacked or healed, just so you know why your hearts randomly go up or drop. (this currently isn’t working, not sure why the RPC calls are being messed up)
- Added new UI for each ability to differentiate them. 
- Added in functionality for the shield, so if it is used at night, it will block any attacks (currently poison or other damage). It will not block you from being poisoned though, just from not being damaged for one instance. 
- Items will be removed from inventory after use. 
- Poison damage and villager damage are still a bit buggy, usually it will attack you for twice the amount with poison damage on the first day it happens. Sometimes it will correctly attack you for the right amount. 
- Added in randomized death image newspapers for when players die to make it more fun. Player’s name that died will appear in the newspaper. There are around 7 different newspapers to get!
- Player UI List thing is still messed up, spacing is weird. 
- Knife is still buggy, but it might work. Made sure it leaves the inventory when done with and that it has everything in place to work. 
- Changed the player and party room texts to fit the theme of the game. 
- Added in the 1 button to toggle player movement on and off. 

Backlog for next week: 
Changing players from disconnecting to spectating as ghosts (make sure in every loop that calls playerList that it also checks to make sure they aren’t dead.)
Add in their roles into the newspaper if they die too (and eventually their profile picture image)

**To Do:**
Add in abilities for the rest of the roles that get them:  
Detective: Investigation (watch one player at night to see what happens to them) 
Assistant: Connections (discover if a target player attacks anyone tonight) 
Clairvoyant: Psychic (can select two players to learn if they are friendly or enemies to each other, based on helpful or destructive) 
Old man: Voter Fraud (can vote twice instead of once) 
Villagers: Attack (can attack people for ¼ heart) 
Add in UI for attacks so you know that you were attacked or taken or healed, whatever happened to you. 
Make the shield work as intended and actually block an attack at night (it should block poison, bleed, and normal attacks) 
Bug check the knife to make sure it properly attacks players, attacks player in the dropdown provided.  
I believe it does, but make sure the inventory actually takes the item out when it is used.  
Make a cool UI (image or video, but WEBGL hates videos) for when players die (randomly show different versions of the UI of them dying, like minecraft changes the text). Maybe change this to animation loaded elsewhere with the player’s model and a random death scene.  
Make the voting UI better and cleaner.  
Clean up the player list UI.  
Add in a font that matches the aesthetic for all text in the game.

**Backlog:**
Add in destructive and helpful versions of these abilities:   
Mayor: (currently has mayor's basement for helpful, but if destructive, you will be able to sacrifice the player to WiggleBlorp) 
Assistant (destructive would get brainwash, where you send an anonymous message to a player to invite them to the cult (destructive) side).  
Detective (destructive would get unlawful investigation, same investigation but you get the option to attack them, causing them to bleed for 1/4 hearts DOT for 3 nights) 
Medic (currently has heal, would need fake heal for destructive where it makes another player believe they were attacked and then healed) 
Everyone else gets the same ability no matter what side. 
Add in passives to two characters: Medic (immune to poison and bleed) & Clairvoyant (can't die at night, needs to be voted out). I need to make it so it doesn't give away their characters to any players though, just doesn't end up working if you attack them. 
- I want to try to make it so instead of disconnecting players, they can spectate as ghosts, but can't interact. I think this should work out as long as in all my loops that go through player list, I also check if the player is not dead before adding them to whatever.
- And add in that button to toggle player movement or not.

10/14:

**What was done**
- Health system has been fixed so all players are correctly assigned health based on their roles.
- Added controls, how to play, and roles + abilities to title screen. These are all still works in progress for UI, but the function is there.
- Fixed time between winning and losing, added fancy screen to show what players won along with what side with a cute picture. It'll send you to the main screen after 10 seconds.
- 3 abilities are in the game: Mayor's basement, Heal, and Poison.
   - Mayor's basement: (only for mayor) take a player to the basement (currently a weird looking jail) and disable all their night functions. Keep them there for a night.
   - Heal (only for medic): select a player to heal for one heart
   - Poison (only for baker): select a player to poison, where they will lose 1/4 heart over 3 nights duration.
- Added in Old Man and made him always neutral. Added in win condition for if he is killed, he wins.
- Added in the ability to be able use items in inventory (potion to heal, shield to shield for a night, and knife to attack someone.) (these are still buggy and need testing)
- Added in new UI for half a heart, quarter heart, and three-quarters hearts. Added in new health counter to incorporate these.
- assassin role was changed for just detective
- new shop items have been added (shield and knife)
- fixed properly giving gold to players every day
- attack was replaced with abilities.
- added in the mayor's basement area to spawn to.

**To do**
- Get abilities added and working, with basic UI during the night phase. Each role should get a different ability and some abilities should change based on the alignment. 
- Fix attack, but since attack will be replaced with abilities, it's not a huge deal. 
- Fix time between winning and losing, adding in a UI screen or separate scene that plays a temporary UI to show what side and what players on that side won.
- Fill shop with more items (I think I want to have 3 total):
      -  Potion of Life: Restore one heart (max 2 per player, single use)
  -  Shield: Wear for a night to prevent an attack (max 1 per player, single use, only use at night)
    - Cthulhu's knife: Deal an extra half heart damage with an attack (max 1, single use, only use at night)

- Make it so you can actually use items in your inventory and lose the item after using it. 
-Need UI for half a heart and 1/4 of a heart because abilities add this in. 
- Change assassin role name to just detective (can be a bad detective) 
- Get neutral alignment in the game, just for if the old man is there. Also add in the win condition if the old man is killed or voted out, they win.
- Add options to the main menu to include: "Controls", "Roles + Abilities" and possibly sound control. 
- Additionally, I need to figure out why it randomly will set health wrong and sometimes set it right. Also including why sometimes it properly disconnects a player and sometimes it doesn't.

  
Backlog:

- For more variety, I want some of the roles to have special passives. Such as the medic is immune to bleeding or poison and the Clairvoyant is immune to dying at night (must be voted out).
- Eventually implement voice chat (photon has apparently a plugin for this). 
- Add in characters and assign them randomly to players. Give the buff that each character gives to players and make a temporary UI for what that looks like. (Later on, that white box will be the player's character icon). I am still debating this as it is might just be feature creep. For this one, I don't think I am going to worry about adding characters unless I have extra time. 

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
