# RollingBeads

직선 왕복 운동만으로 원운동처럼 보이는 착시(Tusi couple)를 만들어 내는 시뮬레이션 앱입니다.

중심점을 기준으로 `180° / N` 간격으로 기울어진 N개의 직선 위에서 구슬들이 단순 왕복 운동을 합니다. 각 구슬의 출발 시점에 위상차를 주면, 전체 구슬들이 마치 원을 그리며 구르는 것처럼 보입니다.

- 구슬(직선) 개수와 한 사이클의 주기(초)를 조절할 수 있습니다.
- 가이드 라인 표시를 켜면 각 구슬이 실제로는 직선 위에서만 움직인다는 것을 확인할 수 있습니다.

## 기술 스택

| 구성 | 내용 |
| --- | --- |
| UI 프레임워크 | [Uno Platform](https://platform.uno/) 6.6 (WinUI XAML) |
| 런타임 | .NET 9 / WebAssembly ([Uno.Wasm.Bootstrap](https://github.com/unoplatform/Uno.Wasm.Bootstrap) 9.x) |
| 배포 | Azure Static Web Apps (GitHub Actions) |

## 프로젝트 구조

| 프로젝트 | 설명 |
| --- | --- |
| `RollingBeads` | 앱 본체. 구슬 운동 모델(`Models/Bead`, `Models/BeadCollection`)과 화면(`Presentation/MainPage`) |
| `RollingBeads.Wasm` | WebAssembly 헤드 (`Microsoft.NET.Sdk.WebAssembly`) |
| `RollingBeads.Shared` | 헤드 공통 리소스 (AppHead, 아이콘, 스플래시) |
| `RollingBeads.DataContracts` | 데이터 계약 |

## 빌드 및 실행

### 사전 요구 사항

- .NET SDK 9.0.3xx ([global.json](global.json)에 고정)
- wasm-tools 워크로드

```bash
dotnet workload install wasm-tools
```

### 명령줄에서 실행

```bash
dotnet run --project RollingBeads.Wasm
```

브라우저가 `http://localhost:5001/index.html`로 자동으로 열립니다.

### Visual Studio에서 실행

1. `RollingBeads.sln`을 엽니다.
2. `RollingBeads.Wasm`을 시작 프로젝트로 설정합니다.
3. 실행 프로필 `RollingBeads.Wasm`(IIS Express 아님)을 선택하고 F5를 누릅니다.

### 배포용 게시

```bash
dotnet publish RollingBeads.Wasm -c Release
```

정적 사이트가 `RollingBeads.Wasm/bin/Release/net9.0/publish/wwwroot`에 생성되며, `master` 브랜치에 푸시하면 [GitHub Actions 워크플로](.github/workflows/azure-static-web-apps-kind-rock-01d216600.yml)가 Azure Static Web Apps로 자동 배포합니다.

## 버전 관리 메모

- 프레임워크 버전은 [Directory.Build.props](Directory.Build.props)의 `DotNetVersion` 한 곳에서 관리합니다.
- 패키지 버전은 Central Package Management([Directory.Packages.props](Directory.Packages.props))로 관리합니다.
- Uno.Sdk 버전([global.json](global.json))을 올릴 때는 [Directory.Packages.props](Directory.Packages.props)의 `UnoVersion` 기본값(Wasm 헤드용)을 함께 맞춰야 합니다.
