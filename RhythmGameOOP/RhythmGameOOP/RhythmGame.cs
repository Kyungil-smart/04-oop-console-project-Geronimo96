using System;
using System.Diagnostics;
using System.Threading;

namespace RhythmGameOOP
{
    // [게임 메인 로직]
    public class RhythmGame
    {
        private bool isRunning = true;
        private AudioEngine audio;
        private Stopwatch stopwatch;
        private NoteManager noteManager;
        private ScoreManager scoreManager;
        private Renderer renderer;

        public RhythmGame()
        {
            noteManager = new NoteManager();
            scoreManager = new ScoreManager();
            renderer = new Renderer();
            stopwatch = new Stopwatch();
            audio = new AudioEngine();
        }

        // [변경] 게임이 끝나면 점수 정보(ScoreManager)를 반환합니다.
        public ScoreManager Run()
        {
            renderer.Init();

            // 파일 존재 여부 확인
            if (!System.IO.File.Exists(GlobalSettings.MusicPath))
            {
                Console.Clear();
                Console.WriteLine("오류: 파일을 찾을 수 없습니다.");
                Console.ReadKey();
                return null; // 오류 시 null 반환
            }

            // 음악 재생 (볼륨 제거됨)
            try { audio.Play(GlobalSettings.MusicPath); }
            catch { }

            renderer.DrawStaticUI();
            stopwatch.Start();

            Thread gameThread = new Thread(GameLoop);
            gameThread.Start();

            HandleInput(); // 키 입력 대기

            // 게임 종료 처리
            isRunning = false;
            gameThread.Join();
            audio.Stop();

            // 게임 결과(점수판)를 반환
            return scoreManager;
        }

        private void HandleInput()
        {
            while (isRunning)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    // ESC 누르면 게임 종료
                    if (key == ConsoleKey.Escape) isRunning = false;
                    else
                    {
                        ProcessKey(key);
                    }
                }
                Thread.Sleep(1);
            }
        }

        private void ProcessKey(ConsoleKey key)
        {
            int laneIndex = Array.IndexOf(GlobalSettings.Keys, key);
            if (laneIndex == -1) return;

            Note target = noteManager.GetClosestNote(laneIndex);

            if (target != null)
            {
                float distance = Math.Abs(target.Y - GlobalSettings.HitLine);
                if (distance < 1.5f)
                {
                    scoreManager.AddScore(100, "PERFECT");
                    target.IsHit = true;
                    target.Y = GlobalSettings.TrackHeight + 5;
                }
                else if (distance < 3.0f)
                {
                    scoreManager.AddScore(50, "GOOD");
                    target.IsHit = true;
                    target.Y = GlobalSettings.TrackHeight + 5;
                }
            }
        }

        private void GameLoop()
        {
            while (isRunning)
            {
                double currentTime = stopwatch.Elapsed.TotalSeconds;
                noteManager.SpawnLogic(currentTime);
                noteManager.UpdateNotes(scoreManager);
                renderer.Draw(scoreManager, noteManager.GetNotes());
                Thread.Sleep(33);
            }
        }
    }
}