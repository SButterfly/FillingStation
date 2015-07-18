# Filling Station simulation program #

It's a simple program to simulate working process of filling station. Was made by students under software engineering course.

#####Main features:

 * Create custom filling station field
 * Move, turn, delete patterns
 * Adjust fuel consumption
 * Simulate operation of filling station with different streams of vehicles
 * Speed up or slow down simulation process
 * Get statistics of simulation process

#Version v1.1 is here!!
What's new:
* Vehicles have learned how to speed up and slow down!
* Event more their road choose became more logical
* You can use middle mouse click to copy already placed pattern
* Also fuel type would change if you scroll it
* Fixed some issues with random generators
* Fixed simulation on a high speed
* Added more bugs (as usual :D)

Comparer: v1 vs v1.1 <br>
//TODO add link
 
## How to use

######View full video by clicking on gif. <br>
<div style="text-align:center">Pattern placement: <br>
[![Patterns placement](http://share.gifyoutube.com/mLj9xq.gif)](http://www.youtube.com/watch?v=wOzE6Ihfvvc)<br>
Simulation proccess: <br>
[![Fillers arriving](http://share.gifyoutube.com/vW1VgN.gif)](http://www.youtube.com/watch?v=prnUkOdPebc) </div>
 
 //TODO add text guide

##How to try?
Direct link to download (don't worry no viruses):<br>
//TODO add link to v1.1<br><br>
Previous versions:<br>
[Filling Station v1.0.zip](https://dl.dropboxusercontent.com/u/69487763/Filling%20Station%20v1.0.zip)

## Requirements
It's C# WPF .Net program, which uses xna framework. 
So you have to install xna on your PC for application to work (https://msxna.codeplex.com).

There is one more project in solution. It is simulation kernel, which is fully xna project.
WPF doesn't support xna. That's why it's a big hack. If you make changes in kernel project, rebuild it separately. 
Otherwise kernel project won't compile.

## Developers
This project was made under software engineering course at [SSAU] in 2014-2015 and was developed by students of IT degree.

 * Sergei Ermakov ([sbutterfly@outlook.com])
 * Efim Poberezkin ([efimpoberezkin@gmail.com])
 * Michael Savachaev

### Todo's

 - [DONE] Change Access DB to use app preferences
 - [DONE] Change file reading/writing from binary to json format

[SSAU]:http://www.ssau.ru/english/
[sbutterfly@outlook.com]:mailto:sbutterfly@outlook.com?subject=Filling%20Station
[efimpoberezkin@gmail.com]:mailto:efimpoberezkin@gmail.com?subject=Filling%20Station
