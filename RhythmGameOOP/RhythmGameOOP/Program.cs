using System;

namespace RhythmGameOOP
{
    class Program
    {
        static void Main(string[] args)
        {
            // [초기화] 메뉴 화면 객체를 미리 만들어둡니다.
            MusicSelectScene menu = new MusicSelectScene();

            // 게임은 사용자가 종료할 때까지 계속 반복되어야 하므로 while(true)를 씁니다.
            while (true)
            {
                // 1. 메뉴 화면을 실행하고, 사용자가 선택한 곡 정보를 받아옵니다.
                // (사용자가 '종료'를 누르면 null이 반환됩니다.)
                Song selectedSong = menu.Run();

                // 사용자가 종료를 선택했다면 반복문을 탈출하여 프로그램을 끕니다.
                if (selectedSong == null)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n    게임을 종료합니다. 이용해 주셔서 감사합니다!");
                    break;
                }

                // 2. 선택된 곡의 정보(경로, BPM)를 전역 설정(GlobalSettings)에 저장합니다.
                // 이렇게 하면 게임 내 어디서든 이 정보를 쓸 수 있습니다.
                GlobalSettings.MusicPath = selectedSong.FilePath;
                GlobalSettings.BPM = selectedSong.BPM;

                // 3. 실제 게임(리듬 게임) 객체를 생성하고 실행합니다.
                RhythmGame game = new RhythmGame();

                // 게임을 플레이하고, 끝난 뒤 결과(점수, 콤보 등)를 담은 ScoreManager를 받아옵니다.
                ScoreManager finalResult = game.Run();

                // 4. 게임이 정상적으로 끝났다면(강제종료 아님) 결과를 보여줍니다.
                if (finalResult != null)
                {
                    // 목숨이 다 닳아서 죽었는지 확인
                    if (finalResult.IsDead)
                    {
                        // [A] 게임 오버 화면 출력 (점수는 보여주지 않음)
                        GameOverScene gameOver = new GameOverScene();
                        gameOver.Show();
                    }
                    else
                    {
                        // [B] 클리어 성공 시 성적표(랭크) 화면 출력
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
}