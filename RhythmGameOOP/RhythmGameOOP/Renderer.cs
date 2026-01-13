using System;
using System.Text;
using System.Collections.Generic;

namespace RhythmGameOOP
{
    // [화면 출력 담당 클래스]
    // 게임의 데이터를 받아 콘솔 창에 글자를 그리는 역할을 합니다.
    public class Renderer
    {
        // =========================================================
        // [상수 및 설정 변수]
        // =========================================================

        // 트랙(노트 내려오는 곳)이 시작될 Y 좌표 (위에서 4번째 줄부터 그짐)
        private const int TrackStartRow = 4;

        // [중요] 화면 왼쪽 끝에서 몇 칸 띄우고 트랙을 그릴지 결정
        // 이 값이 0이면 트랙이 왼쪽에 딱 붙고, 4면 약간 여유를 두고 그려집니다.
        private const int TrackMarginLeft = 4;

        // UI(점수, 하트 등)가 그려질 X 좌표 (Init 함수에서 계산됨)
        private int uiLeftX = 0;

        // 키를 눌렀을 때 반짝이는 이펙트를 관리하는 타이머 배열
        private int[] effectTimers;

        // [최적화 도구] 화면에 글자를 한 글자씩 찍으면 깜빡임이 심하므로,
        // 긴 문자열을 한 번에 조립해서 찍기 위해 사용하는 도구입니다.
        private StringBuilder trackBuffer = new StringBuilder();


        // =========================================================
        // [초기화 함수] 게임 시작 전 한 번 실행
        // =========================================================
        public void Init()
        {
            // 커서(깜빡이는 밑줄)를 숨겨서 게임 화면을 깔끔하게 만듭니다.
            Console.CursorVisible = false;

            // -----------------------------------------------------
            // [1. 트랙 너비 자동 계산 로직]
            // 노트 1칸의 구성: "공백7칸" + "구분선(|)" = 총 8칸
            // 맨 왼쪽 시작 벽(|) = 1칸
            // 예: 4키 모드 = (4 * 8) + 1 = 33칸
            // 예: 8키 모드 = (8 * 8) + 1 = 65칸
            // -----------------------------------------------------
            int laneCount = GlobalSettings.LaneCount;
            int trackWidth = (laneCount * 8) + 1;

            // [2. UI 위치 잡기]
            // 트랙이 끝나는 지점(Margin + Width)에서 5칸 더 오른쪽으로 띄움
            uiLeftX = TrackMarginLeft + trackWidth + 5;

            // [3. 창 크기 설정]
            // UI가 표시될 위치에 넉넉하게 30칸을 더해서 창 너비를 정함
            int windowWidth = uiLeftX + 30;

            try
            {
                // 창 너비가 너무 좁으면(60 미만) 강제로 60으로 맞춤 (오류 방지)
                if (windowWidth < 60) windowWidth = 60;

                // 계산된 크기로 콘솔 창 크기 변경
                Console.SetWindowSize(windowWidth, 32);
            }
            catch
            {
                // 윈도우 설정이나 권한 문제로 창 크기 변경 실패 시 무시하고 진행
            }

            // 이펙트 타이머 배열 생성 (키 개수만큼)
            effectTimers = new int[GlobalSettings.LaneCount];

            // 창 크기 변경이 적용될 시간을 잠깐 줌
            System.Threading.Thread.Sleep(100);
            Console.Clear(); // 화면을 깨끗하게 지움
        }


        // =========================================================
        // [이펙트 발동 함수]
        // =========================================================
        public void TriggerEffect(int lane)
        {
            // 해당 키(lane)의 이펙트 타이머를 3으로 설정 (3프레임 동안 빛남)
            if (lane >= 0 && lane < effectTimers.Length)
            {
                effectTimers[lane] = 3;
            }
        }


