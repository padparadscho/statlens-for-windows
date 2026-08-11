; SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
; SPDX-License-Identifier: AGPL-3.0-only

#define Version GetEnv("PRODUCT_VERSION")
#if Version == ""
  #error PRODUCT_VERSION environment variable must be set before compiling this script.
#endif

[Setup]
AppId={{371F8A06-42EE-475D-A4F2-634A03F9FD62}
AppName=Statlens
AppVersion={#Version}
AppPublisher=Statlens
DefaultDirName={autopf}\Statlens
DefaultGroupName=Statlens
OutputDir=bin
OutputBaseFilename=StatlensSetup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
CloseApplications=yes
RestartApplications=no
UninstallDisplayIcon={app}\Statlens.exe

[Files]
Source: "..\src\Statlens\bin\Release\net10.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Statlens"; Filename: "{app}\Statlens.exe"
Name: "{autoprograms}\Statlens"; Filename: "{app}\Statlens.exe"

[Run]
Filename: "{app}\Statlens.exe"; Description: "Launch Statlens"; Flags: nowait postinstall skipifsilent
