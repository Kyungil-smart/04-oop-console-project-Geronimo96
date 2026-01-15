using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RhythmGameOOP
{
    // [오디오 클래스] WAV 파일 재생에 최적화된 버전입니다.
    public class AudioEngine
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        // 현재 재생 중인 음악의 ID (별명)
        private string currentAlias;

        // [음악 재생 함수]
        public void Play(string filePath)
        {
            // 1. 이미 재생 중인 음악이 있다면 끄고 리소스를 정리합니다.
            Close();

            // 2. 음악 별명 생성 (고유한 이름을 줘서 겹치지 않게 함)
            currentAlias = "Media" + DateTime.Now.Ticks;

            // 3. [WAV 최적화] 'type waveaudio'를 명시합니다.
            // 이렇게 하면 윈도우가 WAV 파일을 가장 빠르고 가볍게 처리합니다.
            // (mp3를 억지로 wav처럼 열거나 하지 않아 오류가 적습니다.)
            string openCommand = $"open \"{filePath}\" type waveaudio alias {currentAlias}";
            mciSendString(openCommand, null, 0, IntPtr.Zero);

            // 4. 재생 시작
            mciSendString($"play {currentAlias}", null, 0, IntPtr.Zero);
        }

        // [볼륨 조절 함수]
        public void SetVolume(int volume)
        {
            // [주의] .wav 파일(waveaudio 타입)은 mciSendString으로 볼륨 조절이 불가능합니다.
            // 억지로 'setaudio' 명령어를 보내면 게임이 멈추거나 에러가 발생할 수 있습니다.
            // 따라서 WAV 모드일 때는 이 함수가 아무 동작도 안 하게 막는 것이 '최적화'입니다.

            // (만약 볼륨 조절이 꼭 필요하다면 파일을 .mp3로 변환해야 합니다.)
            return;
        }

        // [재생 길이 확인 함수]
        public double GetDuration()
        {
            if (string.IsNullOrEmpty(currentAlias)) return 0;

            StringBuilder lengthBuf = new StringBuilder(32);

            // 현재 음악의 길이를 물어봅니다.
            mciSendString($"status {currentAlias} length", lengthBuf, 32, IntPtr.Zero);

            if (long.TryParse(lengthBuf.ToString(), out long ms))
            {
                // 밀리초(ms) 단위를 초(s) 단위로 바꿔서 반환
                return ms / 1000.0;
            }
            return 0;
        }

        // [정지 함수]
        public void Stop()
        {
            if (!string.IsNullOrEmpty(currentAlias))
            {
                mciSendString($"stop {currentAlias}", null, 0, IntPtr.Zero);
                Close();
            }
        }

        // [자원 해제 함수]
        private void Close()
        {
            if (!string.IsNullOrEmpty(currentAlias))
            {
                mciSendString($"close {currentAlias}", null, 0, IntPtr.Zero);
                currentAlias = null;
            }
        }
    }
}