        // =========================================================
        // [배경 그리기] 점수판 틀, 트랙 테두리 등 변하지 않는 것들
        // 매 프레임 그리면 낭비이므로 게임 시작 시 딱 한 번만 호출합니다.
        // =========================================================
        public void DrawStaticUI()
        {
            Console.Clear();

            // 트랙 너비 다시 계산 (지역 변수)
            int laneCount = GlobalSettings.LaneCount;
            int trackWidth = (laneCount * 8) + 1;

            // -----------------------------------------------------
            // 1. 최상단 정보창 (SCORE, COMBO 표시 영역)
            // -----------------------------------------------------
            string topBar = new string('=', trackWidth + 20); // '=' 문자를 길게 반복 생성
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(topBar);
            Console.WriteLine($" SCORE: 00000  |  COMBO: 0"); // 초기값 0 출력
            Console.WriteLine($" JUDGE: READY");
            Console.WriteLine(topBar);

            // -----------------------------------------------------
            // 2. 트랙 상단 뚜껑 (테두리)
            // -----------------------------------------------------
            Console.SetCursorPosition(TrackMarginLeft, TrackStartRow - 1);
            string trackBorder = new string('=', trackWidth); // 트랙 너비만큼 '=' 생성
            Console.Write(trackBorder);

            // -----------------------------------------------------
            // 3. 빈 트랙 미리 그리기 (기둥 세우기)
            // -----------------------------------------------------
            string emptyLine = "|"; // [시작 벽]
            for (int i = 0; i < laneCount; i++)
            {
                emptyLine += "       |"; // [공백 7칸] + [구분선 1칸]
            }

            // 트랙 높이(20줄)만큼 반복해서 빈 줄 출력
            for (int i = 0; i < GlobalSettings.TrackHeight; i++)
            {
                Console.SetCursorPosition(TrackMarginLeft, TrackStartRow + i);
                Console.Write(emptyLine);
            }

            // -----------------------------------------------------
            // 4. 트랙 바닥 테두리 & 키 가이드 (D F J K)
            // -----------------------------------------------------
            int bottomRow = TrackStartRow + GlobalSettings.TrackHeight;

            // 바닥 선
            Console.SetCursorPosition(TrackMarginLeft, bottomRow);
            Console.Write(trackBorder);

            // 키 이름 출력 (예: |   D   |   F   | ...)
            Console.SetCursorPosition(TrackMarginLeft, bottomRow + 1);
            string keyLine = "|";
            foreach (var key in GlobalSettings.Keys)
            {
                // 키 이름 가져오기 (Spacebar 같은 건 너무 기니까 앞글자만 따옴)
                string keyName = key.ToString();
                if (keyName.Length > 1) keyName = keyName.Substring(0, 1);

                // 가운데 정렬해서 붙이기
                keyLine += $"   {keyName}   |";
            }
            Console.Write(keyLine);

            // 키 가이드 밑줄
            Console.SetCursorPosition(TrackMarginLeft, bottomRow + 2);
            Console.Write(trackBorder);

            // -----------------------------------------------------
            // 5. 우측 UI (생명력, 볼륨) 초기 텍스트
            // -----------------------------------------------------
            Console.SetCursorPosition(uiLeftX, TrackStartRow);
            Console.Write("[ LIFE ]");

        }


