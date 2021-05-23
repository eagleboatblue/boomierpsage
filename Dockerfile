ARG MODE=Release

FROM mono:6.8.0.96 as builder
# https://github.com/moby/moby/issues/34482#issuecomment-454716952
ARG MODE

WORKDIR /build

COPY . .

RUN nuget restore ./Nov.Caps.Int.Erp.Sage
RUN mono ./Nov.Caps.Int.Erp.Sage/packages/NUnit.ConsoleRunner.3.11.1/tools/nunit3-console.exe $(find ./Nov.Caps.Int.Erp.Sage -name '*.Tests.dll' | grep bin)
RUN msbuild ./Nov.Caps.Int.Erp.Sage /p:Configuration=${MODE} /p:Platform=anycpu

FROM mono:6.8.0.96
ARG MODE

WORKDIR /app

COPY --from=builder /build/Nov.Caps.Int.Erp.Sage/Nov.Caps.Int.Erp.Sage.Server/bin/${MODE} .

CMD ["mono", "./Nov.Caps.Int.Erp.Sage.Server.exe"]
