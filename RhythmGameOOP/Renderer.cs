using System;
using System.Text;
using System.Collections.Generic;

namespace RhythmGameOOP
{
    public class Renderer
    {
        // 트랙을 그리기 시작할 화면 Y 좌표
        private const int TrackStartRow = 4;

        // 화면 왼쪽 여백 (너무 왼쪽에 붙으면 보기 싫어서 띄움)
        private const int TrackMarginLeft = 4;

        // UI(점수, 하트 등)가 그려질 X 좌표 (Init 함수에서 자동 계산됨)
        private int uiLeftX = 0;

        // 키를 눌렀을 때 반짝이는 이펙트 시간을 저장하는 배열
        private int[] effectTimers;

        // [최적화 도구] 화면을 한 글자씩 찍으면 깜빡거리므로, 
        // 문자열을 미리 조립(StringBuilder)해서 한 번에 찍기 위해 사용합니다.
        private StringBuilder trackBuffer = new StringBuilder();

        // [초기화 함수]
        public void Init()
        {
            Console.CursorVisible = false; // 커서 깜빡임 숨김

            // 1. 현재 모드(4키/8키)에 따라 트랙 너비 계산
            // 노트 1칸당 8글자(공백7+선1) + 마지막 닫는 선 1글자
            int laneCount = GlobalSettings.LaneCount;
            int trackWidth = (laneCount * 8) + 1;

            // 2. UI 위치와 전체 창 너비 계산
            uiLeftX = TrackMarginLeft + trackWidth + 5;
            int windowWidth = uiLeftX + 30; // UI 오른쪽에 여유 공간 30칸 확보

            try
            {
                // 창 너비가 너무 작으면 깨지므로 최소값(60) 보정
                if (windowWidth < 60) windowWidth = 60;

                // 계산된 크기로 콘솔 창 크기 변경
                Console.SetWindowSize(windowWidth, 32);
            }
            catch { /* 콘솔 권한 문제 등으로 실패 시 무시 */ }

            // 이펙트 타이머 배열 초기화
            effectTimers = new int[GlobalSettings.LaneCount];

            // 창 크기 변경이 적용될 시간을 잠깐 줌
            System.Threading.Thread.Sleep(100);
            Console.Clear();
        }

        // [이펙트 발동] 키를 눌렀을 때 호출됨
        public void TriggerEffect(int lane)
        {
            if (lane >= 0 && lane < effectTimers.Length)
            {
                effectTimers[lane] = 3; // 3프레임 동안 이펙트 유지
            }
        }

        // [정적 UI 그리기] 매 프레임 변하지 않는 배경과 테두리를 그림
        public void DrawStaticUI()
        {
            Console.Clear();

            int trackWidth = (GlobalSettings.LaneCount * 8) + 1;
            string topBar = new string('=', trackWidth + 20); // 긴 가로선 생성

            // 상단 점수판 틀
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(topBar);
            Console.WriteLine($" SCORE: 00000  |  COMBO: 0");
            Console.WriteLine($" JUDGE: READY");
            Console.WriteLine(topBar);

            // 트랙 상단 테두리
            Console.SetCursorPosition(TrackMarginLeft, TrackStartRow - 1);
            Console.Write(new string('=', trackWidth));

            // 빈 트랙(기둥) 미리 그리기
            string emptyLine = "|";
            for (int i = 0; i < GlobalSettings.LaneCount; i++) emptyLine += "       |";

            for (int i = 0; i < GlobalSettings.TrackHeight; i++)
            {
                Console.SetCursorPosition(TrackMarginLeft, TrackStartRow + i);
                Console.Write(emptyLine);
            }

            // 하단 테두리 및 키 가이드
            int bottomRow = TrackStartRow + GlobalSettings.TrackHeight;
            Console.SetCursorPosition(TrackMarginLeft, bottomRow);
            Console.Write(new string('=', trackWidth));

            // 키 이름 표시 (예: | D | F | J | K |)
            Console.SetCursorPosition(TrackMarginLeft, bottomRow + 1);
            string keyLine = "|";
            foreach (var key in GlobalSettings.Keys)
            {
                string k = key.ToString();
                if (k.Length > 1) k = k.Substring(0, 1); // 너무 길면 첫 글자만
                keyLine += $"   {k}   |";
            }
            Console.Write(keyLine);

            Console.SetCursorPosition(TrackMarginLeft, bottomRow + 2);
            Console.Write(new string('=', trackWidth));

            // 우측 UI 제목 (생명력)
            Console.SetCursorPosition(uiLeftX, TrackStartRow);
            Console.Write("[ LIFE ]");
        }

        // [메인 그리기] 매 프레임 호출되어 움직이는 것들(노트, 점수, 이펙트)을 그림
        public void Draw(ScoreManager scoreMgr, IReadOnlyList<Note> notes)
        {
            // 1. 점수 정보 갱신
            Console.SetCursorPosition(8, 1);
            Console.Write(scoreMgr.Score.ToString("D5")); // 00000 형식 맞춤
            Console.SetCursorPosition(25, 1);
            Console.Write(scoreMgr.CurrentCombo.ToString().PadRight(4)); // 잔상 지우기
            Console.SetCursorPosition(8, 2);
            Console.Write(scoreMgr.LastJudge.PadRight(10));

            // 2. 하트(생명력) 그리기
            DrawHearts(scoreMgr.Life);

            // 3. 트랙 및 노트 그리기
            for (int y = 0; y < GlobalSettings.TrackHeight; y++)
            {
                trackBuffer.Clear();
                trackBuffer.Append("|"); // 시작 벽

                for (int x = 0; x < GlobalSettings.LaneCount; x++)
                {
                    string shape = "       "; // 기본은 빈 공간

                    // [판정선 및 이펙트 처리]
                    if (y == GlobalSettings.HitLine)
                    {
                        if (effectTimers[x] > 0)
                        {
                            shape = " [###] "; // 이펙트 발동 중
                            effectTimers[x]--; // 시간 감소
                        }
                        else
                        {
                            shape = "-------"; // 평소 판정선
                        }
                    }

                    // [노트 그리기]
                    foreach (var note in notes)
                    {
                        if (note.LaneIndex == x && !note.IsHit)
                        {
                            // 노트의 Y좌표와 현재 그리는 줄(y)이 가까우면(0.6 오차 내) 그림
                            if (Math.Abs(note.Y - y) < 0.6f)
                                shape = "  (O)  ";
                        }
                    }
                    trackBuffer.Append(shape + "|"); // 칸 닫기
                }

                // 완성된 한 줄을 화면에 출력
                Console.SetCursorPosition(TrackMarginLeft, TrackStartRow + y);
                Console.Write(trackBuffer.ToString());
            }
        }

        // 하트 그리기 함수
        private void DrawHearts(int life)
        {
            Console.SetCursorPosition(uiLeftX, TrackStartRow + 2);
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < 15; i++) // 최대 15칸
            {
                if (i < life) Console.Write("♥ ");
                else Console.Write("  "); // 잃은 목숨은 빈칸으로 덮어씀
            }
            Console.ResetColor();
        }
    }
}