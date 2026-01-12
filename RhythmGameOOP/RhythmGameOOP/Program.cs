using System;

namespace RhythmGameOOP
{
    class Program
    {
        static void Main(string[] args)
        {
            MusicSelectScene menu = new MusicSelectScene();

            while (true)
            {
                // 1. 메뉴 실행
                Song selectedSong = menu.Run();

                if (selectedSong == null)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n    게임을 종료합니다.");
                    break;
                }

                // 설정 적용
                GlobalSettings.MusicPath = selectedSong.FilePath;
                GlobalSettings.BPM = selectedSong.BPM;

                // 2. 게임 실행
                RhythmGame game = new RhythmGame();

                // 게임이 끝나면 점수판(ScoreManager)을 반환받습니다.
                ScoreManager finalResult = game.Run();

                // 3. 결과 화면 출력 (게임이 정상적으로 끝난 경우에만)
                if (finalResult != null)
                {
                    ResultScene resultScene = new ResultScene();
                    resultScene.Show(
                        finalResult.Score,
                        finalResult.MaxCombo,
                        finalResult.GetRank()
                    );
                }
            }
        }
    }
}