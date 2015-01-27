# Filling Station simulation program #

Main features:

 * Create custom filling station field
 * Move, turn, delete patterns
 * Adjust fuel consumption
 * Simulate operation of filling station with different streams of vehicles
 * Speed up or slow down simulation process
 * Get statistics of simulation process

## Download
Direct link to zip file: [https://dl.dropboxusercontent.com/u/69487763/Filling Station v1.0.zip](https://dl.dropboxusercontent.com/u/69487763/Filling%20Station%20v1.0.zip)

## How to use
 TODO
 
## Requirements
It's WPF .Net program, which uses xna framework. 
So you have to install xna on your PC for application to work (https://msxna.codeplex.com).

## C# Solution
There is one more project in C# solution. It is simulation kernel, which is fully xna project.
WPF doesn't support xna. That's why it's a big hack. If you make changes in kernel project, rebuild it separately. 
Otherwise kernel project won't compile.

## Developers
This project was made under software engineering course at [SSAU] in 2014-2015 and was developed by students of IT degree.

 * [Sergei Ermakov] 
 * [Efim Poberezkin]
 * Michael Savachaev

### Todo's

 - Change Access DB to SQLite - done (changed datebase to app settings)
 - Change file reading/writing from binary to json format - done

[SSAU]:http://www.ssau.ru/english/
[Sergei Ermakov]:mailto:sbutterfly@hotmail.com?subject=Filling%20Station
[Efim Poberezkin]:mailto:efimpoberezkin@gmail.com?subject=Filling%20Station
