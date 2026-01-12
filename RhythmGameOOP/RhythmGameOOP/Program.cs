using System;

namespace RhythmGameOOP
{
    class Program
    {
        static void Main(string[] args)
        {
            // 메뉴 화면 객체 생성
            MusicSelectScene menu = new MusicSelectScene();

            // 무한 루프: 게임이 끝나면 다시 메뉴로 돌아옴
            while (true)
            {
                // 1. 메뉴 실행하고 선택된 곡 받아오기
                Song selectedSong = menu.Run();

                // 2. 받아온 곡이 없으면(null) 종료 선택한 것임
                if (selectedSong == null)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n    게임을 종료합니다. 안녕히 가세요!");
                    break; // 프로그램 완전 종료
                }

                // 3. 선택된 곡 정보를 전역 설정에 적용
                GlobalSettings.MusicPath = selectedSong.FilePath;
                GlobalSettings.BPM = selectedSong.BPM;

                // 4. 게임 객체 생성 후 실행 (Run)
                RhythmGame game = new RhythmGame();
                game.Run();

                // 게임 Run()이 끝나면 다시 while문의 처음으로 돌아가 메뉴가 뜸
            }
        }
    }
}