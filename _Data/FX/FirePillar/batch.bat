@set ASEPRITE="C:\Program Files (x86)\Steam\steamapps\common\Aseprite\aseprite.exe"
Rem %ASEPRITE% -b firepillar.aseprite --split-tags --save-as firepillar.gif
%ASEPRITE% -b --tag "FireOn" firepillar.aseprite --sheet  firepillar_FireOn.png
%ASEPRITE% -b --tag "Loop" firepillar.aseprite --sheet  firepillar_Loop.png
%ASEPRITE% -b --tag "FireOff" firepillar.aseprite --sheet  firepillar_FireOff.png