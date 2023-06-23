# Offworld Dragon Supernova URP
URP variant of my Offworld project

![SVGame](https://user-images.githubusercontent.com/25352770/227788720-bbfcb18b-e46f-494a-84b5-7303f1ce76bd.png)
![image](https://user-images.githubusercontent.com/25352770/227788750-e2a668b3-eb78-434a-a05a-e346c8fb806c.png)


So far the project is an absolute mess, this is a prototype through and through.
It is intended really to further my own knowledge of unity's wackiness, such as fun shaders
and what not.

If you're interested, go hogwild with a fork. But I do warn you this is probably one of the worst and messiest
things you have ever witnessed as a programmer

This branch is a change to the current system. Instead of focusing on a living moving world, I have instead decided to make a few compromises. Why? Simply because the original seamless system was taking too much time and it caused a lot of issues for when I wanted to work on planet environment and space legs, a huge problem.

A lot of things will be changing too: There is no longer a speed limit in space and you will have to rely on proper orbits instead of just stopping at 0 velocity. The game will also be limited to one massive planet instead of multiple, allowing for an incredibly diverse planet to be created instead of trying to make multiple.

this also means that the sun now rotates around the world, rather than the other way round, stopping floating point jitters from ruining everything further. In the end, this change was made so that I'd finally stop spending too much time on the current system and actually focus on building a game. I do not have the skill to actually produce a moving world just yet, but given how large I wanted to make it, a moving world is effectively useless, as if I want yearly orbits to be noticable, planets would be moving way too quickly for the player to reach with any reasonable speed.

In short: sacrificing realism to make a game, who would've thought!

For now, I'll stick to really basic gameplay loops like cargo running and debris deorbitting. Spacelegs and EVA will finally be added and functional, alongside many other features such as a globe map system.
