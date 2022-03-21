:: %1 - name
:: %2 - repo
:: %3 - branch
:: %4 - destination
:: %5 - flags


cd C:/
mkdir tempPublish
cd C:/tempPublish
git clone %2 .
git checkout %3
dotnet clean -v q -c Release %1
dotnet test %1
dotnet publish -o publish -c Release %1

mkdir %4
xcopy publish %4 /E /H /R /X /Y /I /K /C /O
cd C:/
rmdir /s /q tempPublish