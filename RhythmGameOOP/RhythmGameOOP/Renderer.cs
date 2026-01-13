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

        // [신규 기능] 각 레인별 이펙트 지속 시간 (0이면 꺼짐, 숫자가 있으면 켜짐)
        private int[] effectTimers = new int[4];

        // 문자열을 조립할 임시 공간 (StringBuilder)
        private StringBuilder trackBuffer = new StringBuilder();

        public void Init()
        {
            Console.CursorVisible = false;
            try
            {
                // [수정] 1. 하드모드면 창을 더 넓게 설정 (90칸), 아니면 70칸
                int width = GlobalSettings.IsHardMode ? 90 : 70;
                Console.SetWindowSize(width, 32);
            }
            catch { }

            // 이펙트 타이머도 8개까지 커버되도록 재할당
            effectTimers = new int[8];

            // 2. 윈도우가 적용할 시간 주기
            System.Threading.Thread.Sleep(100);

            // 3. 화면 싹 지우기
            Console.Clear();
        }

        // [신규 기능] 외부에서 이 함수를 부르면 해당 레인이 번쩍임!
        public void TriggerEffect(int lane)
        {
            if (lane >= 0 && lane < 4)
            {
                effectTimers[lane] = 3; // 3프레임 동안 유지
            }
        }

        // [최적화 1] 변하지 않는 배경과 테두리는 게임 시작 때 한 번만 그린다.
        public void DrawStaticUI()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine(" SCORE: 00000  |  COMBO: 0"); // 초기 텍스트
            Console.WriteLine(" JUDGE: READY");
            Console.WriteLine("========================================");

            // 트랙 빈 공간 그리기
            for (int i = 0; i < GlobalSettings.TrackHeight; i++)
            {
                // 레인 개수만큼 반복해서 파이프(|)를 그립니다.
                string line = "   |";
                for (int k = 0; k < GlobalSettings.LaneCount; k++)
                {
                    line += "                           |"; // 칸 너비 맞춤
                }
                // 여기서는 실제 모양이 loop에서 그려지므로 생략하고, Draw 함수에서 처리하게 둠
            }

            // [수정] 바닥 키 가이드 출력 (동적으로 변경)
            Console.WriteLine("========================================");
            if (GlobalSettings.IsHardMode)
            {
                Console.WriteLine("    | A | S | D | F | H | J | K | L |");
            }
            else
            {
                Console.WriteLine("    |   D   |   F   |   J   |   K   |");
            }
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

                    // 판정선 그리기 & 이펙트 처리
                    if (y == GlobalSettings.HitLine)
                    {
                        // [핵심] 이펙트 타이머가 남아있으면 빛나는 모양으로 그림
                        if (effectTimers[x] > 0)
                        {
                            shape = " [###] "; // 눌렀을 때 모양
                            effectTimers[x]--; // 시간 감소
                        }
                        else
                        {
                            shape = "-------"; // 평소 모양
                        }
                    }

                    // 노트 그리기 (이펙트 위에 덮어씌워지지 않게 주의)
                    foreach (var note in notes)
                    {
                        if (note.LaneIndex == x && !note.IsHit)
                        {
                            if (Math.Abs(note.Y - y) < 0.6f)
                                shape = "  (O)  "; // 노트 모양
                        }
                    }
                    trackBuffer.Append(shape + "|");
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