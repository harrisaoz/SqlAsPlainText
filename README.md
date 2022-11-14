# Purpose

Facilitate extraction of plain text from HTML and Rich Text Format documents stored in a Microsoft SQL Server database.

# How

## Plain Text from Rich Text

Rely on the Windows.Forms.RichTextBox component, which is distributed on Windows (only) as part of .net framework (up to
4.x), .net core (2 and 3) and .NET (5+), to effect the extraction of plain text from Rich Text documents.

## Plain Text from HTML

Use FSharp.Data to parse the HTML document into a tree structure, then a bespoke visitor to extract the desired text
elements.

## Deploy to Microsoft SQL Server

- Annotate the function appropriately (SqlFunction)
- Mangle dependencies to ensure that versions are all consistent (especially FSharp.Core)
- Copy all generated DLLs to the SQL Server
- Create the assembly in SQL Server (see below).

# Limitations

This solution can only be deployed to SQL Server running on Windows, for two reasons:

- RichTextBox requires System.Windows.Forms (there's probably a possible workaround using Mono)
- SQL Server only supports dotnet external stored procedures (and functions) when running on Windows.

# Target Framework (Rationale)

The .net framework version 4.8 is targeted in order to facilitate use in the MsSqlServer CLR - albeit unsupported due to
all the dependencies being **unsupported** in the SQL Server CLR.

# Build

```powershell
dotnet build -c Release
```

# Deployment to MS SQL Server CLR

In order to create the assembly in Sql Server, the following DLL dependencies must be loaded as assemblies:

- .Net Framework libraries:
  - System.Windows.Forms
  - System.Drawing
  - System.Runtime.Serialization.Formatters.Soap
- Libraries produced by this project (all).

Simply copy these from the Microsoft.NET references
folder ```(SYSTEM_DRIVE\Windows\Microsoft.NET\Framework[64]\<version>)``` and the ```SqlAsPlainText\bin\Release\net48```
folder to a single folder (referred to below as ```<dll-folder>```) on the database server that is accessible by the Sql
Server instance, then execute the following:

```tsql
-- noinspection SqlNoDataSourceInspectionForFile
create assembly AsPlainText from '<dll-folder>/SqlAsPlainText.dll' with permission_set = unsafe;
```

At this point, you can create a function that makes use of ```SqlAsPlainText.asPlainText```. The following is a
demonstration of the syntax.

```tsql
-- noinspection SqlNoDataSourceInspectionForFile
create function [demo].asPlainText(@fileExtension nvarchar(10), @docText nvarchar(max))
returns nvarchar(max)
as external name AsPlainText.[SqlAsPlainText].asPlainText;
```
