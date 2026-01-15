using System;

namespace RhythmGameOOP
{
    public class ScoreManager
    {
        // 외부에서는 읽기만 가능하고 수정은 불가능하게 설정 (보안)
        public int Score { get; private set; }
        public int MaxCombo { get; private set; }
        public int CurrentCombo { get; private set; }
        public string LastJudge { get; private set; } = "READY";

        // 생명력 (기본 15칸)
        public int Life { get; private set; } = 15;

        // 생명력이 0 이하면 죽음 상태
        public bool IsDead => Life <= 0;

        // [점수 획득 함수]
        public void AddScore(int amount, string judgeText)
        {
            Score += amount;
            CurrentCombo++;
            LastJudge = judgeText;

            // 최대 콤보 갱신
            if (CurrentCombo > MaxCombo) MaxCombo = CurrentCombo;

            // 보너스: 콤보 10 단위마다 생명력 1 회복 (최대 15)
            if (CurrentCombo % 10 == 0 && Life < 15) Life++;
        }

        // [미스 처리 함수]
        public void ResetCombo()
        {
            // 생명력 감소
            if (Life > 0) Life--;

            // 콤보가 있었다면 점수 차감 패널티
            if (CurrentCombo > 0)
            {
                LastJudge = "MISS";
                Score = Math.Max(0, Score - 10);
            }
            else
            {
                LastJudge = "MISS";
            }
            CurrentCombo = 0; // 콤보 초기화
        }

        // 점수에 따른 랭크 계산
        public string GetRank()
        {
            if (Score >= 5000) return "S";
            if (Score >= 3000) return "A";
            if (Score >= 1500) return "B";
            if (Score >= 500) return "C";
            return "F";
        }
    }
}