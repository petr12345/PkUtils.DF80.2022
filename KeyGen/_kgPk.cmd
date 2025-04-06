call devenvset.bat

if "%1"=="" goto Blank
:NonBlank
sn -k Pk."%1".KeyPair.snk
sn -p Pk."%1".KeyPair.snk Pk."%1".Public.snk
sn -tp Pk."%1".Public.snk > Pk."%1".Public.txt
goto theEnd
:Blank
sn -k Pk.KeyPair.snk
sn -p Pk.KeyPair.snk Pk.Public.snk
sn -tp Pk.Public.snk > Pk.Public.txt
goto theEnd
:theEnd
