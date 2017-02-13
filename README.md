# ![Alt](/logo.ico "Logo") File Obfuscator

A simple tool which obfuscates (by simple byte by byte XOR) any file to make it sendable as an e-mail attachment.
Probably only works for Windows, but could be easily changed to work with other operating systems as well.

## Installation

To install the program, run *install.bat*. This will compile the program, put it at *%ProgramFiles%\FileObfuscator\FileObfuscator.exe* and add it to the "Send to" context menu in windows. 

The project can also be opened using Visual Studio and compiled. 

## Usage

If the program was installed using *install.bat*, the program can be run by right clicking a file, choosing "Send to" followed by "File Obfuscator".
This will create a copy of the file with a *.obf* extension. To deobfuscate the file, simply do the same thing with the *.obf* file.

The program can also be run from a shell, by typing:
~~~~
FileObfuscator.exe file.exe -o
~~~~
where *file.exe* is the file to be obfuscated. The program will create a *file.exe.obf* file. To deobfuscate, simply run:
~~~~
FileObfuscator.exe file.exe.obf -o
~~~~
and the deobfuscated file will be saved as *file.exe* again.

Additionally, you can open the file directly by running:
~~~~
FileObfuscator.exe file.exe.obf
~~~~
This will make the file open with the default program for the file type.

You can also assign File Obfuscator to open .obf files in Windows, and the program will redirect the file to the default program for the file type.