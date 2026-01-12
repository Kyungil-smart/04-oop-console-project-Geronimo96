using System;
using System.Text;
using System.Collections.Generic;

namespace RhythmGameOOP
{
    public class Renderer
    {
        // 화면 좌표 상수 (어디에 점수를 찍을지)
        private const int ScoreRow = 1;
        private const int JudgeRow = 2;
        private const int TrackStartRow = 4;

        // 문자열을 조립할 임시 공간 (StringBuilder)
        private StringBuilder trackBuffer = new StringBuilder();

        public void Init()
        {
            Console.CursorVisible = false;
            try
            {
                // 1. 창 크기 설정 (70칸)
                Console.SetWindowSize(70, 32);
            }
            catch { }

            // 2. 윈도우가 적용할 시간 주기
            System.Threading.Thread.Sleep(100);

            // 3. 화면 싹 지우기
            Console.Clear();
        }

        // [최적화 1] 변하지 않는 배경과 테두리는 게임 시작 때 한 번만 그린다.
        public void DrawStaticUI()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine(" SCORE: 00000  |  COMBO: 0"); // 초기 텍스트
            Console.WriteLine(" JUDGE: READY");
            Console.WriteLine("========================================");

            // 빈 트랙 미리 그리기
            for (int i = 0; i < GlobalSettings.TrackHeight; i++)
            {
                Console.WriteLine("   |                           |");
            }

            Console.WriteLine("========================================");
            Console.WriteLine("    |   D   |   F   |   J   |   K   |");
            Console.WriteLine("========================================");

            // [★추가] 하트 UI 설명 텍스트
            Console.SetCursorPosition(45, 3);
            Console.Write("[ LIFE ]");
        }

        // [최적화 2] 매 프레임 변하는 숫자와 노트만 커서를 옮겨 덮어쓴다.
        public void Draw(ScoreManager scoreMgr, IReadOnlyList<Note> notes)
        {
            // 1. 점수와 콤보 갱신
            Console.SetCursorPosition(8, ScoreRow);
            Console.Write(scoreMgr.Score.ToString("D5"));

            Console.SetCursorPosition(25, ScoreRow);
            // PadRight: 숫자가 줄어들 때 잔상이 남지 않게 뒤에 공백 추가
            Console.Write(scoreMgr.CurrentCombo.ToString().PadRight(4));

            Console.SetCursorPosition(8, JudgeRow);
            Console.Write(scoreMgr.LastJudge.PadRight(10));

            // [★추가] 오른쪽에 하트 그리기
            DrawHearts(scoreMgr.Life);

            // 2. 트랙(노트) 갱신
            trackBuffer.Clear();
            for (int y = 0; y < GlobalSettings.TrackHeight; y++)
            {
                trackBuffer.Clear(); // 줄마다 버퍼 비우기

                for (int x = 0; x < GlobalSettings.LaneCount; x++)
                {
                    string shape = "       "; // 기본은 빈 공간

                    // 판정선 위치 표시
                    if (y == GlobalSettings.HitLine) shape = "-------";

                    // 현재 좌표(x, y)에 노트가 있는지 검사
                    foreach (var note in notes)
                    {
                        if (note.LaneIndex == x && !note.IsHit)
                        {
                            // 노트의 Y좌표와 현재 그리는 줄(y)이 가까우면 노트 그리기
                            if (Math.Abs(note.Y - y) < 0.6f)
                                shape = "  [O]  ";
                        }
                    }
                    trackBuffer.Append(shape + "|"); // 칸 구분선 추가
                }

                // 완성된 한 줄을 화면의 정확한 위치에 덮어쓰기
                Console.SetCursorPosition(4, TrackStartRow + y);
                Console.Write(trackBuffer.ToString());
            }
        }
        // [★추가] 하트 그리는 함수
        private void DrawHearts(int life)
        {
            Console.SetCursorPosition(45, 5); // 오른쪽 위치 잡기

            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < 10; i++) // 최대 10개
            {
                if (i < life) Console.Write("♥ "); // 남은 목숨
                else Console.Write("  ");       // 깎인 목숨은 빈칸
            }
            Console.ResetColor();
        }
    }
}