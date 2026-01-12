using System;
using System.Diagnostics; // 정밀 시간 측정을 위해
using System.Threading;   // 멀티 스레딩을 위해

namespace RhythmGameOOP
{
    public class RhythmGame
    {
        private bool isRunning = true; // 게임 실행 중 여부
        private AudioEngine audio;     // 음악 재생기
        private Stopwatch stopwatch;   // 시간 측정기

        // 각 역할별 매니저들
        private NoteManager noteManager;
        private ScoreManager scoreManager;
        private Renderer renderer;

        public RhythmGame()
        {
            // 부품 조립 (생성)
            noteManager = new NoteManager();
            scoreManager = new ScoreManager();
            renderer = new Renderer();
            stopwatch = new Stopwatch();
            audio = new AudioEngine();
        }

        // 게임 실행 함수
        public void Run()
        {
            renderer.Init();

            // [안전 장치] 파일이 실제로 존재하는지 확인
            if (!System.IO.File.Exists(GlobalSettings.MusicPath))
            {
                // 파일이 없으면 친절하게 에러 화면을 띄움
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\n    ===================================================");
                Console.WriteLine("    [오류] 음악 파일을 찾을 수 없습니다!");
                Console.WriteLine("    ===================================================");
                Console.ResetColor();
                Console.WriteLine($"\n    경로: {GlobalSettings.MusicPath}");
                Console.WriteLine("\n    1. 'Songs' 폴더 안에 파일이 있는지 확인하세요.");
                Console.WriteLine("    2. 파일 속성 '출력 디렉터리로 복사'를 설정했는지 확인하세요.");
                Console.ReadKey(); // 키를 누를 때까지 대기
                return; // 게임 시작하지 않고 종료
            }

            // 음악 재생 시도
            try { audio.Play(GlobalSettings.MusicPath, GlobalSettings.Volume); }
            catch { /* 재생 실패해도 게임은 켜지게 함 */ }

            // 배경 화면 먼저 그리기
            renderer.DrawStaticUI();
            stopwatch.Start();

            // [중요] 게임 로직(화면 갱신, 노트 이동)을 담당할 '별도 작업자(Thread)' 생성
            // 이렇게 해야 키 입력 대기(Main Thread)와 화면 갱신이 동시에 일어남
            Thread gameThread = new Thread(GameLoop);
            gameThread.Start();

            // 메인 스레드는 키 입력만 담당
            HandleInput();

            // 키 입력 루프가 끝나면(ESC) 정리 작업
            isRunning = false;
            gameThread.Join(); // 스레드 종료 대기
            audio.Stop();      // 음악 정지
        }

        // 사용자 입력 처리 (키보드)
        private void HandleInput()
        {
            while (isRunning)
            {
                // 키가 눌렸는지 확인
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    // ESC 누르면 종료
                    if (key == ConsoleKey.Escape) isRunning = false;

                    // 게임 중 볼륨 조절 (+/- 키)
                    if (key == ConsoleKey.OemPlus || key == ConsoleKey.Add)
                    {
                        GlobalSettings.Volume = Math.Min(100, GlobalSettings.Volume + 5);
                        audio.SetVolume(GlobalSettings.Volume);
                    }
                    else if (key == ConsoleKey.OemMinus || key == ConsoleKey.Subtract)
                    {
                        GlobalSettings.Volume = Math.Max(0, GlobalSettings.Volume - 5);
                        audio.SetVolume(GlobalSettings.Volume);
                    }
                    else
                    {
                        // 그 외의 키는 노트 판정 시도
                        ProcessKey(key);
                    }
                }
                // CPU 과부하 방지를 위해 1ms 대기
                Thread.Sleep(1);
            }
        }

        // 노트 판정 로직
        private void ProcessKey(ConsoleKey key)
        {
            // 누른 키가 설정된 키(D,F,J,K) 중 몇 번째인지 확인
            int laneIndex = Array.IndexOf(GlobalSettings.Keys, key);
            if (laneIndex == -1) return; // 엉뚱한 키는 무시

            // 해당 레인의 가장 아래쪽 노트 가져오기
            Note target = noteManager.GetClosestNote(laneIndex);

            if (target != null)
            {
                // 노트와 판정선 사이의 거리 계산 (절대값)
                float distance = Math.Abs(target.Y - GlobalSettings.HitLine);

                // 거리에 따른 판정
                if (distance < 1.5f) // 아주 가까움 -> Perfect
                {
                    scoreManager.AddScore(100, "PERFECT");
                    target.IsHit = true;
                    target.Y = GlobalSettings.TrackHeight + 5; // 화면 밖으로 숨김
                }
                else if (distance < 3.0f) // 조금 멈 -> Good
                {
                    scoreManager.AddScore(50, "GOOD");
                    target.IsHit = true;
                    target.Y = GlobalSettings.TrackHeight + 5;
                }
            }
        }

        // 게임 루프 (노트 이동 및 화면 그리기)
        private void GameLoop()
        {
            while (isRunning)
            {
                // 현재 시간
                double currentTime = stopwatch.Elapsed.TotalSeconds;

                // 1. 노트 생성 및 이동
                noteManager.SpawnLogic(currentTime);
                noteManager.UpdateNotes(scoreManager);

                // 2. 화면 그리기
                renderer.Draw(scoreManager, noteManager.GetNotes());

                // 3. 프레임 조절 (약 30 FPS 유지)
                Thread.Sleep(33);
            }
        }
    }
}