using System;

namespace RhythmGameOOP
{
    // [점수 관리 클래스] 점수, 콤보, 랭크 계산을 담당합니다.
    public class ScoreManager
    {
        // 외부에서 읽기만 가능(get), 수정 불가(private set)
        public int Score { get; private set; }
        public int MaxCombo { get; private set; } // 최대 콤보 기록용
        public int CurrentCombo { get; private set; }
        public string LastJudge { get; private set; } = "READY";

        // [★추가] 목숨 (기본 15개)
        public int Life { get; private set; } = 15;

        // [★추가] 게임 오버 상태 확인
        public bool IsDead => Life <= 0;

        // 점수 추가
        public void AddScore(int amount, string judgeText)
        {
            Score += amount;
            CurrentCombo++;
            LastJudge = judgeText;

            // 최대 콤보 갱신 (현재 콤보가 기록보다 높으면 갱신)
            if (CurrentCombo > MaxCombo)
            {
                MaxCombo = CurrentCombo;
            }
        }

        // 콤보 초기화 (틀렸을 때)
        public void ResetCombo()
        {
            if (Life > 0)
            {
                Life--;
            }

            //콤보 초기화 로직
            if (CurrentCombo > 0)
            {
                LastJudge = "MISS";
                Score = Math.Max(0, Score - 10);
            }
            else
            {
                LastJudge = "MISS"; // 콤보가 없어도 미스 뜨게 처리 

            }
            CurrentCombo = 0;
        }

        public string GetRank()
        {
            if (Score >= 5000) return "S"; // 기준 점수는 원하는 대로 수정하세요
            if (Score >= 3000) return "A";
            if (Score >= 1500) return "B";
            if (Score >= 500) return "C";
            return "F";
        }
    }
}