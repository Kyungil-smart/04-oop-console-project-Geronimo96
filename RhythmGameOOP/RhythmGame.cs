using System;
using System.Diagnostics;
using System.Threading;

namespace RhythmGameOOP
{
    // [게임 메인 로직]
    public class RhythmGame
    {
        private bool isRunning = true; // 게임 실행 여부

        // 각 역할별 매니저들
        private AudioEngine audio;
        private Stopwatch stopwatch; // 시간 측정용
        private NoteManager noteManager;
        private ScoreManager scoreManager;
        private Renderer renderer;
        private double songDuration = 0; // 노래 길이

        public RhythmGame()
        {
            // 각 매니저들 생성
            noteManager = new NoteManager();
            scoreManager = new ScoreManager();
            renderer = new Renderer();
            stopwatch = new Stopwatch();
            audio = new AudioEngine();
        }

        // [게임 실행 함수]
        public ScoreManager Run()
        {
            renderer.Init(); // 화면 초기화
            renderer.DrawStaticUI(); // 배경 그리기

            // 음악 파일이 실제로 있는지 확인
            if (!System.IO.File.Exists(GlobalSettings.MusicPath))
            {
                Console.Clear();
                Console.WriteLine("오류: 음악 파일을 찾을 수 없습니다.");
                Console.ReadKey();
                return null;
            }

            // 음악 재생 및 총 길이 계산
            audio.Play(GlobalSettings.MusicPath);
            songDuration = audio.GetDuration() + 2.0; // 노래 끝나고 2초 정도 여유 줌

            // 정확한 타이밍 계산을 위해 스톱워치 시작
            stopwatch.Start();

            // 게임 루프(화면 갱신, 노트 이동)를 별도 스레드로 실행
            Thread gameThread = new Thread(GameLoop);
            gameThread.Start();

            // 메인 스레드는 키 입력만 전담해서 반응 속도를 높임
            HandleInput();

            // 키 입력 루프가 끝나면(ESC 등) 정리 작업 수행
            isRunning = false;
            gameThread.Join(); // 게임 스레드가 완전히 끝날 때까지 대기
            audio.Stop(); // 음악 정지

            return scoreManager; // 최종 점수판 반환
        }

        // [키 입력 처리 루프]
        private void HandleInput()
        {
            while (isRunning)
            {
                // 키 입력이 있을 때만 처리
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Escape) isRunning = false; // ESC 누르면 종료
                    else
                    {
                        // 게임 조작키(D,F,J,K 등)인지 확인하고 처리
                        ProcessKey(key);
                    }
                }
                Thread.Sleep(1); // CPU 점유율 폭주 방지용 대기
            }
        }

        // [키 판정 로직]
        private void ProcessKey(ConsoleKey key)
        {
            // 눌린 키가 현재 모드(4키/8키) 설정에 있는 키인지 확인
            int laneIndex = Array.IndexOf(GlobalSettings.Keys, key);
            if (laneIndex == -1) return; // 게임 키가 아니면 무시

            // 1. 시각적 이펙트 발동 (맞추든 틀리든 반응)
            renderer.TriggerEffect(laneIndex);

            // 2. 해당 라인에서 판정할 노트 가져오기
            Note target = noteManager.GetClosestNote(laneIndex);

            if (target != null)
            {
                // 판정선과 노트 사이의 거리 계산
                float distance = Math.Abs(target.Y - GlobalSettings.HitLine);

                // 거리에 따른 점수 부여
                if (distance < 1.0f) // 아주 가까움 -> PERFECT
                {
                    scoreManager.AddScore(100, "PERFECT");
                    target.IsHit = true;
                    target.Y = GlobalSettings.TrackHeight + 5; // 화면 밖으로 치워버림
                }
                else if (distance < 2.5f) // 적당히 가까움 -> GOOD
                {
                    scoreManager.AddScore(50, "GOOD");
                    target.IsHit = true;
                    target.Y = GlobalSettings.TrackHeight + 5;
                }
                // 거리가 멀면 헛친 것으로 간주 (아무 일도 안 일어남, 미스는 바닥 닿을 때 처리)
            }
        }

        // [게임 루프] 노트 이동 및 화면 그리기를 담당
        private void GameLoop()
        {
            while (isRunning)
            {
                double currentTime = stopwatch.Elapsed.TotalSeconds;

                // 종료 조건 1: 노래가 다 끝남
                if (songDuration > 0 && currentTime > songDuration) isRunning = false;

                // 종료 조건 2: 목숨이 다 닳음 (Game Over)
                if (scoreManager.IsDead) isRunning = false;

                // 1. 노트 생성
                noteManager.SpawnLogic(currentTime);
                // 2. 노트 이동 및 Miss 체크
                noteManager.UpdateNotes(scoreManager);
                // 3. 화면 그리기
                renderer.Draw(scoreManager, noteManager.GetNotes());

                Thread.Sleep(16); // 약 60 FPS (초당 60프레임) 유지
            }
        }
    }
}