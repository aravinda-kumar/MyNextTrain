for /f %%f in ('dir /b *.xml') do xsd %%f
for /f %%g in ('dir /b *.xsd') do xsd %%g /c