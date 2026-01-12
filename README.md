콘솔 프로젝트로 구현한 리듬 게임 입니다.
# 🎵 C# Console Rhythm Game

Windows 콘솔(터미널) 환경에서 실행되는 **4키(4-Key) 리듬 게임**입니다.
C# 언어와 객체지향(OOP) 설계를 기반으로 제작되었으며, `winmm.dll`을 활용한 오디오 재생과 더블 버퍼링 렌더링을 지원합니다.

![Game Screenshot](https://via.placeholder.com/600x300?text=Rhythm+Game+Screenshot)
*(스크린샷 자리)*

## ✨ 주요 기능 (Key Features)

* **콘솔 그래픽**: `System.Console` 만을 사용하여 리듬 게임 UI 구현
* **4키 플레이**: `D`, `F`, `J`, `K` 키를 사용하는 정통 리듬 게임 방식
* **곡 선택 시스템**: 방향키로 곡을 선택하고 엔터로 시작하는 메뉴 UI
* **결과 화면**: 게임 종료 후 점수, 최대 콤보, 랭크(S~F) 출력
* **부드러운 렌더링**: `StringBuilder`와 부분 갱신을 통한 깜빡임(Flickering) 최소화
* **오디오 엔진**: `winmm.dll`을 사용하여 `.wav` 및 `.mp3` 파일 재생 지원

## 🎮 조작 방법 (Controls)

| 상황 | 키 (Key) | 동작 |
|:---:|:---:|:---|
| **메뉴** | `↑`, `↓` | 곡 선택 이동 |
| | `Enter` | 곡 선택 / 게임 시작 |
| **인게임** | `D`, `F`, `J`, `K` | 노트 판정 (왼쪽부터 순서대로) |
| | `ESC` | 게임 종료 / 뒤로 가기 |
| **결과창** | `Enter` | 메뉴로 돌아가기 |

## 🛠️ 설치 및 실행 방법 (How to Run)

이 프로젝트는 **Visual Studio**와 **.NET** 환경에서 개발되었습니다.

1.  이 저장소를 클론(Clone)하거나 다운로드합니다.
    ```bash
    git clone [https://github.com/Kyungil-smart/04-oop-console-project-Geronimo96]
    ```
2.  `RhythmGameOOP.sln` 파일을 열어 Visual Studio를 실행합니다.
3.  **[중요]** `Songs` 폴더 내의 음악 파일 설정 확인:
    * 솔루션 탐색기에서 음악 파일 우클릭 -> **속성(Properties)**
    * `출력 디렉터리로 복사` 항목을 **`새 버전이면 복사`**로 설정해야 합니다.
4.  `Ctrl + F5`를 눌러 빌드 및 실행합니다.

## 📁 프로젝트 구조 (Project Structure)

* **Program.cs**: 프로그램 진입점 및 전체 흐름 관리
* **RhythmGame.cs**: 게임 루프 및 주요 로직 제어
* **GlobalSettings.cs**: BPM, 키 설정, 경로 등 전역 설정 관리
* **AudioEngine.cs**: `winmm.dll` 기반 오디오 재생기
* **Renderer.cs**: 더블 버퍼링 기반 콘솔 화면 출력
* **NoteManager.cs**: 노트 생성 및 이동 로직
* **ScoreManager.cs**: 점수, 콤보, 랭크 계산
* **Scenes/**: `MusicSelectScene.cs`, `ResultScene.cs` 등 화면별 클래스
* **Songs/**: 게임에 사용되는 음악 파일 저장소

## 🎵 곡 추가 방법 (Adding Songs)

새로운 노래를 추가하고 싶다면 아래 단계를 따르세요.

1.  프로젝트 내 `Songs` 폴더에 `.wav`파일을 넣습니다. (mp3 파일은 wav 파일로 변환해주시길 바랍니다.)
2.  파일 속성에서 **`출력 디렉터리로 복사: 새 버전이면 복사`** 설정을 적용합니다.
3.  `MusicSelectScene.cs` 파일을 열고 리스트에 곡을 추가합니다.

```csharp
// MusicSelectScene.cs
string path = Path.Combine(basePath, "Songs", "YourSong.wav");
songs.Add(new Song("곡 제목", path, BPM값));
