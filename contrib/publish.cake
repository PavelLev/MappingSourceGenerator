var version = Argument<string>("version");
StartProcess(
    "dotnet", 
    $"pack ../src/MappingSourceGenerator/MappingSourceGenerator.csproj -c Release -o ../Packages /p:Version={version}");