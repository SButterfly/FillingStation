# Filling Station simulation program #

Main features:

 * Create custom filling station view
 * Move, turn, delete patterns
 * Change prices, tank's volumes, skins of cars.
 * Simulate working process on different types of input vehicles streams
 * Speed up or slow down simulation process
 * Use statistics for your own purposes

## Download
There are two ways to try this project:
 - simple zip file (without db connection): TODO add link
 - setup file with additional [Microsoft Access Database Engine]: TODO add link

## How to use
 TODO
 
## Before install
It's WPF .Net program, which uses xna framework and Microsoft Access DB.
Solution won't work if you have not install xna on your computer (https://msxna.codeplex.com).
Program will work without connection to db. So it's not required to install special db accessor. 
But if you want full functionality you must install [Microsoft Access Database Engine].

## C# Solution
There is one more project in C# solution. It is simulation kernel, which is fully xna project.
WPF doesn't support xna. That's why it's a big hack. If you make changes in kernel project, rebuild it separately. 
Otherwise kernel project won't compile.

## Credits
This work was made under software engineering course at [SSAU] in 2014-2015.
All credits were students of IT degree.

 * [Sergei Ermakov] 
 * Efim Poberezkin
 * Michael Savachaev

### Todo's

 - Change Access DB to SQLite
 - Change file reading/writing from binary to json format

[SSAU]:http://www.ssau.ru/english/
[Microsoft Access Database Engine]:http://www.microsoft.com/en-us/download/details.aspx?id=13255
[Sergei Ermakov]:mailto:sbutterfly@hotmail.com
