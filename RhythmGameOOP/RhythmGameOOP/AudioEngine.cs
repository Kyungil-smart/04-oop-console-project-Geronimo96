using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RhythmGameOOP
{
    public class AudioEngine
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        private string currentAlias;

        public void Play(string filePath, int volume)
        {
            Close(); // 기존 음악 끄기

            // 별명 생성 (겹치지 않게)
            currentAlias = "Media" + DateTime.Now.Ticks;

            // [수정된 부분] "type mpegvideo"를 제거했습니다.
            // 이제 윈도우가 알아서 파일 형식(mp3, wav 등)을 판단합니다.
            string openCommand = $"open \"{filePath}\" alias {currentAlias}";

            mciSendString(openCommand, null, 0, IntPtr.Zero);

            // 재생 및 볼륨 설정
            mciSendString($"play {currentAlias}", null, 0, IntPtr.Zero);
            SetVolume(volume);
        }

        public void SetVolume(int volume)
        {
            // 0 ~ 100 사이 값으로 제한
            int clampedVolume = Math.Max(0, Math.Min(volume, 100));
            int internalVolume = clampedVolume * 10; // 윈도우는 0~1000 사용

            if (!string.IsNullOrEmpty(currentAlias))
            {
                mciSendString($"setaudio {currentAlias} volume to {internalVolume}", null, 0, IntPtr.Zero);
            }
        }

        public void Stop()
        {
            if (!string.IsNullOrEmpty(currentAlias))
            {
                mciSendString($"stop {currentAlias}", null, 0, IntPtr.Zero);
                Close();
            }
        }

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