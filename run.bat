pushd %~dp0
    staradmin stop host
    star --resourceDir=Playground/wwwroot Playground/bin/Debug/Playground.exe
    star --resourceDir=Subground/wwwroot Subground/bin/Debug/Subground.exe
popd