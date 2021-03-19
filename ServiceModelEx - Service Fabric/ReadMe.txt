You will need the Moq.dll assembly for compiling your tests. 

You can download the assembly from Nuget and install it:
http://www.nuget.org/packages/moq

You can also download Moq.dll and install it in the GAC so you could add reference to it: 
gacutil /i Moq.dll

Finally, you can Moq.dll assembly from Nuget by placing in the root a file called packages.config
with this reference inside: 

<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="Moq" version="4.2.1510.2205" targetFramework="net46" />
</packages>
