# Hex3Rebalance

A work-in-progress balancing mod that changes some vanilla systems, and adds features in an effort to make the game's balance more satisfying.

<a href="https://docs.google.com/document/d/1LYXD3NK8ujFzKgfpnBfxXZfSdS0FrEV_qzumdOeqln0/edit?usp=sharing">Detailed notes on the changes can be found on the Hex3Rebalance google doc.</a>

### Interactables
**Shrine Of Revelation**

![revelationImg]

Spawns in three colors: <ins>White</ins>, <ins>Green</ins> and <ins>Red</ins>. Spend a Lunar Coin to randomize all items of the corresponding tier in your inventory. Activates <ins>once</ins>.

[revelationImg]:
https://i.imgur.com/YIwWaxr.png

### Items
**9 item reworks:**

| Item  | Rework - Configurable values are <ins>underlined</ins>. |
| ------------- | ------------- |
|  | **COMMON** |  |
| ![bungusImg] | **Bustling Fungus**<br>Every **<ins>10</ins>** seconds, leave behind a fungal zone that heals **<ins>2.5</ins>%** <sup>(+<ins>2</ins>% per stack)</sup> max health every second to all allies within **<ins>4</ins>m** <sup>(+<ins>2</ins>m per stack)</sup>. |
| ![stunGrenadeImg] | **Stun Grenade**<br>**<ins>5</ins>%** <sup>(+<ins>5</ins>% per stack)</sup> chance on hit to create an explosion for **<ins>25</ins>%** base damage, which **stuns enemies** in a **<ins>1.5</ins>m** radius <sup>(+<ins>0.5</ins>m per stack)</sup>. |
|  | **UNCOMMON** |  |
| ![daisyImg] | **Lepton Daisy**<br>Release a **healing nova** during the Teleporter event, **healing** allies for **50%** of their maximum health and **weakening** enemies for **<ins>6</ins>** seconds. Occurs **1** <sup>(+1 per stack)</sup> times. |
| ![harpoonImg] | **Hunter's Harpoon**<br>Killing a boss enemy **permanently increases your movement speed** by **<ins>3</ins>%** up to a maximum of **<ins>30</ins>%** (+<ins>30</ins>% per stack). **Mountain shrines gain <ins>1</ins> stacks** <sup>(+<ins>1</ins> per stack)</sup> **of effectiveness**. |
|  | **LUNAR** |  |
| ![corpseBloomImg] | **Corpsebloom**<br>**All health regeneration is inverted**, slowly draining your HP over time <sup>(+<ins>50</ins>% drain speed per stack)</sup>. Every time you’re drained of **<ins>100</ins>%** of your max health, emit a **poisoning wave** within a **<ins>20</ins>m** <sup>(+<ins>5</ins>m per stack)</sup> radius. |
| ![focusedConvergenceImg] | **Focused Convergence**<br>Teleporters and holdout zones charge **<ins>30</ins>%** <sup>(+<ins>30</ins>% per stack)</sup> faster, **but their radius shrinks over time**. Standing outside of holdout radius **diminishes charge** by **<ins>5</ins>%** per second <sup>(+<ins>2.5</ins>% per stack)</sup>. |
| ![lightFluxImg] | **Light Flux Pauldron**<br>Gain **<ins>30</ins>%** <sup>(+<ins>30</ins>% per stack)</sup> movement speed and reduce utility skill cooldown by **<ins>50</ins>%** (-<ins>50</ins>% per stack). **Your maximum health is reduced by 50%** <sup>(-50% per stack)</sup>. |
| ![stoneFluxImg] | **Stone Flux Pauldron**<br>Increase your **maximum health** by **<ins>100</ins>%** <sup>(+<ins>100</ins>% per stack)</sup> and **enhance regeneration** by **<ins>8</ins> hp/s** <sup>(+<ins>4</ins> per stack)</sup>. **All of your healing is reduced by <ins>50</ins>%** <sup>(-<ins>50</ins>% per stack)</sup>. |
|  | **VOID** |  |
| ![needleTickImg] | **Needletick**<br>Your first hit on an enemy inflicts **Collapse**, which deals **<ins>80%</ins>** <sup>(+<ins>40</ins>% per stack)</sup> TOTAL damage after a short delay. **Corrupts all Tri-Tip Daggers.** |

[bungusImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/3/33/Bustling_Fungus.png
[stunGrenadeImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/2/27/Stun_Grenade.png
[daisyImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/7/73/Lepton_Daisy.png
[harpoonImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/c/c4/Hunter%27s_Harpoon.png
[corpseBloomImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/3/31/Corpsebloom.png
[focusedConvergenceImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/2/2c/Focused_Convergence.png
[lightFluxImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/6/6e/Light_Flux_Pauldron.png
[stoneFluxImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/b/b1/Stone_Flux_Pauldron.png
[needleTickImg]:
https://static.wikia.nocookie.net/riskofrain2_gamepedia_en/images/7/76/Needletick.png

### Gameplay
**Interactables**
* Void Cradles now prevent all healing for <ins>15 seconds</ins> after use.

# Bugs

* Please give feedback/bug reports by pinging me on the RoR2 Modding discord, or by messaging directly: hex3#7952

# Changelog

### 0.1.0
* Initial release