﻿![banner](https://cdn.discordapp.com/attachments/402557384209203200/760175794407604265/v10397_banner.png)

**Class Helpers**

- Separated Iai Slash and Helm Breaker regen. buff timers since they stack.
- Added Hunting Horn song queue, this has been added so the user can disable the in-game health bar in the future.

![huntingHorn](https://cdn.discordapp.com/attachments/678287048200683572/755152760881676410/unknown.png)

- Added Heavy Bowgun helper, it shows the current ammo, maximum ammo, total ammo, how much ammo you can craft with the current items in your inventory, wyvernheart and wyvernsniper timers, zoom percentage and Safi'jiiva regen. counter.

![heavyBowgun](https://cdn.discordapp.com/attachments/678287048200683572/757639168506593400/unknown.png)


**Plugins**

- Added drag 'n drop way to install plugins, you can just drag the plugin "module.json" inside HunterPie and HunterPie will handle everything for you.
- Added Plugin auto-update.
- Plugins now have access to player inventory, player action id, hotkey API, etc.

**GUI**

- Added slider textbox to make it easier to set exact values.
- Added blur-behind effect to console.
- Refactored console for more performance.

**Other Changes**

- Added option to send crash logs to the dev whenever HunterPie crashes.
- Refactored updater for more performance.
- Changed **GAME_VERSION_NOT_SUPPORTED** message to be more clear.

**Bug Fixes**

- Fixed error when unloading disabled modules.
- Fixed Windows detecting zombie MHW processes and making HunterPie wait for the process infinitely.
- Fixed Presence crashing HunterPie on startup.
- Fixed memory leak when choosing class in the arena weapon selection.
- Fixed HunterPie crashing when quest timer value is invalid.
