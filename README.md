# Filling Station simulation program #

It's a simple program to simulate working process of filling station.

Main features:

 * Create custom filling station field
 * Move, turn, delete patterns
 * Adjust fuel consumption
 * Simulate operation of filling station with different streams of vehicles
 * Speed up or slow down simulation process
 * Get statistics of simulation process

## Download
Direct link to try program: [https://dl.dropboxusercontent.com/u/69487763/Filling Station v1.0.zip](https://dl.dropboxusercontent.com/u/69487763/Filling%20Station%20v1.0.zip)

## How to use

View full version by clicking on gif.

| [![Patterns placement](http://share.gifyoutube.com/mLj9xq.gif)](http://www.youtube.com/watch?v=wOzE6Ihfvvc) | [![Setup field](http://share.gifyoutube.com/yxGqYP.gif)](http://www.youtube.com/watch?v=pixa8_0EZfg)        |
|-------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------|
| [![Moving patterns](http://share.gifyoutube.com/mGMwJ1.gif)](http://www.youtube.com/watch?v=U5F7oigGeI4)    | [![Process simulation](http://share.gifyoutube.com/ya6rM5.gif)](http://www.youtube.com/watch?v=HHO79XUu5_Q) |
| [![Fillers arriving](http://share.gifyoutube.com/vW1VgN.gif)](http://www.youtube.com/watch?v=prnUkOdPebc)   | [![Collector arriving](http://share.gifyoutube.com/vM0a1b.gif)](http://www.youtube.com/watch?v=UTTcqnGOoV4) |
 
## Requirements
It's WPF .Net program, which uses xna framework. 
So you have to install xna on your PC for application to work (https://msxna.codeplex.com).

## C# Solution
There is one more project in C# solution. It is simulation kernel, which is fully xna project.
WPF doesn't support xna. That's why it's a big hack. If you make changes in kernel project, rebuild it separately. 
Otherwise kernel project won't compile.

## Developers
This project was made under software engineering course at [SSAU] in 2014-2015 and was developed by students of IT degree.

 * Sergei Ermakov ([sbutterfly@outlook.com])
 * Efim Poberezkin ([efimpoberezkin@gmail.com])
 * Michael Savachaev

### Todo's

 - Change Access DB to SQLite - done (changed datebase to app settings)
 - Change file reading/writing from binary to json format - done

[SSAU]:http://www.ssau.ru/english/
[sbutterfly@outlook.com]:mailto:sbutterfly@outlook.com?subject=Filling%20Station
[efimpoberezkin@gmail.com]:mailto:efimpoberezkin@gmail.com?subject=Filling%20Station
