Basic example on how to use CS-Script ( http://www.csscript.net/ ) inside Unity.
With CS-Script you are able to compile and run C# scripts at runtime. This way you can ship new C# code after release without building a new version. This is also a great tool for adding mod support to 
See comments inside samplescripts for more info on how it works.

This example ships with v3.11.0.0 of CS-Script but it is advised to always use the newest stable release from http://www.csscript.net/ .
Until Unity updates its version of Mono you have to use the NET 1.1 version of CS-Script. Also, don't forget to copy Mono.CSharp.dll from the CS-Script lib folder when updating.

Please note:
CS-Script will most likely work on any platform supported by Unity, but you might be prohibited by the platform holder to actually use it.
Apple might not allow the compilation of code at runtime on iOS devices.
The same might be true for XboxOne, Playstation 4 and WiiU (i haven't checked the docs on that. And even if i did, i would not be allowed to talk about it due to NDA madness ;) ).
In any case, make sure you are allowed to use CS-Script on the platform of your choice before starting a project.

by http://www.dotmos.org

