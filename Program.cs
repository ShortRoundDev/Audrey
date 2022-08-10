using System.Diagnostics;
using System.CommandLine;
using System.Text.Json;

using Audrey.Models;

namespace Audrey
{
    public class Program
    {
        public static void Main(string path, string outputPath = "./", string? audible = null)
        {
            string activationBytes = "";
            Console.Write("Getting activation bytes from audible...");
            using (Process audibleproc = new Process()) {
                if(audible == null)
                {
                    audibleproc.StartInfo.FileName = "audible.exe";
                } else
                {
                    audibleproc.StartInfo.FileName = Path.Join(audible, "audible.exe")?.ToString();
                }

                audibleproc.StartInfo.Arguments = "activation-bytes";
                audibleproc.StartInfo.RedirectStandardOutput = true;

                audibleproc.Start();

                activationBytes = audibleproc.StandardOutput.ReadToEnd().Trim();
                audibleproc.WaitForExit();
            }
            Console.WriteLine("done!");

            string chapterInfo = "";
            Console.Write("Getting chapter information from ffprobe...");
            using (Process ffprobe = new Process())
            {
                ffprobe.StartInfo.FileName = "ffprobe.exe";
                ffprobe.StartInfo.Arguments = string.Join(" ", "-i", path, "-sexagesimal", "-print_format", "json", "-show_chapters");
                ffprobe.StartInfo.RedirectStandardOutput = true;
                ffprobe.Start();

                chapterInfo = ffprobe.StandardOutput.ReadToEnd().Trim();
                ffprobe.WaitForExit();
            }

            if (string.IsNullOrEmpty(chapterInfo))
            {
                Console.Error.WriteLine("Couldn't get chapter info from ffprobe!");
                return;
            } else
            {
                Console.WriteLine("done!");
            }

            //ffmpeg -activation_bytes 27fa4908 -i .\Masters_of_Doom_B008K8BQG6.aax -acodec libmp3lame .\Masters_of_Doom_B008K8BQG6.mp3

            var chapters = JsonSerializer.Deserialize<FfprobeOutput>(chapterInfo);

            Console.Write("Converting aax to mp3...");
            var filename = Path.GetFileNameWithoutExtension(path);

            Parallel.ForEach(chapters.Chapters, chapter =>
            {
                using (Process ffmpeg = new Process())
                {
                    ffmpeg.StartInfo.FileName = "ffmpeg.exe";
                    ffmpeg.StartInfo.Arguments = String.Join(" ",
                        "-activation_bytes", activationBytes,
                        "-i", path,
                        "-acodec", "libmp3lame",
                        "-ss", chapter.StartTime,
                        "-to", chapter.EndTime,
                        Path.Join(outputPath, filename + "_" + chapter.Tags.Title.Replace(" ", "_").Replace(":", "_") + ".mp3")
                    );
                    ffmpeg.Start();
                    ffmpeg.WaitForExit();
                }
            });
            Console.WriteLine("DONE!!!!");
        }
    }
}