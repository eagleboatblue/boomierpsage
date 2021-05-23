default: build start

start:
	dotenv mono ./Nov.Caps.Int.Erp.Sage/Nov.Caps.Int.Erp.Sage.Server/bin/Debug/Nov.Caps.Int.Erp.Sage.Server.exe

build:
	msbuild ./Nov.Caps.Int.Erp.Sage