        // =========================================================
        // [메인 그리기] 매 프레임(0.03초마다) 호출되어 변화를 그림
        // =========================================================
        public void Draw(ScoreManager scoreMgr, IReadOnlyList<Note> notes)
        {
            // -----------------------------------------------------
            // 1. 점수 정보 갱신 (화면 상단)
            // -----------------------------------------------------
            Console.SetCursorPosition(8, 1);
            Console.Write(scoreMgr.Score.ToString("D5")); // 00100 처럼 5자리 맞춤

            Console.SetCursorPosition(25, 1);
            Console.Write(scoreMgr.CurrentCombo.ToString().PadRight(4)); // 잔상 지우기용 공백

            Console.SetCursorPosition(8, 2);
            Console.Write(scoreMgr.LastJudge.PadRight(10)); // PERFECT, MISS 등 출력


            // -----------------------------------------------------
            // 2. 하트(생명력) 그리기
            // -----------------------------------------------------
            DrawHearts(scoreMgr.Life);

            // -----------------------------------------------------
            // 3. 트랙과 노트 그리기 (가장 중요한 부분)
            // -----------------------------------------------------

            // StringBuilder 초기화 (쓰레기 값 제거)
            trackBuffer.Clear();

            // 한 줄(Row)씩 내려가면서 그립니다.
            for (int y = 0; y < GlobalSettings.TrackHeight; y++)
            {
                trackBuffer.Clear(); // 줄 바뀔 때마다 버퍼 비움

                // [★핵심] 맨 왼쪽 벽(|)을 먼저 버퍼에 넣습니다.
                // 아까 불필요한 공백이 생겼던 건 여기서 "   |" 처럼 공백을 넣어서였습니다.
                trackBuffer.Append("|");

                // 왼쪽 레인부터 오른쪽 레인까지 반복 (x: 0, 1, 2, 3...)
                for (int x = 0; x < GlobalSettings.LaneCount; x++)
                {
                    string shape = "       "; // 기본 모양: 빈 공백 7칸

                    // ----------------------------------
                    // A. 판정선(HitLine) 그리기 로직
                    // ----------------------------------
                    if (y == GlobalSettings.HitLine)
                    {
                        // 키를 눌러서 이펙트 타이머가 켜져 있다면?
                        if (effectTimers[x] > 0)
                        {
                            shape = " [###] "; // 눌렀을 때 모양 (반짝!)
                            effectTimers[x]--; // 타이머 시간 감소 (점점 꺼짐)
                        }
                        else
                        {
                            shape = "-------"; // 평소 판정선 모양
                        }
                    }

                    // ----------------------------------
                    // B. 노트 그리기 로직
                    // ----------------------------------
                    foreach (var note in notes)
                    {
                        // 1. 현재 검사 중인 레인(x)에 있는 노트인지?
                        // 2. 아직 맞추지 않은(IsHit == false) 노트인지?
                        if (note.LaneIndex == x && !note.IsHit)
                        {
                            // 3. 노트의 Y좌표가 현재 그리는 줄(y)과 비슷한지? (오차 0.6칸 이내)
                            // float 좌표를 int 줄에 그리기 위해 근사값 체크
                            if (Math.Abs(note.Y - y) < 0.6f)
                                shape = "  (O)  "; // 노트 모양 덮어쓰기
                        }
                    }

                    // 완성된 7칸 모양 + 오른쪽 벽(|)을 버퍼에 추가
                    trackBuffer.Append(shape + "|");
                }

                // ----------------------------------
                // C. 완성된 한 줄을 화면에 출력
                // ----------------------------------
                // [중요] 왼쪽 여백(MarginLeft)만큼 커서를 옮긴 뒤 출력해야 칸이 딱 맞음
                Console.SetCursorPosition(TrackMarginLeft, TrackStartRow + y);
                Console.Write(trackBuffer.ToString());
            }
        }

        // =========================================================
        // [하트 그리기 함수]
        // =========================================================
        private void DrawHearts(int life)
        {
            // 계산된 UI 위치(uiLeftX)로 이동해서 그립니다.
            Console.SetCursorPosition(uiLeftX, TrackStartRow + 2);
            Console.ForegroundColor = ConsoleColor.Red; // 빨간색

            for (int i = 0; i < 15; i++) // 최대 체력 15칸
            {
                if (i < life)
                    Console.Write("♥ "); // 남은 목숨
                else
                    Console.Write("  ");  // 잃은 목숨 (빈칸으로 덮어써서 지움)
            }
            Console.ResetColor(); // 색상 복구
        }
    }
}