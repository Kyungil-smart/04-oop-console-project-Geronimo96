using System;

namespace RhythmGameOOP
{
    public class ScoreManager
    {
        // 외부에서 읽을 수는 있지만(get), 수정은 이 클래스 내부에서만(private set) 가능
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public string LastJudge { get; private set; } = "READY"; // 판정 문구 (Perfect/Good/Miss)

        // 점수 추가 메서드
        public void AddScore(int amount, string judgeText)
        {
            Score += amount;
            Combo++;
            LastJudge = judgeText;
        }

        // 콤보 초기화 (틀렸을 때)
        public void ResetCombo()
        {
            // 콤보가 이어지고 있었는데 틀린 경우에만 감점
            if (Combo > 0)
            {
                LastJudge = "MISS";
                // 점수가 음수가 되지 않도록 0과 비교 (Math.Max)
                Score = Math.Max(0, Score - 10);
            }
            Combo = 0; // 콤보는 무조건 0으로
        }
    }
}