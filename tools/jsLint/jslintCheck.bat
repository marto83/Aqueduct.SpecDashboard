for /f "tokens=*" %%a in ('dir /b ..\..\src\BritishRedCross.Site\js\brc*.js') do jslint.bat ..\..\src\BritishRedCross.Site\js\%%a %*

pause